using api.Models;

namespace api.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> CreateAsync(Portfolio portfolio);   
        Task<List<Stock>> GetUserPortfolio(AppUser user);
        Task<Portfolio> DeletePortfolio(AppUser user, string symbol);
    }
}
