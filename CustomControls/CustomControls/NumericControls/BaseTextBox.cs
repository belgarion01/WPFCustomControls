using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CustomControls.Annotations;
using CustomControls.Utilities;
using System.Numerics;

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
        set
        {
            if (IsClampingActive)
            {
                SetValue(ValueProperty, ClampValue(value, MinValue, MaxValue));
            }
            else
            {

                SetValue(ValueProperty, value);
            }
        }
    }

    private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is BaseTextBox<T> self)
        {
            self.UpdateFillerRect();
            self.RefreshTextBox();
        }
    }
    
    public static DependencyProperty MinValueProperty = DependencyProperty.Register(
        nameof(MinValue),
        typeof(T),
        typeof(BaseTextBox<T>),
        new FrameworkPropertyMetadata(OnMinValueChanged));

    public T MinValue
    {
        get => (T)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }
    private static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BaseTextBox<T> self)
        {
            self.OnPropertyChanged(nameof(IsClampingActive));
            self.UpdateFillerRect();
        }
    }

    public static DependencyProperty MaxValueProperty = DependencyProperty.Register(
        nameof(MaxValue),
        typeof(T),
        typeof(BaseTextBox<T>),
        new FrameworkPropertyMetadata(OnMaxValueChanged));

    private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BaseTextBox<T> self)
        {
            self.OnPropertyChanged(nameof(IsClampingActive));
            self.UpdateFillerRect();
        }
    }

    public T MaxValue
    {
        get => (T)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    protected TextBox _textBox;
    protected Border _fillerRect;

    public BaseTextBox()
    {
        AddHandler(PreviewMouseMoveEvent, new MouseEventHandler(InternalPreviewMouseMove));
        AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(InternalPreviewMouseLeftButtonDown));
        AddHandler(PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(InternalPreviewMouseLeftButtonUp));
    }

    private bool _isManuallyEditing;

    private bool _mouseDown = false;
    private bool _isDragging = false;
    private Point _lastDragPosition;
    private const double DRAG_THRESHOLD = 5.0;

    private void InternalPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _mouseDown = true;
        _lastDragPosition = e.GetPosition(this);

        if (!_textBox.IsFocused)
        {
            CaptureMouse();
            e.Handled = true;
        }
    }

    private void InternalPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        ReleaseMouseCapture();

        if (_mouseDown && !_isDragging && !_textBox.IsFocused)
        {
            _textBox.Focus();
            TextBoxSelectAllAsync();
        }

        _mouseDown = false;

        if (_isDragging)
        {
            EndDrag();
        }
    }

    private void InternalPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_mouseDown)
        {
            var position = e.GetPosition(this);
            var delta = position - _lastDragPosition;
            delta.Y = 0;

            if(delta.Length > DRAG_THRESHOLD && !_isDragging)
            {
                StartDrag();
                e.Handled= true;
                return;
            }

            if (_isDragging)
            {
                double direction = delta.X < 0 ? -1 : 1;

                AddToValue(ComputeDragOperationDelta(delta.Length, direction));

                _lastDragPosition = position;

                // TODO: Stop mouse from moving + hid it with Cursors.None
                Mouse.OverrideCursor = Cursors.None;

                e.Handled = true;
            }
        }
    }

    private void StartDrag()
    {
        _isDragging = true;
        Mouse.OverrideCursor = Cursors.SizeWE;
        Keyboard.ClearFocus();
    }

    private void EndDrag()
    {
        _isDragging = false;
        Mouse.OverrideCursor = null;
    }

    protected override void OnIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnIsKeyboardFocusedChanged(e);
        if ((bool)e.NewValue)
        {
            _textBox.Focus();
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        
        if (GetTemplateChild("PART_TextBox") is TextBox textBox)
        {
            _textBox = textBox;

            _textBox.GotFocus += TextBox_GotFocus;
            _textBox.LostFocus += TextBox_LostFocus;
            _textBox.PreviewKeyDown += TextBox_PreviewKeyDown;

            _textBox.Cursor = Cursors.SizeWE;
        }
        
        if (GetTemplateChild("PART_FillerRect") is Border fillerRect)
        {
            _fillerRect = fillerRect;
            UpdateFillerRect();
        }

        RefreshTextBox();
    }

    protected virtual void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        RefreshTextBox();

        TextBoxSelectAllAsync();
    }

    protected virtual void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        ValidateText();
    }

    protected virtual void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                ValidateText();
                _textBox.SelectAll();
                e.Handled = true;
                break;
            case Key.Escape:
                if(_textBox.SelectionLength == _textBox.Text.Length)
                {
                    RefreshTextBox();
                    _textBox.Select(_textBox.Text.Length, 0);
                    e.Handled = true;
                }
                break;
        }
    }

    private void ValidateText()
    {
        string text = _textBox.Text;
        if(TryConvertFromString(text, out T value))
        {
            Value = value;
        }

        RefreshTextBox();
    }

    private void RefreshTextBox()
    {
        if(_textBox == null)
        {
            return;
        }

        _textBox.Text = Value.ToString();
    }

    protected abstract void AddToValue(T valueToAdd);
    protected abstract T ComputeDragOperationDelta(double delta, double direction);
    protected abstract T ClampValue(T value, T min, T max);
    protected abstract double ConvertToDouble(T value);
    protected abstract bool TryConvertFromString(string text, out T value);
    public abstract bool IsClampingActive { get; }

    protected virtual void UpdateFillerRect()
    {
        if (_fillerRect == null || _textBox == null || !IsClampingActive)
        {
            return;
        }

        double value = ConvertToDouble(Value);
        double minValue = ConvertToDouble(MinValue);
        double maxValue = ConvertToDouble(MaxValue);

        _fillerRect.Width = MathUtility.Lerp(0.0, _textBox.ActualWidth, MathUtility.InverseLerp(minValue, maxValue, value));
    }

    private async void TextBoxSelectAllAsync()
    {
        // Async is needed because the TextBox is busy with the Focus event so he can't handle SelectAll. We have to wait before SelectAll.
        // https://stackoverflow.com/questions/59558368/how-to-select-all-text-of-a-textbox-on-mouse-click-textbox-selectall-not-wor
        await Application.Current.Dispatcher.InvokeAsync(_textBox.SelectAll);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}