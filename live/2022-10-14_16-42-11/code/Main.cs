using System;
using System.Collections.Generic;
using System.Globalization;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Orders;
using QuantConnect.Securities;

namespace QuantConnect.Algorithm.CSharp
{
    public partial class IQFeedTest : QCAlgorithm
    {

        public override void Initialize()
        {           
            // Increase the max number of symbols
            Settings.DataSubscriptionLimit = 500;

            // Symbols
            var strSymbols = "AMD,NVDA,AMZN,META,GOOGL,GOOG,SHOP,OXY,RBLX,MU,XOM,BABA,GM,PLUG,SQ,ORCL,PYPL,DVN,CVX,COIN,JPM,SLB,AFRM,SBUX,LI,TSM,MRVL,DIS,UAL,MS,ROKU,NEE,U,APA,EQT,CVNA,PDD,QCOM,AA,RCL,COP,FDX,BA,LVS,PENN,CRM,UPST,ABNB,SCHW,GE,WMT,NKE,AMAT,AR,DOW,MOS,ON,RUN,DT,PG,MPC,MMM,DASH,RTX,ASAN,DOCU,ZIM,JD,WDC,ARRY,FTNT,BTU,ENVX,AIG,OKTA,GIS,BYND,TWLO,NET,TMUS,FISV,MET,MCHP,SE,LTHM,W,VLO,CVS,IBM,PM,TJX,DDOG,RRC,BX,WYNN,CTVA,ZM,CZR,TTD,Z,CHWY,GME,RIO,PSX,BHP,CTSH,MTCH,DD,EOG,PBF,ADI,OVV,APP,AXP,EXPE,CF,ADM,COF,YUMC,LYB,OKE,FSLR,AEM,BLDR,ETSY,CNQ,DLTR,PVH,NOVA,EMR,NUE,AEP,ICE,S,SPR,PGR,NTR,FANG,LAC,BBY,APH,MAR,STX,HLT,MP,BE,DUK,ANET,HES,MNST,TTE,CPRI,LYV,BIDU,SPLK,CHK,PRU,SWKS,STLD,CC,SYY,RNG";
            var symbols = new List<string>(strSymbols.Split(","));

            foreach (var symbol in symbols)
            {
                var security = AddEquity(symbol, Resolution.Tick, Market.USA, false, 1, true).Symbol;
                // Initialize consolidators
                var consolidator = new TickConsolidator(TimeSpan.FromMinutes(1));
                consolidator.DataConsolidated += OnDataMinute;
                SubscriptionManager.AddConsolidator(Symbol(symbol), consolidator);
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
    }
}
