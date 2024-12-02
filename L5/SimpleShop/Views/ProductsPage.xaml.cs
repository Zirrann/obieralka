using System.Windows;
using Shared.Models.Dto;
using Shop.WPF.ViewModels;

namespace Shop.WPF.Views
{
    public partial class ProductsPage : Window
    {
        public ProductsPage(ProductsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;  // Ustawienie ViewModelu jako DataContext
        }

        // Rejestracja trasy lub nawigacji, jeœli potrzebna
        private void NavigateToProductDetails(ProductDto product)
        {
            var detailsPage = new ProductDetailsPage(product);
            detailsPage.Show();  // Otwórz stronê z detalami produktu
        }
    }
}
