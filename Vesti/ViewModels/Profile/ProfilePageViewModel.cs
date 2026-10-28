using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Vesti.Infrastructure;
using Vesti.Services;
using Vesti.Views.Onboarding;

namespace Vesti.ViewModels.Profile;

/// <summary>
/// ViewModel réel de l'onglet Profil (remplace le squelette statique).
///
/// DÉCISION ASSUMÉE : la maquette "Profil: Déconnexion" propose
/// "Se déconnecter" avec un texte évoquant un compte ("retrouvées à votre
/// prochaine connexion"). Le cadrage et nos décisions antérieures excluent
/// tout compte ou authentification distante — il n'y a rien dont se
/// "déconnecter". J'ai donc remplacé cette action par "Effacer mes
/// données", avec un avertissement honnête sur l'irréversibilité : c'est
/// la seule action de ce type qui ait un sens dans une application
/// strictement locale.
/// </summary>
public class ProfilePageViewModel : BaseViewModel
{
    private readonly ProfileService _profileService;
    private readonly ClothingService _clothingService;
    private readonly OutfitService _outfitService;
    private readonly DataResetService _dataResetService;
    private readonly IServiceProvider _services;

    public ProfilePageViewModel(ProfileService profileService,
                                ClothingService clothingService,
                                OutfitService outfitService,
                                DataResetService dataResetService,
                                IServiceProvider services)
    {
        _profileService = profileService;
        _clothingService = clothingService;
        _outfitService = outfitService;
        _dataResetService = dataResetService;
        _services = services;

        GoToEditProfileCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.EditProfile));
        GoToStatisticsCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.Statistics));
        GoToOutfitsCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.OutfitList));
        GoToSettingsCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.Settings));
        GoToHelpCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.Help));

        ShowClearDataConfirmCommand = new Command(() => IsClearDataConfirmVisible = true);
        CancelClearDataCommand = new Command(() => IsClearDataConfirmVisible = false);
        ConfirmClearDataCommand = new Command(async () => await ClearDataAsync());
    }

    private string _displayName = string.Empty;
    public string DisplayName { get => _displayName; private set => SetProperty(ref _displayName, value); }

    private string _email = string.Empty;
    public string Email { get => _email; private set => SetProperty(ref _email, value); }

    public string InitialLetter => string.IsNullOrEmpty(DisplayName) ? "?" : DisplayName[..1].ToUpper();

    private int _outfitCount;
    public int OutfitCount { get => _outfitCount; private set => SetProperty(ref _outfitCount, value); }

    private int _pieceCount;
    public int PieceCount { get => _pieceCount; private set => SetProperty(ref _pieceCount, value); }

    private bool _isClearDataConfirmVisible;
    public bool IsClearDataConfirmVisible
    {
        get => _isClearDataConfirmVisible;
        private set => SetProperty(ref _isClearDataConfirmVisible, value);
    }

    public ICommand GoToEditProfileCommand { get; }
    public ICommand GoToStatisticsCommand { get; }
    public ICommand GoToOutfitsCommand { get; }
    public ICommand GoToSettingsCommand { get; }
    public ICommand GoToHelpCommand { get; }
    public ICommand ShowClearDataConfirmCommand { get; }
    public ICommand CancelClearDataCommand { get; }
    public ICommand ConfirmClearDataCommand { get; }

    public async Task LoadAsync()
    {
        try
        {
            var profile = await _profileService.GetProfileAsync();
            DisplayName = profile?.DisplayName ?? string.Empty;
            Email = profile?.Email ?? string.Empty;
            OnPropertyChanged(nameof(InitialLetter));

            PieceCount = await _clothingService.CountAsync();
            OutfitCount = await _outfitService.CountAsync();

            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible de charger le profil.";
            System.Diagnostics.Debug.WriteLine($"[ProfilePageViewModel] {ex}");
        }
    }

    /// <summary>
    /// Efface les données puis ramène l'application à l'écran d'onboarding,
    /// exactement comme au premier lancement — même logique que
    /// App.xaml.cs.OnStart() quand IsFirstLaunchAsync() est vrai.
    /// </summary>
    private async Task ClearDataAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await _dataResetService.ClearAllDataAsync();
            IsClearDataConfirmVisible = false;

            var onboarding = _services.GetRequiredService<OnboardingPage>();
            var nav = new NavigationPage(onboarding)
            {
                BarBackgroundColor = Color.FromArgb("#FAF9F7")
            };
            NavigationPage.SetHasNavigationBar(onboarding, false);
            Application.Current!.MainPage = nav;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible d'effacer les données.";
            System.Diagnostics.Debug.WriteLine($"[ProfilePageViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }
}
