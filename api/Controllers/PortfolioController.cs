﻿using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IFMPService _fmpService;

        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepository, IPortfolioRepository portfolioRepository, IFMPService fmpService)
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStockToPortfolio(string symbol)
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepository.GetBySymbol(symbol);
            if (stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)
                {
                    return BadRequest("Stock does not exists.");
                }
                else
                {
                    await _stockRepository.CreateAsync(stock);
                }
            }

            if (stock == null)
            {
                return BadRequest("Stock not found");
            }

            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

            if (userPortfolio.Any(e => e.Symbol.Equals(symbol, StringComparison.CurrentCultureIgnoreCase)))
            {
                return BadRequest("Cannot add same stock to portfolio");
            }

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = appUser.Id,
            };

            await _portfolioRepository.CreateAsync(portfolioModel);

            if (portfolioModel == null)
            {
                return StatusCode(500, "Could not create");
            }

            return Created();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteStockFromThePortfolio(string symbol)
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
            var filteredStock = userPortfolio.Where(s => s.Symbol.Equals(symbol, StringComparison.CurrentCultureIgnoreCase)).ToList();

            if (filteredStock.Any())
            {
                await _portfolioRepository.DeletePortfolio(appUser, symbol);
            }
            else
            {
                return BadRequest("Stock not in your portfolio");
            }

            return Ok();
        }
    }
}
