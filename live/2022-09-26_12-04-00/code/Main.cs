using System;
using System.Collections.Generic;
using System.Globalization;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Orders;
using QuantConnect.Securities;


namespace QuantConnect.Algorithm.CSharp
{
    public partial class IQFeed : QCAlgorithm
    {
        private readonly CultureInfo cultureInfo = new("en-US");


        // Internals
        private const decimal tickSize = 0.01m;
        
        public override void Initialize()
        {           
            // Increase the max number of symbols
            Settings.DataSubscriptionLimit = 500;
            // Disable Margin Call Model
            Portfolio.MarginCallModel = MarginCallModel.Null;

            // Symbols
            var symbols = new List<string>("AAPL,ABNB".Split(","));

            foreach (var symbol in symbols)
            {
                var security = AddEquity(symbol, Resolution.Tick, Market.USA, false, 1, true);
                // Initialize consolidators
                var consolidator = new TickConsolidator(TimeSpan.FromMinutes(1));
                consolidator.DataConsolidated += OnDataMinute;
                SubscriptionManager.AddConsolidator(symbol, consolidator);
                // Set PatterDayTradingModel
                Securities[symbol].SetBuyingPowerModel(new PatternDayTradingMarginModel());
            }
        }

        private static string BarToString(TradeBar bar)
        {
            return $"{bar.Symbol} - {bar.Time.Hour}:{bar.Time.Minute:D2},{bar.Open},{bar.High},{bar.Low},{bar.Close},{bar.Volume}";
        }

        private void OnDataMinute(object sender, TradeBar bar)
        {
            var symbol = bar.Symbol.Value;
            // Do not process tick outside of opening hours
            if (bar.Time.TimeOfDay < new TimeSpan(9, 30, 00) || new TimeSpan(16, 00, 00) <= bar.Time.TimeOfDay) return;
            // Bar
            Log($" - BAR - {BarToString(bar)}");
        }

        public void OnData(Ticks data)
        {
            foreach (var Symbol in data.Keys)
            {
                var symbol = Symbol.Value;
            }
        }

        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            var symbol = orderEvent.Symbol.Value;
        }
    }
}
