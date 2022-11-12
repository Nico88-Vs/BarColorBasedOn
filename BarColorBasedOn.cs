// Copyright QUANTOWER LLC. © 2017-2021. All rights reserved.

using System;
using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace BarColorBasedOn
{
    /// <summary>
    /// An example of blank indicator. Add your code, compile it and use on the charts in the assigned trading terminal.
    /// Information about API you can find here: http://api.quantower.com
    /// Code samples: https://github.com/Quantower/Examples
    /// </summary>
	public class BarColorBasedOn : Indicator
    {
        private Indicator ema;

        #region user imput

        [InputParameter("Period of MA for envelopes", 0, 1, 999)]
        public int Period = 20;

        // Price type of moving average.
        [InputParameter("Sources prices for MA", 20, variants: new object[]
        {
            "Close", PriceType.Close,
            "Open", PriceType.Open,
            "High", PriceType.High,
            "Low", PriceType.Low,
            "Typical", PriceType.Typical,
            "Median", PriceType.Median,
            "Weighted", PriceType.Weighted,
            "Volume", PriceType.Volume,
            "Open interest", PriceType.OpenInterest
        })]
        public PriceType SourcePrice = PriceType.Close;

        //
        [InputParameter("Calculation type", 30, variants: new object[]
        {
            "All available data", IndicatorCalculationType.AllAvailableData,
            "By period", IndicatorCalculationType.ByPeriod,
        })]
        public IndicatorCalculationType CalculationType = Indicator.DEFAULT_CALCULATION_TYPE;

        public int MinHistoryDepths => this.CalculationType switch
        {
            IndicatorCalculationType.AllAvailableData => this.Period,
            _ => this.Period * 2
        };

        #endregion user imput

        public override string ShortName => $"EMA >> BarColor ({this.Period}: {this.SourcePrice})";

        public BarColorBasedOn()
            : base()
        {
            // Defines indicator's name and description.
            Name = "BarColorBasedOn";
            Description = "Set bar color depending on ema position";

            AddLineSeries("Ema", Color.White, 1, LineStyle.Solid);
        }

        protected override void OnInit()
        {
            ema = Core.Indicators.BuiltIn.EMA(this.Period, this.SourcePrice, this.CalculationType);
            AddIndicator(ema);
        }

        protected override void OnUpdate(UpdateArgs args)
        {
            if (this.HistoricalData.Count > this.MinHistoryDepths)
            {
                SetValue(ema.GetValue());

                if (Close(0) > ema.GetValue())
                {
                    SetBarColor(Color.CadetBlue);
                }
                else if (Close(0) < ema.GetValue())
                {
                    SetBarColor(Color.DeepPink);
                }
            }
        }
    }
}