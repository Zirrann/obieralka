using Shared.Models.Dto;
using Shop.BLZR.Services.ServicesDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLZR.Services
{
    public class StockServiceDto : CrudServiceDto<StockDto, int>, IStockServiceDto
    {
        public StockServiceDto(HttpClient httpClient) 
            : base(httpClient, "api/Stock")
        {
        }
    }
}
