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
    private Country? _selectedCountry;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _newCountry = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

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
            ErrorMessage = $"No se pudieron cargar los países: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OnCountrySelected()
    {
        if (SelectedCountry != null)
        {
            NewCountry = SelectedCountry.Name;
            ErrorMessage = string.Empty;
        }
    }

    [RelayCommand]
    public async Task AddCountry()
    {
        if (IsBusy) return;

        // Validaciones
        if (string.IsNullOrWhiteSpace(NewCountry))
        {
            ErrorMessage = "El campo es obligatorio";
            return;
        }

        if (Countries.Any(c => c.Name.Equals(NewCountry, StringComparison.OrdinalIgnoreCase)))
        {
            ErrorMessage = "El país ya existe";
            return;
        }

        if (NewCountry.Length > 100)
        {
            ErrorMessage = "El nombre del país no puede tener más de 100 caracteres";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var newCountry = new Country
            {
                Name = NewCountry.Trim(),
                States = new List<State>()
            };

            var success = await _apiService.AddCountryAsync(newCountry);
            if (success)
            {
                await LoadCountries();
                NewCountry = string.Empty;
                SelectedCountry = null;
            }
            else
            {
                ErrorMessage = "No se pudo agregar el país";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo agregar el país: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void EditCountry(Country country)
    {
        if (country != null)
        {
            SelectedCountry = country;
            NewCountry = country.Name;
            ErrorMessage = string.Empty;
        }
    }

    [RelayCommand]
    public async Task UpdateCountry()
    {
        if (SelectedCountry == null)
        {
            ErrorMessage = "Seleccione un país para actualizar";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewCountry))
        {
            ErrorMessage = "El campo es obligatorio";
            return;
        }

        if (Countries.Any(c => c.Name.Equals(NewCountry, StringComparison.OrdinalIgnoreCase) && c.Id != SelectedCountry.Id))
        {
            ErrorMessage = "El país ya existe";
            return;
        }

        if (NewCountry.Length > 100)
        {
            ErrorMessage = "El nombre del país no puede tener más de 100 caracteres";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var updatedCountry = new Country
            {
                Id = SelectedCountry.Id,
                Name = NewCountry.Trim(),
                States = SelectedCountry.States ?? new List<State>()
            };

            var success = await _apiService.UpdateCountryAsync(updatedCountry);
            if (success)
            {
                await LoadCountries();
                NewCountry = string.Empty;
                SelectedCountry = null;
            }
            else
            {
                ErrorMessage = "No se pudo actualizar el país";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo actualizar el país: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteCountry(Country? country = null)
    {
        var countryToDelete = country ?? SelectedCountry;
        if (countryToDelete == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmar",
            $"¿Está seguro de que desea eliminar el país '{countryToDelete.Name}'?",
            "Sí",
            "No");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var success = await _apiService.DeleteCountryAsync(countryToDelete.Id);
            if (success)
            {
                Countries.Remove(countryToDelete);
                if (SelectedCountry?.Id == countryToDelete.Id)
                {
                    NewCountry = string.Empty;
                    SelectedCountry = null;
                }
            }
            else
            {
                ErrorMessage = "No se pudo eliminar el país";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"No se pudo eliminar el país: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}