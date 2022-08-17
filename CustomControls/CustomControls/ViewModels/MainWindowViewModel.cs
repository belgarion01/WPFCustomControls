using System.ComponentModel;
using System.Runtime.CompilerServices;
using CustomControls.Annotations;

namespace CustomControls.ViewModels;

public class MainWindowViewModel : ViewModel
{
    private int _myInteger;

    public int MyInteger
    {
        get => _myInteger;
        set
        {
            _myInteger = value;
            OnPropertyChanged();
        }
    }
    
    private double _myDouble;

    public double MyDouble
    {
        get => _myDouble;
        set
        {
            _myDouble = value;
            OnPropertyChanged();
        }
    }
}