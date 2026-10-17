using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Vesti.ViewModels;

/// <summary>
/// Base commune à tous les ViewModels.
///
/// POURQUOI INotifyPropertyChanged ?
/// Le binding XAML écoute cet événement pour savoir quand rafraîchir
/// l'interface. Sans lui, modifier une propriété n'aurait aucun effet visuel.
///
/// SetProperty() compare l'ancienne et la nouvelle valeur avant de notifier,
/// ce qui évite des mises à jour inutiles de l'interface.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    // ── État de chargement ───────────────────────────────────────────────
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
}
