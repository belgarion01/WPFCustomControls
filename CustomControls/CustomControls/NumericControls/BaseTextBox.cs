using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CustomControls.Annotations;
using CustomControls.Utilities;

namespace CustomControls.NumericControls;

public abstract class BaseTextBox<T> : Control, INotifyPropertyChanged
{
    public static DependencyProperty ValueProperty = DependencyProperty.Register(
    nameof(Value),
    typeof(T),
    typeof(BaseTextBox<T>),
    new FrameworkPropertyMetadata(OnValueChanged));

    public T Value
    {
        get => (T)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is BaseTextBox<T> self)
        {
            self.UpdateFillerRect();
        }
    }
    
    public static DependencyProperty MinValueProperty = DependencyProperty.Register(
        nameof(MinValue),
        typeof(T),
        typeof(BaseTextBox<T>),
        new FrameworkPropertyMetadata());
    public T MinValue
    {
        get => (T)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }
    
    public static DependencyProperty MaxValueProperty = DependencyProperty.Register(
        nameof(MaxValue),
        typeof(T),
        typeof(BaseTextBox<T>),
        new FrameworkPropertyMetadata());

    public T MaxValue
    {
        get => (T)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }
    
    public static DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(double),
        typeof(BaseTextBox<T>),
        new FrameworkPropertyMetadata(0.0));

    public double CornerRadius
    {
        get => (double)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    protected TextBox _textBox;
    protected Border _fillerRect;
    
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

    protected abstract void UpdateFillerRect();
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}