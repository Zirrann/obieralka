using Shop.WPF.ViewModels;  

namespace Shop.WPF.Views  
{
    public partial class ProductDetailsPage : Window
    {
        public ProductDetailsPage(ProductDetailsViewModel viewModel)
        {
            InitializeComponent();  
            this.DataContext = viewModel; 
        }
    }
}
