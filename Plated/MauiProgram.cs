using CommunityToolkit.Maui;
using Microsoft.Maui;
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
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        ConfigureHandlers();
        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterPages(builder.Services);

        return builder.Build();
    }

    /// <summary>Removes Android's default Material underline so our rounded field containers look clean.</summary>
    private static void ConfigureHandlers()
    {
#if ANDROID
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) => handler.PlatformView.Background = null);
        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("Borderless", (handler, view) => handler.PlatformView.Background = null);
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) => handler.PlatformView.Background = null);
#endif
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAuthService, FirebaseAuthService>();
        services.AddSingleton<IPlateService, FirestorePlateService>();
        services.AddSingleton<IPhotoCaptureService, MediaPickerPhotoCaptureService>();
        services.AddSingleton<IPlateOcrService, PlateOcrService>();
        services.AddSingleton<IAppNavigator, AppNavigator>();

#if ANDROID
        services.AddSingleton<IGoogleSignInService, GoogleSignInService>();
#elif IOS
        services.AddSingleton<IGoogleSignInService, GoogleSignInService>();
#endif
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<CreateAccountViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<AddEntryViewModel>();
        services.AddTransient<PlateDetailViewModel>();
        services.AddTransient<ProfileViewModel>();
    }

    private static void RegisterPages(IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<CreateAccountPage>();
        services.AddTransient<SearchPage>();
        services.AddTransient<AddEntryPage>();
        services.AddTransient<PlateDetailPage>();
        services.AddTransient<ProfilePage>();
        services.AddTransient<AppShell>();
    }
}
