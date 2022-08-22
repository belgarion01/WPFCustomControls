using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Shapes;
using CustomControls.Annotations;
using CustomControls.Utilities;

namespace CustomControls.NumericControls;

[TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
[TemplatePart(Name = "PART_FillerRect", Type = typeof(Rectangle))]
public class IntTextBoxOld : Control, INotifyPropertyChanged
{
    public static DependencyProperty IntValueProperty = DependencyProperty.Register(
        nameof(IntValue),
        typeof(int),
        typeof(IntTextBoxOld),
        new FrameworkPropertyMetadata(0, OnIntValueChanged));

    public int IntValue
    {
        get => (int)GetValue(IntValueProperty);
        set => SetValue(IntValueProperty, value);
    }
    
    private static void OnIntValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is IntTextBoxOld self)
        {
            self.UpdateFillerRect();
        }
    }
    
    public static DependencyProperty MinValueProperty = DependencyProperty.Register(
        nameof(MinValue),
        typeof(int),
        typeof(IntTextBoxOld),
        new FrameworkPropertyMetadata(0));
    public int MinValue
    {
        get => (int)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }
    
    public static DependencyProperty MaxValueProperty = DependencyProperty.Register(
        nameof(MaxValue),
        typeof(int),
        typeof(IntTextBoxOld),
        new FrameworkPropertyMetadata(10));

    public int MaxValue
    {
        get => (int)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }
    
    public static DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(double),
        typeof(IntTextBoxOld),
        new FrameworkPropertyMetadata(0.0));

    public double CornerRadius
    {
        get => (double)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    static IntTextBoxOld()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IntTextBoxOld), new FrameworkPropertyMetadata(typeof(IntTextBoxOld)));
    }

    private TextBox _textBox;
    private Border _fillerRect;
    
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        
        if (GetTemplateChild("PART_TextBox") is TextBox textBox)
        {
            _textBox = textBox;
        }
        
        if (GetTemplateChild("PART_FillerRect") is Border fillerRect)
        {
            _fillerRect = fillerRect;
            UpdateFillerRect();
        }
    }

    private void UpdateFillerRect()
    {
        if (_fillerRect == null || _textBox == null)
        {
            return;
        }

        _fillerRect.Width = MathUtility.Lerp(0.0, _textBox.ActualWidth, MathUtility.InverseLerp(MinValue, MaxValue, IntValue));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}