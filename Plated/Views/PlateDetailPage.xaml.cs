using Plated.Core.ViewModels;

namespace Plated.Views;

public partial class PlateDetailPage : ContentPage
{
    public PlateDetailPage(PlateDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
