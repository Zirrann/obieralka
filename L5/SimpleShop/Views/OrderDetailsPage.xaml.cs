using Newtonsoft.Json;
using Shared.Models;
using Shared.Models.Dto;
using Shop.WPF.ViewModels; 

namespace Shop.WPF.Views 
{
    public partial class OrderDetailsPage : Window
    {
        public List<OrderProductDto> OrderProducts { get; set; }

        public OrderDetailsPage(OrderDetailsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel; 
        }
    }
}
