using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ecommerce.Shared.Entities;

using Ecommerce.Mobile.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Mobile.ViewModels
{
    public partial class CountriesViewModel : ObservableValidator
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<Country> _countries;

        [ObservableProperty]
        private Country _selectedCountry;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _newCountryName;

        [ObservableProperty]
        private string _errorMessage;

        public CountriesViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Countries = new ObservableCollection<Country>();
        }

        [RelayCommand]
        public async Task LoadCountries()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var countries = await _apiService.GetCountriesAsync();

                Countries.Clear();
                foreach (var country in countries)
                {
                    Countries.Add(country);
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
        public async Task AddCountry()
        {
            // Validación manual en lugar de usar atributos en el campo
            if (string.IsNullOrWhiteSpace(NewCountryName))
            {
                ErrorMessage = "El nombre del país no puede estar vacío";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                return;
            }

            if (NewCountryName.Length > 100)
            {
                ErrorMessage = "El nombre no puede tener más de 100 caracteres";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                return;
            }

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var newCountry = new Country
                {
                    Name = NewCountryName,
                    States = new List<State>()
                };

                bool success = await _apiService.AddCountryAsync(newCountry);

                if (success)
                {
                    await LoadCountries();
                    NewCountryName = string.Empty;
                }
                else
                {
                    ErrorMessage = "No se pudo agregar el país";
                    await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al agregar país: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task EditCountry()
        {
            if (SelectedCountry == null)
                return;

            // Aquí se implementaría la navegación a la página de edición
            // Por ahora, mostraremos un cuadro de diálogo sencillo para editar
            string result = await Shell.Current.DisplayPromptAsync("Editar País",
                "Ingrese el nuevo nombre:", initialValue: SelectedCountry.Name);

            if (!string.IsNullOrWhiteSpace(result))
            {
                if (result.Length > 100)
                {
                    ErrorMessage = "El nombre no puede tener más de 100 caracteres";
                    await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                    return;
                }

                var countryToUpdate = new Country
                {
                    Id = SelectedCountry.Id,
                    Name = result,
                    States = SelectedCountry.States
                };

                bool success = await _apiService.UpdateCountryAsync(countryToUpdate);

                if (success)
                {
                    await LoadCountries();
                }
                else
                {
                    ErrorMessage = "No se pudo actualizar el país";
                    await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                }
            }
        }

        [RelayCommand]
        public async Task DeleteCountry()
        {
            if (SelectedCountry == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar",
                $"¿Está seguro que desea eliminar {SelectedCountry.Name}?", "Sí", "No");

            if (confirm)
            {
                try
                {
                    IsBusy = true;
                    ErrorMessage = string.Empty;

                    bool success = await _apiService.DeleteCountryAsync(SelectedCountry.Id);

                    if (success)
                    {
                        Countries.Remove(SelectedCountry);
                        SelectedCountry = null;
                    }
                    else
                    {
                        ErrorMessage = "No se pudo eliminar el país";
                        await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al eliminar país: {ex.Message}";
                    await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        public void CountrySelected(Country country)
        {
            if (country != null)
            {
                SelectedCountry = country;
            }
        }
    }
}