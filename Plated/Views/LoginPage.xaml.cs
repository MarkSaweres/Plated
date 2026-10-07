using Plated.Core.UI;
using Plated.Core.ViewModels;

namespace Plated.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        PageStyling.ApplyLightStatusBar(this);
        BindingContext = viewModel;
    }
}
