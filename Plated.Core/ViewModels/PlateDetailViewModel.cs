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
    private readonly IPhotoCaptureService _photoCaptureService;
    private readonly IStorageService _storageService;

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
    private string? pendingPhotoPath;

    public PlateDetailViewModel(
        IPlateService plateService,
        IAuthService authService,
        IPhotoCaptureService photoCaptureService,
        IStorageService storageService)
    {
        _plateService = plateService;
        _authService = authService;
        _photoCaptureService = photoCaptureService;
        _storageService = storageService;
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
        catch (Exception)
        {
            ErrorMessage = "Couldn't load this plate. Check your connection and try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AttachPhotoAsync()
    {
        var choice = await Shell.Current.DisplayActionSheetAsync("Add a photo", "Cancel", null, "Take Photo", "Choose from Gallery");
        if (choice == "Take Photo")
        {
            PendingPhotoPath = await _photoCaptureService.CapturePhotoAsync();
        }
        else if (choice == "Choose from Gallery")
        {
            PendingPhotoPath = await _photoCaptureService.PickPhotoAsync();
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

            string? photoUrl = null;
            if (!string.IsNullOrEmpty(PendingPhotoPath))
            {
                photoUrl = await _storageService.UploadPlatePhotoAsync(PendingPhotoPath, _plateId);
            }

            var comment = await _plateService.AddCommentAsync(_plateId, user, NewCommentText.Trim(), photoUrl);
            Comments.Insert(0, comment);
            CommentCount++;
            NewCommentText = string.Empty;
            PendingPhotoPath = null;
        }
        catch (Exception)
        {
            ErrorMessage = "Couldn't post your comment. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ReportCommentAsync(PlateComment? comment)
    {
        if (comment is null || _plateId is null)
        {
            return;
        }

        var user = _authService.CurrentUser;
        if (user is null)
        {
            return;
        }

        var reasonChoice = await Shell.Current.DisplayActionSheetAsync(
            "Report this comment", "Cancel", null, "Spam", "Harassment", "Inaccurate", "Other");

        if (reasonChoice is null || reasonChoice == "Cancel")
        {
            return;
        }

        var reason = reasonChoice switch
        {
            "Spam" => ReportReason.Spam,
            "Harassment" => ReportReason.Harassment,
            "Inaccurate" => ReportReason.Inaccurate,
            _ => ReportReason.Other,
        };

        try
        {
            await _plateService.ReportCommentAsync(_plateId, comment.Id, user.Uid, reason, null);
            await Shell.Current.DisplayAlertAsync("Thanks", "This comment has been reported.", "OK");
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Couldn't submit the report. Please try again.", "OK");
        }
    }
}
