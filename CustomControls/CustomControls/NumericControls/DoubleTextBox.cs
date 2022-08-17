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

    protected override void UpdateFillerRect()
    {
        if (_fillerRect == null || _textBox == null)
        {
            return;
        }

        _fillerRect.Width = MathUtility.Lerp(0.0, _textBox.ActualWidth, MathUtility.InverseLerp(MinValue, MaxValue, Value));
    }
}