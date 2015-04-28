﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
using QuantConnect.Data.Market;

namespace QuantConnect.Indicators
{
    /// <summary>
    /// This indicator computes the On Balance Volume (OBV). 
    /// The On Balance Volume is calculated by determining the price of the current close price and previous close price.
    /// If the current close price is equivalent to the previous price the OBV remains the same,
    /// If the current close price is higher the volume of that day is added to the OBV, while a lower close price will
    /// result in negative value.
    /// </summary>
    public class OnBalanceVolume : TradeBarIndicator
    {
        private TradeBar _previousInput;

        /// <summary>
        /// Gets the value of the On Balance Volume for a series of trade periods. 
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> OnBalanceVoulumeValues { get; set; }

        /// <summary>
        /// Initializes a new instance of the Indicator class using the specified name.
        /// </summary> 
        /// <param name="name">The name of this indicator</param>
        public OnBalanceVolume(string name)
            : base(name)
        {
            OnBalanceVoulumeValues = new Identity("OBV");
        }

        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady
        {
            //get { return OnBalanceVoulumeValues.IsReady && OnBalanceVoulumeValues.Current.Value != 0; }
            get { return OnBalanceVoulumeValues.IsReady; }
        }

        /// <summary>
        /// Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="input">The input given to the indicator</param>
        /// <returns> A new value for this indicator </returns>
        protected override decimal ComputeNextValue(TradeBar input)
        {
            decimal obv = 0;

            if (_previousInput != null && input.Value == _previousInput.Value)
            {
                obv = OnBalanceVoulumeValues.Current.Value;
                OnBalanceVoulumeValues.Update(new IndicatorDataPoint(input.Time, obv));

                _previousInput = input;
                return obv;
            }

            if (_previousInput != null && input.Value > _previousInput.Value)
            {
                if (OnBalanceVoulumeValues.Current.Value != 0)
                {
                    obv = input.Volume + OnBalanceVoulumeValues.Current.Value;
                    OnBalanceVoulumeValues.Update(new IndicatorDataPoint(input.Time, obv));

                    _previousInput = input;
                    return obv;
                }
            }

            if (_previousInput != null && input.Value < _previousInput.Value)
            {
                if (OnBalanceVoulumeValues.Current.Value != 0)
                { 
                    obv = OnBalanceVoulumeValues.Current.Value - input.Volume;
                    OnBalanceVoulumeValues.Update(new IndicatorDataPoint(input.Time, obv));

                    _previousInput = input;
                    return obv;
                }
            }

            _previousInput = input;
            return obv;
        }

        /// <summary>
        /// Resets this indicator to its initial state
        /// </summary>
        public override void Reset()
        {
            OnBalanceVoulumeValues.Reset();
        }
    }
}
