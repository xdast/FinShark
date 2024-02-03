using api.Data;
using api.Dtos.Stock;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.AddAsync(stockModel);
            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stockModel == null) 
            {
                return null;
            }

            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync() => await _context.Stocks.Include(c => c.Comments).ToListAsync();

        public async Task<Stock?> GetByIdAsync(int id) => await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(i => id == i.Id);

        public async Task<bool> StackExists(int id) => await _context.Stocks.AnyAsync(x => x.Id == id);

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (stock == null)
            {
                return null;
            }

            stock.Symbol = stockDto.Symbol;
            stock.Purchase = stockDto.Purchase;
            stock.Industry = stockDto.Industry;
            stock.LastDiv = stockDto.LastDiv;
            stock.CompanyName = stockDto.CompanyName;
            stock.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync();

            return stock;
        }
    }
}
