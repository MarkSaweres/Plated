using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plated.Core.Navigation;
using Plated.Core.Services;
using Plated.Core.ViewModels;
using Plated.Services;
using Plated.Views;
using Plugin.Maui.OCR;

namespace Plated;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseOcr()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Caveat-Bold.ttf", "CaveatBold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterPages(builder.Services);

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAuthService, FirebaseAuthService>();
        services.AddSingleton<IPlateService, FirestorePlateService>();
        services.AddSingleton<IStorageService, FirebaseStorageService>();
        services.AddSingleton<IPhotoCaptureService, MediaPickerPhotoCaptureService>();
        services.AddSingleton<IPlateOcrService, PlateOcrService>();
        services.AddSingleton<IAppNavigator, AppNavigator>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<AddEntryViewModel>();
        services.AddTransient<PlateDetailViewModel>();
        services.AddTransient<ProfileViewModel>();
    }

    private static void RegisterPages(IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<SearchPage>();
        services.AddTransient<AddEntryPage>();
        services.AddTransient<PlateDetailPage>();
        services.AddTransient<ProfilePage>();
        services.AddTransient<AppShell>();
    }
}
