using System.Globalization;

namespace Vesti;

/// <summary>
/// Inverse un booléen.
/// Utilisation XAML : IsEnabled="{Binding IsBusy, Converter={StaticResource InvertBoolConverter}}"
/// </summary>
public class InvertBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}

/// <summary>
/// Retourne vrai si la valeur est non nulle et non vide.
/// Utilisation : IsVisible="{Binding ErrorMessage, Converter={StaticResource NotNullConverter}}"
/// </summary>
public class NotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string s ? !string.IsNullOrEmpty(s) : value is not null;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retourne vrai si l'entier vaut 0.</summary>
public class EqualToZeroConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i && i == 0;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retourne vrai si l'entier vaut 1.</summary>
public class EqualToOneConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i && i == 1;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Retourne vrai si l'entier vaut 2.</summary>
public class EqualToTwoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i && i == 2;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
