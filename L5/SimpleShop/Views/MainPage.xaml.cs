using Shop.WPF.ViewModels;

namespace Shop.WPF.Views
{
    public partial class MainPage : Window  
    {
        public MainPage(OrderViewModel viewModel)
        {
            InitializeComponent(); 
            this.DataContext = viewModel;
        }
    }
}
