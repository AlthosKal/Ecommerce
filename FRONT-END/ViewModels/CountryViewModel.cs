using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FRONT_END.Service;
using LIBRARY.Shared.Entity;
using System.Collections.ObjectModel;

namespace FRONT_END.ViewModels;

public partial class CountryViewModel : ObservableValidator
{
    private readonly ApiService _apiService;
    [ObservableProperty]
    private ObservableCollection<Country> _countries;

    [ObservableProperty]
    private Country _selectedCountry;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _newCountry;

    [ObservableProperty]
    private string _errorMessage;


    public CountryViewModel(ApiService apiService)
    {
        _apiService = apiService;
        _countries = new ObservableCollection<Country>();
    }

    [RelayCommand]
    public async Task LoadCountries()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            var countries = await _apiService.GetCountriesAsync();

            if (countries != null)
            {
                Countries.Clear();
                foreach (var country in countries)
                {
                    Countries.Add(country);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudieron cargar los pa�ses: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void SelectCountry()
    {
        if (SelectedCountry != null)
        {
            NewCountry = SelectedCountry.Name;
        }
    }

    [RelayCommand]
    public async Task AddCountry()
    {
        if (IsBusy) { return; }
        else if (string.IsNullOrWhiteSpace(NewCountry))
        {
            ErrorMessage = "El campo es obligatorio";
            return;
        }
        else if (Countries.Any(c => c.Name.Equals(NewCountry, StringComparison.OrdinalIgnoreCase)))
        {
            ErrorMessage = "El pa�s ya existe";
            return;
        }else if (NewCountry.Length > 100)
        {
            ErrorMessage = "El nombre del pa�s no puede tener m�s de 100 caracteres";
            return;
        }
        try
        {
            IsBusy = true;
            var newCountry = new Country { 
                Name = NewCountry,
                States = new List<State>()
            };
            var response = await _apiService.AddCountryAsync(newCountry);
            if (response != null)
            {
                await LoadCountries();
                NewCountry = string.Empty;
                SelectedCountry = null;
            }
            else
            {
                ErrorMessage = "No se pudo agregar el pa�s";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo agregar el pa�s: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task UpdateCountry()
    {
        if (SelectedCountry == null)
        {
            ErrorMessage = "Seleccione un pa�s para actualizar";
            return;
        }
        else if (string.IsNullOrWhiteSpace(NewCountry))
        {
            ErrorMessage = "El campo es obligatorio";
            return;
        }
        else if (Countries.Any(c => c.Name.Equals(NewCountry, StringComparison.OrdinalIgnoreCase) && c.Id != SelectedCountry.Id))
        {
            ErrorMessage = "El pa�s ya existe";
            return;
        }
        else if (NewCountry.Length > 100)
        {
            ErrorMessage = "El nombre del pa�s no puede tener m�s de 100 caracteres";
            return;
        }
        try
        {
            IsBusy = true;
            var updatedCountry = new Country { 
                Id = SelectedCountry.Id,
                Name = NewCountry,
                States = SelectedCountry.States
            };
            var response = await _apiService.UpdateCountryAsync(updatedCountry);
            if (response != null)
            {
                LoadCountries();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo actualizar el pa�s: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteCountry()
    {
        if (SelectedCountry == null) { return; }

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmar",
            $"�Est� seguro de que desea eliminar el pa�s '{SelectedCountry.Name}'?",
            "S�",
            "No");
        if (!confirm) { return; }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            var response = await _apiService.DeleteCountryAsync(SelectedCountry.Id);
            if (response)
            {
                Countries.Remove(SelectedCountry);
                SelectedCountry = null;
            }
            else
            {
                ErrorMessage = "No se pudo eliminar el pa�s";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo eliminar el pa�s: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }
}