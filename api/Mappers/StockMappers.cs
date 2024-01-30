using api.Dtos.Stock;
using api.Models;

namespace api.Mappers
{
    public static class StockMappers
    {
        public static StockDto ToStockDto(this Stock stockModel) 
        {
            return new StockDto
            {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap,
            };
        }

        public static Stock ToStockFromCreateDto(this CreateStockRequestDto stockdto) 
        {
            return new Stock
            {
                Symbol = stockdto.Symbol,
                CompanyName = stockdto.CompanyName,
                Purchase = stockdto.Purchase,
                LastDiv = stockdto.LastDiv,
                Industry = stockdto.Industry,
                MarketCap = stockdto.MarketCap,
            };
        }
    }
}
