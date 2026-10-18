using System.Globalization;

namespace Vesti;

/// <summary>Inverse un booléen.</summary>
public class InvertBoolConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is bool b && !b;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => value is bool b && !b;
}

/// <summary>Vrai si la valeur est une chaîne non vide, ou un objet non null.</summary>
public class NotNullConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is string s ? !string.IsNullOrEmpty(s) : value is not null;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>Vrai si l'entier vaut 0.</summary>
public class EqualToZeroConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is int i && i == 0;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>Vrai si l'entier vaut 1.</summary>
public class EqualToOneConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is int i && i == 1;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>Vrai si l'entier vaut 2.</summary>
public class EqualToTwoConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is int i && i == 2;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>
/// Couleur de fond d'une puce de filtre.
/// Paramètre = valeur du filtre de la puce.
/// Valeur    = filtre actuellement sélectionné.
/// </summary>
public class FilterChipColorConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value?.ToString() == p?.ToString()
            ? Color.FromArgb("#E8D8D1")   // sélectionné : terracotta clair
            : Colors.Transparent;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>Couleur du texte d'une puce de filtre.</summary>
public class FilterChipTextColorConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value?.ToString() == p?.ToString()
            ? Color.FromArgb("#8C5A4A")   // sélectionné : terracotta
            : Color.FromArgb("#6B6B6B");  // non sélectionné : gris
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>Libellé au-dessus de la grille selon l'état de recherche.</summary>
public class SearchLabelConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is bool b && b ? "Résultats" : "Toutes mes pièces";
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}
