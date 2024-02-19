using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;


using System.Globalization;



namespace cAlgo
{

    /// <summary>
    /// <para><b>cTrader Guru | Extensios</b></para>
    /// <para>A group of generic extensions that make the developer's life easier</para>
    /// </summary>
    public static class Extensions
    {
    
        #region DateTime

        /// <param name="Culture">Localization of double value</param>
        /// <returns>double : Time representation in double format (example : 10:34:07 = 10,34)</returns>
        public static double ToDouble(this DateTime thisDateTime, string Culture = "en-EN")
        {

            string nowHour = (thisDateTime.Hour < 10) ? string.Format("0{0}", thisDateTime.Hour) : string.Format("{0}", thisDateTime.Hour);
            string nowMinute = (thisDateTime.Minute < 10) ? string.Format("0{0}", thisDateTime.Minute) : string.Format("{0}", thisDateTime.Minute);

            return string.Format("{0}.{1}", nowHour, nowMinute).ToDouble(Culture);

        }

        #endregion
    
    
        #region String

        /// <param name="Culture">Localization of double value</param>
        /// <returns>double : Time representation in double format (example : "10:34:07" = 10,34)</returns>
        public static double ToDouble(this string thisString, string Culture = "en-EN")
        {
            var culture = CultureInfo.GetCultureInfo(Culture);
            return double.Parse(thisString.Replace(',', '.').ToString(CultureInfo.InvariantCulture), culture);

        }

        #endregion

        #region Spread
        /// <param name="MaxSpread">Settings value for Maximum allowed spread</param>
        /// <returns>bool : if the spread is to large will return <b>true</b></returns>
        public static bool SpreadFilter(this Symbol thisSymbol, double MaxSpread) {
            var spread = thisSymbol.TickSize == 0.01 ? thisSymbol.Spread : thisSymbol.Spread * 10000;
            if(spread.CompareTo(MaxSpread) < 0) {
                return true;
            }
            return false;
        }


        #endregion


        #region RSI
        /// <param name="RsiUpper">Settings value for Maximum allowed spread</param>
        /// <param name="RsiLower">Settings value for Maximum allowed spread</param>
        /// <param name="useRsiFilter">Settings value for Maximum allowed spread</param>
        /// <returns>bool : if price is outsite the lower/upper then its allowed to trade returns <b>false</b></returns>
        public static bool RsiFilter(this RelativeStrengthIndex thisRsi, double RsiUpper, double RsiLower, bool useRsiFilter) {
            if(!useRsiFilter) {
                return false;
            }
            
            var rsi = thisRsi.Result.LastValue;
            if(rsi < RsiUpper && rsi > RsiLower) {
                return true;
            }
            return false;
        
        }

        #endregion


        #region EMA
        /// <param name="UseEmaFilter">Settings value for activating the EMA filter spread</param>
        /// <param name="symbol">Current symbol data</param>
        /// <param name="trType">Tradetype to be executed</param>
        /// <returns>bool : if price is outsite the lower/upper then its allowed to trade returns <b>false</b></returns>
        public static bool EmaFilter(this MovingAverage thisEma, bool UseEmaFilter, Symbol symbol, TradeType trType) {
            if(!UseEmaFilter) {
                return false;
            }    
            var ema = thisEma.Result.LastValue;
            
            if(ema > symbol.Ask & trType == TradeType.Sell) {
                return false;
            }
            else if(ema < symbol.Bid & trType == TradeType.Buy) {
                return false;
            }
            return true;
        }

        #endregion


        #region Positions
        
        /// <param name="label">Settings label value (filters open positions made only by this bot)</param>
        /// <returns>bool : if there is an open position from this bot returns <b>true</b></returns>
        public static bool Count(this Positions thisPos, string label) {
            var pos = thisPos.Find(label);
            if(pos != null) {
                return true;
            }
            return false;
        }

        #endregion

        #region Symbol
        
        /// <param name="AccountBalance">Account.balance value</param>
        /// <param name="RiskPercentage">% of capital to risk per trade</param>
        /// <param name="_StopLoss">SL difference from Stoploss & Entry</param>
        /// <returns>double : Lot size for Forex/Stocks</returns>
        public static double CalculateLotSize(this Symbol thisSymbol, double AccountBalance, double RiskPercentage, double _StopLoss) {
            var amount_to_risk_per_trade = AccountBalance * (RiskPercentage / 100);
            var  PipScale = thisSymbol.PipValue;
            var trade_volume   = amount_to_risk_per_trade / (_StopLoss * PipScale);
            var truncation_factor   = thisSymbol.LotSize * PipScale * 100;
            var trade_volume_truncated = ( (int) (trade_volume / truncation_factor)) * truncation_factor;
            
            return thisSymbol.TickSize == 0.01 ? thisSymbol.NormalizeVolumeInUnits(trade_volume) : thisSymbol.NormalizeVolumeInUnits(trade_volume_truncated); 
        }
        
