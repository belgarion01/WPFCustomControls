using CustomControls.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CustomControls.NumericControls
{
    internal class IntTextBox : BaseTextBox<int>
    {
        static IntTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntTextBox), new FrameworkPropertyMetadata(typeof(IntTextBox)));
        }

        protected override int ComputeDragOperationDelta(double delta, double direction)
        {
            double modifier;

            if (IsClampingActive)
            {
                modifier = (MaxValue - MinValue) / 1000.0;
            }
            else
            {
                modifier = Math.Abs(Value == 0 ? 1 : Value) / 200.0;

                if (modifier > 0)
                {
                    modifier = Math.Max(0.01, modifier);
                }
            }

            return (int)Math.Round(delta * modifier * direction);
        }

        protected override double ConvertToDouble(int value) => Convert.ToDouble(value);

        public override bool IsClampingActive => MinValue < MaxValue;

        protected override bool TryConvertFromString(string text, out int value)
        {
            if(int.TryParse(text, out value))
            {
                return true;
            }

            return false;
        }

        protected override int ClampValue(int value, int min, int max) => Math.Clamp(value, MinValue, MaxValue);

        protected override void AddToValue(int valueToAdd) => Value += valueToAdd;
    }
}
