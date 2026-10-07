using Plated.Core.UI;
using Plated.Core.ViewModels;

namespace Plated.Views;

public partial class CreateAccountPage : ContentPage
{
    public CreateAccountPage(CreateAccountViewModel viewModel)
    {
        InitializeComponent();
        PageStyling.ApplyLightStatusBar(this);
        BindingContext = viewModel;
    }
}
