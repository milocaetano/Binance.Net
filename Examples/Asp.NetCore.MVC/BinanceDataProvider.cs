using System;
using System.Threading.Tasks;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Interfaces.Clients;
using CryptoExchange.Net.Sockets;

namespace Asp.Net
{
    public interface IBinanceDataProvider
    {
        IBinanceStreamKlineData LastKline { get; }
        Action<IBinanceStreamKlineData> OnKlineData { get; set; }

        Task<decimal> GetPrice(string coiname);
        Task Start();
        Task Stop();
    }

    public class BinanceDataProvider: IBinanceDataProvider
    {
        private IBinanceSocketClient _socketClient;
        private UpdateSubscription _subscription;

        public IBinanceStreamKlineData LastKline { get; private set; }
        public Action<IBinanceStreamKlineData> OnKlineData { get; set; }
       
        public BinanceDataProvider(IBinanceSocketClient socketClient)
        {
            _socketClient = socketClient;

            Start().Wait(); // Probably want to do this in some initialization step at application startup
        }

        public async Task Start()
        {
            var subResult = await _socketClient.SpotStreams.SubscribeToKlineUpdatesAsync("BTCUSDT", KlineInterval.FifteenMinutes, data =>
            {
                LastKline = data.Data;
                OnKlineData?.Invoke(data.Data);
            });
            if (subResult.Success)            
                _subscription = subResult.Data;            
        }

        public async Task<decimal> GetPrice(string coiname)
        {
            var subResult = await _socketClient.SpotStreams.SubscribeToKlineUpdatesAsync(coiname, KlineInterval.FifteenMinutes, data =>
            {
                LastKline = data.Data;
                OnKlineData?.Invoke(data.Data);
            });
            if (subResult.Success)
                _subscription = subResult.Data;

            return LastKline.Data.ClosePrice;
        }


        public async Task Stop()
        {
            await _socketClient.UnsubscribeAsync(_subscription);
        }
    }
}
