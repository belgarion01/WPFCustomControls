using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CustomControls.Utilities;

namespace CustomControls.NumericControls;

public class DoubleTextBox : BaseTextBox<double>
{
    static DoubleTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleTextBox), new FrameworkPropertyMetadata(typeof(DoubleTextBox)));
    }

    protected override double ComputeDragOperationDelta(double delta, double direction)
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

        return delta * modifier * direction;
    }

    protected override double ConvertToDouble(double value) => value;

    public override bool IsClampingActive => MinValue < MaxValue;

    protected override bool TryConvertFromString(string text, out double value)
    {
        string formatedText = text.Replace(".", ",");

        if (double.TryParse(formatedText, out value))
        {
            return true;
        }

        return false;
    }

    protected override double ClampValue(double value, double min, double max) => Math.Clamp(value, MinValue, MaxValue);

    protected override void AddToValue(double valueToAdd) => Value += valueToAdd;
}