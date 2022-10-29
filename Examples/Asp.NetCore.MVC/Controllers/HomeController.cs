using System.Diagnostics;
using System.Threading.Tasks;
using Asp.Net.Models;
using Binance.Net.Interfaces;
using Binance.Net.Interfaces.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asp.Net.Controllers
{

    public class Coin
    {
        public decimal Price { get; set; }
        public string Name { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBinanceClient _binanceClient;
        private readonly IBinanceDataProvider _dataProvider;

        public HomeController(ILogger<HomeController> logger, IBinanceClient client, IBinanceDataProvider dataProvider)
        {
            _logger = logger;
            _binanceClient = client;
            _dataProvider = dataProvider;
        }

        public async Task<IActionResult> Index(string coiname)
        {
            if (string.IsNullOrEmpty(coiname))
            {
                coiname = "BTCUSDT";
            }

            decimal price = await this._dataProvider.GetPrice(coiname);
            var coin = new Coin();
            coin.Name = coiname;
            coin.Price = price;

            return View(coin);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
