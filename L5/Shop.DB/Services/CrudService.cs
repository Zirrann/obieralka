using Microsoft.EntityFrameworkCore;
using Shared.Models.Dto;
using Shared.Services;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shop.DB.Services
{
    public class CrudService<T, TKey> : ICrudService<T, TKey> where T : class
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public CrudService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        protected ServiceReponse<TResponse> HandleException<TResponse>(Exception ex)
        {
            return new ServiceReponse<TResponse>
            {
                Success = false,
                Message = ex.Message,
                Data = default
            };
        }
        public virtual async Task<ServiceReponse<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var data = await _dbSet.ToListAsync();
                return new ServiceReponse<IEnumerable<T>>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<T>>(ex);
            }
        }

        public virtual async Task<ServiceReponse<IEnumerable<T>>> GetFilteredProducts(ObservableCollection<ProductDto> products, ObservableCollection<CategoryDto> categories,
                        ObservableCollection<StockDto> stocks, ProductDto filterData, ProductDto sortBy, bool sortRising, int pageNumber, int pageSize)
        {
            // Połączenie produktów z kategoriami i magazynami
            var combinedProducts = products
                .Join(categories,
                    p => p.CategoryId,
                    c => c.CategoryId,
                    (p, c) => new { Product = p, Category = c })
                .Join(stocks,
                    pc => pc.Product.StockId,
                    s => s.StockId,
                    (pc, s) => new { pc.Product, pc.Category, Stock = s })
                .AsQueryable();

            // Filtracja
            if (!string.IsNullOrEmpty(filterData.Name))
            {
                combinedProducts = combinedProducts.Where(x => x.Product.Name.Contains(filterData.Name));
            }

            if (filterData.Price.HasValue)
            {
                combinedProducts = combinedProducts.Where(x => x.Product.Price == filterData.Price.Value);
            }

            if (filterData.CategoryId.HasValue)
            {
                combinedProducts = combinedProducts.Where(x => x.Product.CategoryId == filterData.CategoryId.Value);
            }

            if (filterData.StockId.HasValue)
            {
                combinedProducts = combinedProducts.Where(x => x.Stock.StockId == filterData.StockId.Value && x.Stock.Quantity > 0);
            }

            // Sortowanie
            if (!string.IsNullOrEmpty(sortBy.Name))
            {
                combinedProducts = sortRising
                    ? combinedProducts.OrderBy(x => x.Product.Name)
                    : combinedProducts.OrderByDescending(x => x.Product.Name);
            }
            else if (sortBy.Price.HasValue)
            {
                combinedProducts = sortRising
                    ? combinedProducts.OrderBy(x => x.Product.Price)
                    : combinedProducts.OrderByDescending(x => x.Product.Price);
            }
            else if (!string.IsNullOrEmpty(sortBy.CategoryId?.ToString()))
            {
                combinedProducts = sortRising
                    ? combinedProducts.OrderBy(x => x.Category.Name)
                    : combinedProducts.OrderByDescending(x => x.Category.Name);
            }
            else if (!string.IsNullOrEmpty(sortBy.StockId?.ToString()))
            {
                combinedProducts = sortRising
                    ? combinedProducts.OrderBy(x => x.Stock.Quantity)
                    : combinedProducts.OrderByDescending(x => x.Stock.Quantity);
            }

            // Stronicowanie
            var pagedData = combinedProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.Product) // Pobieramy tylko produkty
                .Cast<T>()
                .ToList();

            // Zwracamy odpowiedź z przefiltrowanymi danymi
            return new ServiceReponse<IEnumerable<T>>
            {
                Data = pagedData,
                Success = true
            };
        }



        public virtual async Task<ServiceReponse<T>> GetByIdAsync(TKey id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return new ServiceReponse<T>
                    {
                        Success = false,
                        Message = "Entity not found."
                    };
                }

                return new ServiceReponse<T>
                {
                    Data = entity,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        public virtual async Task<ServiceReponse<T>> CreateAsync(T entity)
        {
            try
            {
                _dbSet.Add(entity);
                await _dbContext.SaveChangesAsync();
                return new ServiceReponse<T>
                {
                    Data = entity,
                    Success = true,
                    Message = "Entity created successfully."
                };
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        public virtual async Task<ServiceReponse<T>> UpdateAsync(TKey? id, T entity)
        {
            try
            {
                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity == null)
                {
                    return new ServiceReponse<T>
                    {
                        Success = false,
                        Message = "Entity not found."
                    };
                }

                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _dbContext.SaveChangesAsync();
                return new ServiceReponse<T>
                {
                    Data = entity,
                    Success = true,
                    Message = "Entity updated successfully."
                };
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        public virtual async Task<ServiceReponse<bool>> DeleteAsync(TKey? id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return new ServiceReponse<bool>
                    {
                        Success = false,
                        Message = "Entity not found.",
                        Data = false
                    };
                }

                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return new ServiceReponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Entity deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return HandleException<bool>(ex);
            }
        }

        public virtual async Task<ServiceReponse<bool>> DeleteAllAsync()
        {
            try
            {
                // Usunięcie wszystkich rekordów w tabeli
                _dbSet.RemoveRange(_dbSet);
                await _dbContext.SaveChangesAsync();

                return new ServiceReponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "All entities deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return HandleException<bool>(ex);
            }
        }

    }
}
