using System.Windows.Input;
using Vesti.Services;

namespace Vesti.ViewModels.Profile;

public class EditProfileViewModel : BaseViewModel
{
    private readonly ProfileService _profileService;

    public EditProfileViewModel(ProfileService profileService)
    {
        _profileService = profileService;

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    private string _displayName = string.Empty;
    public string DisplayName { get => _displayName; set => SetProperty(ref _displayName, value ?? string.Empty); }

    private string _email = string.Empty;
    public string Email { get => _email; set => SetProperty(ref _email, value ?? string.Empty); }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public async Task LoadAsync()
    {
        var profile = await _profileService.GetProfileAsync();
        DisplayName = profile?.DisplayName ?? string.Empty;
        Email = profile?.Email ?? string.Empty;
    }

    private async Task SaveAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var error = await _profileService.UpdateProfileAsync(DisplayName, Email);
            if (error is not null) { ErrorMessage = error; return; }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Impossible d'enregistrer les modifications.";
            System.Diagnostics.Debug.WriteLine($"[EditProfileViewModel] {ex}");
        }
        finally { IsBusy = false; }
    }
}
