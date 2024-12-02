using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Shared.Models.Dto;
using Shop.WPF.Services.ServicesDto;
using Shared.Models;
using Shared.Models.Dto;

namespace Shop.WPF.ViewModels
{
    public partial class OrderViewModel : ObservableObject
    {
        private readonly IMessageDialogService _messageDialogService;
        private readonly IOrderServiceDto _orderServiceDto;

        [ObservableProperty]
        private ObservableCollection<OrderDto> orders;

        public OrderViewModel(
            IMessageDialogService messageDialogService,
            IOrderServiceDto orderService)
        {
            _messageDialogService = messageDialogService;
            _orderServiceDto = orderService;

            LoadOrders();
        }

        private async void LoadOrders()
        {
            var response = await _orderServiceDto.GetAllAsync();
            if (response.Success)
            {
                Orders = new ObservableCollection<OrderDto>(response.Data);
            }
            else
            {
                _messageDialogService.ShowMessage("Failed to load orders");
            }
        }

        [RelayCommand]
        private async Task DeleteOrderAsync(OrderDto order)
        {
            if (order == null)
                return;

            var response = await _orderServiceDto.DeleteAsync(order.OrderId);
            if (response.Success)
            {
                Orders.Remove(order);
            }
            else
            {
                _messageDialogService.ShowMessage(response.Message);
            }
        }

        [RelayCommand]
        private void NavigateToProducts()
        {
            // Otwieranie okna z listą produktów
            var productsWindow = new ProductsWindow();
            productsWindow.Show();
        }

        [RelayCommand]
        private void ViewOrderDetails(OrderDto order)
        {
            if (order == null)
                return;

            // Tworzymy i otwieramy nowe okno dla szczegółów zamówienia
            var orderDetailsWindow = new OrderDetailsWindow
            {
                DataContext = new OrderDetailsViewModel(
                    _orderServiceDto,
                    _messageDialogService,
                    order.OrderProducts)
            };

            orderDetailsWindow.ShowDialog();
        }
    }
}
