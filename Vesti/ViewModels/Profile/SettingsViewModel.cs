using System.Windows.Input;
using Vesti.Infrastructure;

namespace Vesti.ViewModels.Profile;

/// <summary>
/// Page Paramètres. La langue (Français) et l'apparence (Claire) sont
/// fixes dans le MVP — affichées pour information, non modifiables.
/// Les "Rappels de tenues" ne sont pas implémentés (cadrage : notifications
/// reportées, P2 conditionnel) : le contrôle est visible mais désactivé,
/// avec un libellé honnête, plutôt que de présenter une fonction qui ne
/// ferait rien.
/// </summary>
public class SettingsViewModel : BaseViewModel
{
    public SettingsViewModel()
    {
        BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        GoToTermsCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.Terms));
        GoToPrivacyCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.Privacy));
    }

    public string Language => "Français";
    public string Appearance => "Claire";

    public ICommand BackCommand { get; }
    public ICommand GoToTermsCommand { get; }
    public ICommand GoToPrivacyCommand { get; }
}
