using L4.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Dto;
using Shop.MAUI.Services.ServicesDto;

namespace Shop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServiceDto _productServiceDto;
        private readonly ICategoryServiceDto _categoryServiceDto;
        private readonly IStockServiceDto _stockServiceDto;

        public ProductController(
            IProductServiceDto productServiceDto,
            ICategoryServiceDto categoryServiceDto,
            IStockServiceDto stockServiceDto)
        {
            _productServiceDto = productServiceDto;
            _categoryServiceDto = categoryServiceDto;
            _stockServiceDto = stockServiceDto;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productServiceDto.GetAllAsync();
            if (response.Success)
            {
                return View(response.Data);
            }

            ViewBag.Error = "Failed to load products.";
            return View(new List<ProductDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _productServiceDto.GetByIdAsync(id);
            if (response.Success)
            {
                return View(response.Data);
            }

            ViewBag.Error = "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productServiceDto.DeleteAsync(id);
            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductDto product)
        {
            if (ModelState.IsValid)
            {
                var response = await _productServiceDto.CreateAsync(product);
                if (response.Success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = response.Message;
            }

            return View("Index", await _productServiceDto.GetAllAsync());
        }
    }
}
