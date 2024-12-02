using L4.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.WPF.Views;
using SimpleShop;
using System;
using System.Windows;

namespace Shop.WPF
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Konfiguracja DI
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            // Ustawienie głównego okna aplikacji
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Rejestracja serwisów, ViewModeli i widoków
            ConfigureAppServices(services);
            ConfigureViewModels(services);
            ConfigureViews(services);
        }

        private void ConfigureAppServices(IServiceCollection services)
        {
            // Rejestracja serwisów
            services.AddSingleton<ICategoryServiceDto, CategoryServiceDto>();
            services.AddSingleton<IOrderProductServiceDto, OrderProductServiceDto>();
            services.AddSingleton<IOrderServiceDto, OrderServiceDto>();
            services.AddSingleton<IProductServiceDto, ProductServiceDto>();
            services.AddSingleton<IStockServiceDto, StockServiceDto>();
            services.AddSingleton<IMessageDialogService, WpfMessageDialogService>();

            services.AddSingleton(sp => new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5104/")
            });
        }

        private void ConfigureViewModels(IServiceCollection services)
        {
            // Rejestracja ViewModeli
            services.AddSingleton<OrderViewModel>();
            services.AddTransient<ProductsViewModel>();
            services.AddTransient<ProductDetailsViewModel>();
            services.AddTransient<OrderDetailsViewModel>();
        }

        private void ConfigureViews(IServiceCollection services)
        {
            // Rejestracja widoków
            services.AddSingleton<MainWindow>();
            services.AddTransient<ProductsView>();
            services.AddTransient<ProductDetailsView>();
            services.AddTransient<OrderDetailsView>();
        }
    }
}
