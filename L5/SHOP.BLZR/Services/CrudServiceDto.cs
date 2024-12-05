using Shared.Models;
using Shared.Models.Dto;
using Shared.Services;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


namespace Shop.BLZR.Services
{
    public class CrudServiceDto<T, TKey> : ICrudService<T, TKey>
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _endpoint;

        public CrudServiceDto(HttpClient httpClient, string endpoint)
        {
            _httpClient = httpClient;
            _endpoint = endpoint;
        }

        public async Task<ServiceReponse<IEnumerable<T>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_endpoint}");
            return await DeserializeResponse<IEnumerable<T>>(response);
        }

        public async Task<ServiceReponse<IEnumerable<T>>> GetAllAsyncs(ProductDto filterData)
        {
            // Pobieramy odpowiedź z endpointu
            var response = await _httpClient.GetAsync($"{_endpoint}");

            // Deserializujemy odpowiedź
            var data = await DeserializeResponse<IEnumerable<T>>(response);

            ObservableCollection<ProductDto>? products = new ObservableCollection<ProductDto>((IEnumerable<ProductDto>)data.Data);

            var filteredProducts = products.AsQueryable();

            if (!string.IsNullOrEmpty(filterData.Name))
            {
                filteredProducts = filteredProducts.Where(p => p.Name.Contains(filterData.Name));
            }

            if (filterData.Price.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price == filterData.Price.Value);
            }

            // Filtracja po CategoryId, jeśli wartość jest podana w filterData
            if (filterData.CategoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.CategoryId == filterData.CategoryId.Value);
            }

            // Filtracja po StockId, jeśli wartość jest podana w filterData
            if (filterData.StockId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.StockId == filterData.StockId.Value);
            }

            var finalData = filteredProducts.Cast<T>().ToList();

            // Zwracamy odpowiedź z przefiltrowanymi danymi
            return new ServiceReponse<IEnumerable<T>>
            {
                Data = finalData,
                Success = true
            };
        }


        public async Task<ServiceReponse<T>> GetByIdAsync(TKey id)
        {
            var response = await _httpClient.GetAsync($"{_endpoint}/{id}");
            return await DeserializeResponse<T>(response);
        }

        public async Task<ServiceReponse<T>> CreateAsync(T entity)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_endpoint}", entity);
            return await DeserializeResponse<T>(response);
        }

        public async Task<ServiceReponse<T>> UpdateAsync(TKey? id, T entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_endpoint}/{id}", entity);
            return await DeserializeResponse<T>(response);
        }

        public virtual async Task<ServiceReponse<bool>> DeleteAsync(TKey? id)
        {
            var response = await _httpClient.DeleteAsync($"{_endpoint}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return new ServiceReponse<bool>() { Success = true, Data = true };
            }

            return new ServiceReponse<bool>() { Success = false, Message = $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}" };
        }

        public virtual async Task<ServiceReponse<bool>> DeleteAllAsync()
        {
            var response = await _httpClient.DeleteAsync($"{_endpoint}");

            if (response.IsSuccessStatusCode)
            {
                return new ServiceReponse<bool>() { Success = true, Data = true };
            }

            return new ServiceReponse<bool>() {Success = false, Message = $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}"};
        }

        private async Task<ServiceReponse<TData>> DeserializeResponse<TData>(HttpResponseMessage response)
        {
            var serviceResponse = new ServiceReponse<TData>();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<TData>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    serviceResponse.Data = data;
                    serviceResponse.Success = true;
                    serviceResponse.Message = "Operation succeeded"; 
                }
                catch (JsonException ex)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Deserialization failed: {ex.Message}";
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error: {response.StatusCode} - {errorContent}";
            }

            return serviceResponse;
        }

    }

}
