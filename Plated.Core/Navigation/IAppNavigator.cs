namespace Plated.Core.Navigation;

public interface IAppNavigator
{
    void ShowMainApp();

    void ShowLogin();

    void ShowVerifyEmail();

    Task ShowCreateAccountAsync();

    Task GoBackAsync();
}
