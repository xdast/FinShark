using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Models;
using api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace api.tests.Repository
{
    [TestFixture]
    internal class StockRepositoryTests
    {
        private ApplicationDbContext _context;

        [SetUp] 
        public void SetUp() 
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            if (!_context.Stocks.Any())
            {
                for (int i = 1; i <= 10; i++)
                {
                    _context.Stocks.Add(
                      new Stock()
                      {
                          Symbol = "CDPR",
                          CompanyName = "CD Project Red",
                          Purchase = 300,
                          LastDiv = 0.40M,
                          Industry = "Game Development",
                          MarketCap = 3,
                          Comments = new List<Comment>
                          {
                            new Comment()
                            {
                                Title = "test",
                                Content = "test",
                                AppUserId = "1"
                            }
                          }
                      });

                    _context.SaveChanges();
                }
            }
        }

        [Test]
        public async Task StockRepository_CreateAsync_ShouldReturnStock() 
        {
            //Arrange
            var stock = new Stock()
            {
                Symbol = "CDPR",
                CompanyName = "CD Project Red",
                Purchase = 300,
                LastDiv = 0.40M,
                Industry = "Game Development",
                MarketCap = 3,
                Comments = new List<Comment>
                          {
                            new Comment()
                            {
                                Title = "test",
                                Content = "test",
                                AppUserId = "1"
                            }
                          }
            };

            var stockRepository = new StockRepository(_context);

            //Act
            var result = await stockRepository.CreateAsync(stock);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(stock));
        }

        [Test]
        public async Task StockRepository_GetByIdAsync_ShouldReturnStock()
        {
            //Arrange
            var id = 3;
            var stockRepository = new StockRepository(_context);

            //Act
            var result = await stockRepository.GetByIdAsync(id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Stock>());
                Assert.That(id, Is.EqualTo(result.Id));
            });
        }

        [Test]
        public async Task StockRepository_GetBySymbol_ShouldReturnStock()
        {
            //Arrange
            var stockRepository = new StockRepository(_context);

            //Act
            var symbol = "CDPR";
            var result = await stockRepository.GetBySymbol(symbol);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Stock>());
                Assert.That(symbol, Is.EqualTo(result.Symbol));
            });
        }

        [Test]
        public async Task StockRepository_GetAllAsync_ShouldReturnAllStocks()
        {
            //Arrange
            var queryObject = new QueryObject();
            var stockRepository = new StockRepository(_context);

            //Act
            var result = await stockRepository.GetAllAsync(queryObject);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<List<Stock>>());
                Assert.That(result, Has.Count.EqualTo(10));
            });
        }

        [Test]
        public async Task StockRepository_UpdateAsync_ShouldReturnUpdatedStock()
        {
            //Arrange
            var stockDto = new UpdateStockRequestDto
            {
                Symbol = "TSLA"
            };

            var id = 3;
            var stockRepository = new StockRepository(_context);

            //Act
            var result = await stockRepository.UpdateAsync(id, stockDto);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Stock>());
                Assert.That(result.Id, Is.EqualTo(id));
                Assert.That(result.Symbol, Is.EqualTo(stockDto.Symbol));
            });
        }

        [Test]
        public async Task StockRepository_DeleteAsync_ShouldDeleteStock()
        {
            //Arrange
            var id = 3;
            var stockRepository = new StockRepository(_context);

            //Act
            var result = await stockRepository.DeleteAsync(id);

            //Assert
            Assert.That(result, Is.Not.Null);

            var deletedStock = await _context.Stocks.FindAsync(id);
            Assert.That(deletedStock, Is.Null, $"Stock with ID {id} should not exist after deletion.");
        }
    }
}
