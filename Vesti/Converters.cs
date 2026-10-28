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

/// <summary>
/// Inverse de NotNullConverter, mais pour des valeurs
/// non booléennes (string?, objet?). InvertBoolConverter ne peut PAS être
/// utilisé sur une valeur string — il retourne toujours false car il teste
/// "value is bool". Ce convertisseur comble ce manque.
/// Usage : IsVisible="{Binding SomeNullableString, Converter={StaticResource IsNullConverter}}"
/// </summary>
public class IsNullConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is string s ? string.IsNullOrEmpty(s) : value is null;
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

/// <summary>: vrai si l'entier est strictement positif</summary>
public class GreaterThanZeroConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is int i && i > 0;
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

public class RelativePathToFullConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c)
    {
        if (value is not string relativePath || string.IsNullOrWhiteSpace(relativePath))
            return null;

        var fullPath = Path.Combine(FileSystem.AppDataDirectory, relativePath);
        return File.Exists(fullPath) ? fullPath : null;
    }

    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

// convertisseurs du calendrier ─────────────────────────────────────

/// <summary>Fond de la case du jour sélectionné (terracotta) ou transparent.</summary>
public class SelectedDayColorConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is bool b && b ? Color.FromArgb("#8C5A4A") : Colors.Transparent;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>Texte blanc si le jour est sélectionné, sinon couleur de texte normale.</summary>
public class SelectedDayTextColorConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is bool b && b ? Colors.White : Color.FromArgb("#252525");
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>Opacité réduite pour les jours hors du mois affiché.</summary>
public class CurrentMonthOpacityConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is bool b && b ? 1.0 : 0.35;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}


/// <summary> titre de la page Statistiques selon l'état (vide ou non).</summary>
public class StatsTitleConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is bool b && b ? "Mon dressing en chiffres" : "En un regard";
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>
/// convertit un ratio (0.0–1.0) en largeur de pixel pour la barre
/// de progression des catégories. Référence 342 px (largeur mobile 390 px
/// moins les marges de 24 px de chaque côté, cf. fondations Vesti 2026).
/// Approximation volontaire : une vraie barre proportionnelle au conteneur
/// réel nécessiterait un comportement non trivial en XAML MAUI pur.
/// </summary>
public class RatioToWidthConverter : IValueConverter
{
    private const double ReferenceWidth = 342;

    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is double ratio ? Math.Max(2, ratio * ReferenceWidth) : 0.0;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary> : vrai si la chaîne est vide ou nulle (ex. e-mail non renseigné).</summary>
public class IsEmptyOrNullConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => string.IsNullOrWhiteSpace(value as string);
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}
