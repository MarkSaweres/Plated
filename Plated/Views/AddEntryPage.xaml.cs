using Plated.Core.ViewModels;

namespace Plated.Views;

public partial class AddEntryPage : ContentPage
{
    public AddEntryPage(AddEntryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
