namespace Plated.Core.Navigation;

public interface IAppNavigator
{
    void ShowMainApp();

    void ShowLogin();

    Task ShowCreateAccountAsync();

    Task GoBackAsync();
}
