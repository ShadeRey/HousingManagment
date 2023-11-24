using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class ViewModelBase : INotifyPropertyChanged, INotifyPropertyChanging
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value)) {
            return false;
        }
        RaisePropertyChanging(propertyName);
        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    protected void RaisePropertyChanging([CallerMemberName] string? propertyName = null) {
        PropertyChanging?.Invoke(this, new(propertyName));
    }
}