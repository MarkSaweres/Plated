using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Plated.Core.Models;
using Plated.Core.Navigation;
using Plated.Core.Services;

namespace Plated.Core.ViewModels;

public partial class PlateDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IPlateService _plateService;
    private readonly IAuthService _authService;

    private string? _plateId;

    public ObservableCollection<PlateComment> Comments { get; } = new();

    [ObservableProperty]
    private string state = string.Empty;

    [ObservableProperty]
    private string plateNumber = string.Empty;

    [ObservableProperty]
    private int commentCount;

    [ObservableProperty]
    private string newCommentText = string.Empty;

    [ObservableProperty]
    private bool isReportSheetOpen;

    [ObservableProperty]
    private bool isReportSent;

    [ObservableProperty]
    private string? reportError;

    private PlateComment? _reportTarget;

    public bool IsSheetVisible => IsReportSheetOpen;

    partial void OnIsReportSheetOpenChanged(bool value) => OnPropertyChanged(nameof(IsSheetVisible));

    public PlateDetailViewModel(
        IPlateService plateService,
        IAuthService authService)
    {
        _plateService = plateService;
        _authService = authService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        State = query.TryGetValue(Routes.StateQueryKey, out var s) ? Uri.UnescapeDataString(s.ToString() ?? string.Empty) : string.Empty;
        PlateNumber = query.TryGetValue(Routes.PlateNumberQueryKey, out var p) ? Uri.UnescapeDataString(p.ToString() ?? string.Empty) : string.Empty;

        _plateId = null;
        Comments.Clear();
        CommentCount = 0;

        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            var plate = await _plateService.FindPlateAsync(State, PlateNumber);
            _plateId = plate?.Id;
            CommentCount = plate?.CommentCount ?? 0;

            Comments.Clear();
            if (plate is not null)
            {
                var comments = await _plateService.GetCommentsAsync(plate.Id);
                foreach (var comment in comments)
                {
                    Comments.Add(comment);
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Couldn't load this plate. Check your connection and try again.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddCommentAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCommentText) || IsBusy)
        {
            return;
        }

        var user = _authService.CurrentUser;
        if (user is null)
        {
            ErrorMessage = "Please sign in again.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            _plateId ??= (await _plateService.GetOrCreatePlateAsync(State, PlateNumber, user.Uid)).Id;

            var comment = await _plateService.AddCommentAsync(_plateId, user, NewCommentText.Trim());
            Comments.Insert(0, comment);
            CommentCount++;
            NewCommentText = string.Empty;
        }
        catch (Exception ex)
        {
            ShowError("Couldn't post your comment. Please try again.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ReportComment(PlateComment? comment)
    {
        if (comment is null || _plateId is null)
        {
            return;
        }

        _reportTarget = comment;
        IsReportSent = false;
        ReportError = null;
        IsReportSheetOpen = true;
    }

    [RelayCommand]
    private async Task SubmitReportAsync(string? reasonKey)
    {
        var user = _authService.CurrentUser;
        if (_reportTarget is null || _plateId is null || user is null || IsBusy)
        {
            return;
        }

        var reason = reasonKey switch
        {
            "spam" => ReportReason.Spam,
            "harassment" => ReportReason.Harassment,
            "inaccurate" => ReportReason.Inaccurate,
            _ => ReportReason.Other,
        };

        IsBusy = true;
        ReportError = null;
        try
        {
            await _plateService.ReportCommentAsync(_plateId, _reportTarget.Id, user.Uid, reason, null);
            IsReportSent = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Plated] Report failed: {ex}");
            ReportError = "Couldn't submit the report. Please try again.";
#if DEBUG
            ReportError += $" ({ex.Message})";
#endif
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void CloseSheets()
    {
        IsReportSheetOpen = false;
        _reportTarget = null;
    }
}