        /// <param name="tradeSize">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="StopLossMultiplier">Value to multiply the stoploss with default in settings = 1</param>
        /// <returns>double : stoploss size</returns>
        public static double CalculateStopLoss(this Symbol thisSymbol, double tradeSize, double StopLossMultiplier) {
            return (tradeSize * (thisSymbol.TickSize / thisSymbol.PipSize * Math.Pow(10, thisSymbol.Digits))) * StopLossMultiplier;
        }
        
        /// <param name="tradeSize">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="StopLossMultiplier">Value to multiply the stoploss with default in settings = 1<</param>
        /// <param name="TakeProfit">TP 2 = 2 tp 1 sl</param>
        /// <returns>double : takeprofit size</returns>
        public static double CalcTakeProfit(this Symbol thisSymbol, double tradeSize, double StopLossMultiplier, double TakeProfit) {
            var atrInPips = tradeSize * (thisSymbol.TickSize / thisSymbol.PipSize * Math.Pow(10, thisSymbol.Digits));
            return (atrInPips * StopLossMultiplier) * TakeProfit;
        }

        #endregion


        #region Positions
        
        /// <param name="label">Settings label value (filters open positions made only by this bot)</param>
        /// <param name="UseBreakEven">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="symbol">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="_StopLoss">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="BreakEvenAt">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="AddToBreakEven">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        public static void MoveToBreakEven(this Positions thisPos, string label, bool UseBreakEven, Symbol symbol, double _StopLoss, double BreakEvenAt, double AddToBreakEven) {
            if(thisPos.Count == 0 | !UseBreakEven)
                return;
                
            var pos = thisPos.Find(label);

            if(pos.Pips >= (_StopLoss * BreakEvenAt))
            {
                double add = pos.TradeType == TradeType.Buy ? symbol.TickSize * AddToBreakEven : -symbol.TickSize * AddToBreakEven;
                pos.ModifyStopLossPrice(pos.EntryPrice + add);   //ModifyPosition(pos, pos.EntryPrice + add, pos.TakeProfit);
            }
        }
        
        /// <param name="UseTrailingStop">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="symbol">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="_StopLoss">SL difference from Stoploss,Entry Or Atr value or Any simular</param>
        /// <param name="label">Settings label value (filters open positions made only by this bot)</param>
        public static void Trailing(this Positions thisPos, bool UseTrailingStop, Symbol symbol, double _StopLoss, string label)
        {

            if (thisPos.Count == 0 | !UseTrailingStop)
            {
                return;
            }

            var position = thisPos.Find(label);
            double distance = position.TradeType == TradeType.Buy ? (symbol.Bid - position.EntryPrice) : (position.EntryPrice - symbol.Ask);
            double newStopLossPrice = 0;
            
            if(_StopLoss > 0 & distance > 0)
            {
                if(position.TradeType == TradeType.Buy)
                {
                    newStopLossPrice = Math.Round(symbol.Bid - _StopLoss * symbol.PipSize, symbol.Digits);
                }
                else
                {
                    newStopLossPrice = Math.Round(symbol.Ask + _StopLoss * symbol.PipSize, symbol.Digits);
                } 
            }
                

            if(newStopLossPrice != 0)
            {
                bool ShortOrLong = position.TradeType == TradeType.Buy ? newStopLossPrice > position.StopLoss : newStopLossPrice < position.StopLoss;
                
                if(position.StopLoss == null | ShortOrLong)
                {
                    position.ModifyStopLossPrice(newStopLossPrice);
                }
            }

        }

        #endregion
    }
    
}


namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class NewBotTemplateStocksForexv11 : Robot
    {
        #region Settings

        #region Trade Settings
        [Parameter("Position Label", DefaultValue = "BOT", Group = "Trade Settings")]
        public string PosLabel { get; set; }
        
        [Parameter("Use Exit Strategy 1 : Close Trade above the EMA and open up another when closing", DefaultValue = true, Group = "Strategy Settings")]
        public bool ExiStrategyOne { get; set; }
        
        [Parameter("Exit Strategy 1: Fixed Lot size", DefaultValue = 1, MinValue = 0.1, Step = 0.1, Group = "Strategy Settings")]
        public double EsoLotSize { get; set; }
        
        [Parameter("Use Exit Strategy 2 : ATR TP/SL from settings", DefaultValue = false, Group = "Strategy Settings")]
        public bool ExiStrategyTwo { get; set; }
        public bool UseExiStrategyTwo
        { 
            get
            {
                if(ExiStrategyOne && ExiStrategyTwo)
                {
                    Print("WARING: CANNOT HAVE BOTH EXIT STRATEGIES ON : STRATEGY 2 DISABLED");
                     return false;
                }
                
                return true;
            }
        }
        
        [Parameter("SMA Periods", DefaultValue = 50, MinValue = 5, Step = 1, Group = "Strategy Settings")]
        public int SmaPeriods { get; set; }


        #endregion

        #region Risk Settings
        [Parameter("Trade size % of account", DefaultValue = 1, MinValue = 0.1, Step = 0.1, Group = "Risk Settings")]
        public double Risk_Percentage { get; set; }
        
        [Parameter("TakeProfit (Multiplied with ATR)", DefaultValue = 2, MinValue = 1, Step = 1, Group = "Risk Settings")]
        public double TakeProfit { get; set; }
        
        [Parameter("ATR multiplier for SL/TP", DefaultValue = 2, MinValue = 1, Step = 1, Group = "Risk Settings")]
        public int AtrMultiplier { get; set; }
        
        [Parameter("Allow Long trades", DefaultValue = true, Group = "Risk Settings")]
        public bool AllowLong { get; set; }
        public bool CanTradeLong
        { 
            get
            {
                if(!AllowLong && _TrType == TradeType.Buy) return true;
                return false;
            }
        }
        
        
        [Parameter("Allow Short trades", DefaultValue = true, Group = "Risk Settings")]
        public bool AllowShort { get; set; }
        public bool CanTradeShort
        { 
            get
            {
                if(!AllowShort && _TrType == TradeType.Sell) return true;
                return false;
            }
        }
        
        [Parameter("Max spread (No trades if spread above)", DefaultValue = 0.5, MinValue = 0.1, Step = 0.1, Group = "Risk Settings")]
        public double MaxSpread { get; set; }
        #endregion
        
        #region Pausa

        [Parameter("From (18.0 = 18:00)", Group = "Pause", DefaultValue = 0, MinValue = 0, MaxValue = 23.59, Step = 0.01)]
        public double PauseFrom { get; set; }

        [Parameter("To (8.20 = 08:20)", Group = "Pause", DefaultValue = 0, MinValue = 0, MaxValue = 23.59, Step = 0.01)]
        public double PauseTo { get; set; }

        public bool IAmInPause
        {

            get
            {

                if (PauseFrom == 0 && PauseTo == 0)
                    return false;

                double now = Server.Time.ToDouble();

                bool intraday = (PauseFrom < PauseTo && now >= PauseFrom && now <= PauseTo);
                bool overnight = (PauseFrom > PauseTo && ((now >= PauseFrom && now <= 23.59) || now <= PauseTo));

                return intraday || overnight;

            }
        }


        #endregion
        
        #region Trailing Stop
        [Parameter("Use trailing stop", DefaultValue = false, Group = "Trailingstop Settings")]
        public bool UseTrailingStop { get; set; }
        #endregion

        #region Breakeven
        [Parameter("Use breakeven", DefaultValue = false, Group = "Breakeven Settings")]
        public bool BreakEven { get; set; }
        
        [Parameter("Breakeven at Risk unit (1:1 default)", DefaultValue = 1, MinValue = 0.5, Step = 0.1, Group = "Breakeven Settings")]
        public double BreakEvenAt { get; set; }
        
        [Parameter("Add Mini Pips/cents to breakeven", DefaultValue = 5, MinValue = 0, Step = 1, Group = "Breakeven Settings")]
        public double AddToBreakEven { get; set; }
        #endregion

        #region ATR settings

        [Parameter("MA type", DefaultValue = MovingAverageType.Simple, Group = "ATR Settings")]
        public MovingAverageType ATR_MA_TYPE { get; set; }
        
        [Parameter("Periods", DefaultValue = 14, MinValue = 1, Step = 1, Group = "ATR Settings")]
        public int ATR_PERIODS { get; set; }
        #endregion
        
        #region EMA Settings
        [Parameter("Activate EMA filter", DefaultValue = false, Group = "EMA Settings")]
        public bool UseEmaFilter { get; set; }
        
        [Parameter("Periods", DefaultValue = 200, MinValue = 5, Step = 1, Group = "EMA Settings")]
        public int EmaPeriods { get; set; }
        
        [Parameter("Type", Group = "EMA Settings")]
        public MovingAverageType EmaType { get; set; }
        
        [Parameter("Timeframe", Group = "EMA Settings")]
        public TimeFrame EmaTimeFrame { get; set; }
        
        [Parameter("Draw EMA", DefaultValue = false, Group = "EMA Settings")]
        public bool UseDrawEma { get; set; }
        
        [Parameter("EMA Color", Group = "EMA Settings")]
        public Color EmaColor { get; set; }
        
        [Parameter("Line thickness", DefaultValue = 3, MinValue = 1, MaxValue = 5, Step = 1, Group = "EMA Settings")]
        public int EmaThickness { get; set; }
        #endregion

        #region Rsi
        
        [Parameter("Activate EMA filter", DefaultValue = false, Group = "RSI Settings")]
        public bool UseRsiFilter { get; set; }
        
        [Parameter("Periods", DefaultValue = 14, MinValue = 2, Step = 1, Group = "RSI Settings")]
        public int RsiPeriods { get; set; }
        
        [Parameter("Source", Group = "RSI Settings")]
        public DataSeries RsiSource { get; set; }
        
        [Parameter("(Upper) No trades between Upper/lower (Consolidation filter)", DefaultValue = 60, MinValue = 2, MaxValue = 100, Step = 1, Group = "RSI Settings")]
        public double RsiUpper { get; set; }

        [Parameter("(Lower) No trades between Upper/lower (Consolidation filter)", DefaultValue = 40, MinValue = 2, MaxValue = 100, Step = 1, Group = "RSI Settings")]
        public double RsiLower { get; set; }
        #endregion

        #endregion

        #region Built in Functions
        protected override void OnStart()
        {
            CalculateATR();
            CalculateRsi();
            CalculateEMA();
        }

        protected override void OnTick()
        {
            // Risk management
            var pos = Positions.Find(PosLabel);
            if(pos != null) {
                Positions.MoveToBreakEven(PosLabel, BreakEven, Symbol, _StopLoss, BreakEvenAt, AddToBreakEven); 
                Positions.Trailing(UseTrailingStop, Symbol, _StopLoss, PosLabel);
            }
        }

        protected override void OnStop()
        {
            // Handle cBot stop here
        }
        
        
        protected override void OnBar() {
            CalculateEMA();
            CalculateRsi();
            DrawOnChart();
            CalculateATR();
            
            
            // Strategy Functions
            CalulateSma50();
            Strategy();
            
            // Strategy Functions - END
            if(ExiStrategyOne) 
            {
                StrategyExit();
            }
            
            Strategy();
            
            //ExecuteTrade();
        }
        #endregion

        #region Variables
        double _StopLoss;
        public AverageTrueRange _ATR;
        public MovingAverage _MAFilter;
        public RelativeStrengthIndex _RSIFilter;
        public TradeType _TrType;
        #endregion


        #region Strategy
        public SimpleMovingAverage _SMA50;
        public String _TradeDirection;
        
        public void CalulateSma50()
        {
            _SMA50 = Indicators.SimpleMovingAverage(Bars.ClosePrices, SmaPeriods); 
        }
        
        public string PriceBarRange()
        {
            int i = Bars.Count - 2;
            double open = Bars.OpenPrices[i];
            double close = Bars.ClosePrices[i];
            double high = Bars.HighPrices[i];
            double low = Bars.LowPrices[i];
            double barSize = high - low;
            double percent20 = barSize * 0.2;
            // red bar
            if(open > close && (low + percent20) >= close) 
            {
                Print("LAST BAR IS OK FOR LONG");
                return "LONG";
            }
            else if (open < close && (high - percent20) <= close)
            {
                Print("LAST BAR IS OK FOR SHORT");
                return "SHORT";
            }
        
            Print("BAR NOT GOOD");
            return "NONE";
        }
        
        public String SmaPricePosition()
        {
            int i = Bars.Count - 2;
            if(_SMA50.Result.LastValue > Bars.ClosePrices[i])
            {
                return "LONG";
            }
            else if(_SMA50.Result.LastValue < Bars.ClosePrices[i])
            {
                return "SHORT";
            }
            
            
            return "NONE";
        }
        
        public bool IsOpenPos()
        {
            Position pos = Positions.Find(PosLabel);
            if(pos != null) 
            {
                return false;
            }
            return true;
        }
        
        public void Strategy()
        {
            string priceDir = PriceBarRange();
            string priceSmaPos = SmaPricePosition();
            bool isOpenPos = IsOpenPos();
            
            if(priceDir == "LONG" && priceSmaPos == "LONG" && isOpenPos)
            {
                _TrType = TradeType.Buy;
                ExecuteTrade();
            }
            if(priceDir == "SHORT" && priceSmaPos == "SHORT" && isOpenPos)
            {
                _TrType = TradeType.Sell;
                ExecuteTrade();
            }
        }
        
        public void StrategyExit()
        {
            Position pos = Positions.Find(PosLabel);
            if(pos != null) 
            {
                string priceSmaPos = SmaPricePosition();
                if(priceSmaPos == "SHORT" && pos.TradeType == TradeType.Buy)
                {
                    pos.Close();
                }
                else if(priceSmaPos == "LONG" && pos.TradeType == TradeType.Sell)
                {
                    pos.Close();
                }
            }
        }
        


        #endregion


        /// <summary>
        /// Calculates StopLoss, TakeProfit, Position Size &
        /// executes Marketorder
        // </summary>
        /// <param name="trtype">Type of Trade Sell/Buy</param>
        #region Trade 
        public void ExecuteTrade() {
            if(RunFilter(_TrType)) {
                return;
            }
            
            
            _StopLoss = Symbol.CalculateStopLoss(_ATR.Result.Last(1), AtrMultiplier);
            double TP = Symbol.CalcTakeProfit(_ATR.Result.Last(1), AtrMultiplier, TakeProfit);
            double SIZE = Symbol.CalculateLotSize(Account.Balance, Risk_Percentage, _StopLoss);
            
            if(ExiStrategyOne) 
            {
                ExecuteMarketOrder(_TrType, Symbol.Name, Symbol.NormalizeVolumeInUnits(EsoLotSize * 100000), PosLabel);
            }
            else if(UseExiStrategyTwo)
            {
                var res = ExecuteMarketOrder(_TrType, Symbol.Name, SIZE, PosLabel, _StopLoss, TP);
                if(res.IsSuccessful) {
                    if(res.Position.StopLoss == null)
                    {
                        ModifyPosition(res.Position, _StopLoss, TP);
                    }
                    if(res.Position.StopLoss == null)
                    {
                        res.Position.Close();
                    }
                }
                else {
                    Print("Error in executing position: ", res.Error);
                }
            }
            
            
        }


        #endregion


        
        #region Helper - Random Number generator
        /// <summary>
        /// Returns random number, used for generating unique ID
        // </summary>
        private int RandomNum()
        {
            return new Random().Next(0, 1000000);
        }
        #endregion


        #region Filters,Drawing & Indicators functions
        
        /// <summary>
        /// Evaluates filters, like position count, EMA, RSI and so on.
        // </summary>
        /// <param name="trtype">Type of Trade Sell/Buy</param>
        public bool RunFilter(TradeType trType) {
            if(
                IAmInPause || Positions.Count(PosLabel) || 
                _MAFilter.EmaFilter(UseEmaFilter, Symbol, trType) || 
                _RSIFilter.RsiFilter(RsiUpper, RsiLower, UseRsiFilter) || 
                Symbol.SpreadFilter(MaxSpread) ||
                CanTradeLong ||
                CanTradeShort
                )
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Draws the Moving average to the chart
        // </summary>
        public void DrawOnChart() {
            if(UseDrawEma && UseEmaFilter) {
                Chart.DrawTrendLine(RandomNum().ToString(), Bars.OpenTimes[Bars.Count - 3], _MAFilter.Result.Last(2), Bars.OpenTimes[Bars.Count - 1], _MAFilter.Result.LastValue, EmaColor, EmaThickness);
            }
        }
        
        /// <summary>
        /// Calculates the Moving average
        // </summary>
        public void CalculateEMA() {
            if(UseEmaFilter) {
                _MAFilter = Indicators.MovingAverage(Bars.ClosePrices, EmaPeriods, EmaType); 
            }
        }
        
        /// <summary>
        /// Calculates the RSI
        // </summary>
        public void CalculateRsi() {
            if(UseRsiFilter) {
                _RSIFilter = Indicators.RelativeStrengthIndex(RsiSource, RsiPeriods);
            }
        }

        /// <summary>
        /// Calculates the ATR based on bot settings
        // </summary>
        public void CalculateATR() {
            _ATR = Indicators.AverageTrueRange(ATR_PERIODS, ATR_MA_TYPE);
        }
        #endregion


    }
}