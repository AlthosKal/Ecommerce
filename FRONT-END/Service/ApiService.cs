using LIBRARY.Shared.Entity;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace FRONT_END.Service
{
    public class ApiService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
            };

            _baseUrl = ConfigService.ApiBaseUrl;
            Debug.WriteLine($"Base URL: {_baseUrl}");
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            try
            {
                var endpoint = $"{_baseUrl}/country";
                Debug.WriteLine($"Fetching countries from: {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Response JSON: {json}");

                    var countries = JsonSerializer.Deserialize<List<Country>>(json, _jsonSerializerOptions);
                    return countries ?? new List<Country>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error Response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Error fetching countries: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Request Error: {ex.Message}");
                string errorMessage = GetFriendlyErrorMessage(ex);
                await ShowErrorAlert("Error", errorMessage);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"Timeout Error: {ex.Message}");
                await ShowErrorAlert("Error", "La solicitud tardó demasiado. Verifique su conexión a internet.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception: {ex.Message}");
                await ShowErrorAlert("Error", $"Error inesperado: {ex.Message}");
                throw;
            }
        }

        public async Task<Country?> GetCountryAsyncById(int id)
        {
            try
            {
                var endpoint = $"{_baseUrl}/country/{id}";
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Country>(json, _jsonSerializerOptions);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error fetching country with ID {id}: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AddCountryAsync(Country country)
        {
            try
            {
                var endpoint = $"{_baseUrl}/country";
                var json = JsonSerializer.Serialize(country, _jsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Debug.WriteLine($"Sending POST to: {endpoint}");
                Debug.WriteLine($"Request JSON: {json}");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error Response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Error creating country: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateCountryAsync(Country country)
        {
            try
            {
                var endpoint = $"{_baseUrl}/country/{country.Id}";
                var json = JsonSerializer.Serialize(country, _jsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Debug.WriteLine($"Sending PUT to: {endpoint}");
                Debug.WriteLine($"Request JSON: {json}");

                var response = await _httpClient.PutAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error Response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Error updating country: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            try
            {
                var endpoint = $"{_baseUrl}/country/{id}";

                Debug.WriteLine($"Sending DELETE to: {endpoint}");

                var response = await _httpClient.DeleteAsync(endpoint);

                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error Response: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Error deleting country: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        private string GetFriendlyErrorMessage(HttpRequestException ex)
        {
            if (ex.Message.Contains("certificate") || ex.Message.Contains("SSL"))
            {
                return "Error de certificado SSL. Verifique la configuración del servidor.";
            }
            else if (ex.Message.Contains("connection") || ex.Message.Contains("refused"))
            {
                return "No se puede conectar al servidor. Verifique que el servidor esté ejecutándose.";
            }
            else if (ex.Message.Contains("timeout"))
            {
                return "La conexión tardó demasiado. Verifique su conexión a internet.";
            }
            return $"Error de conexión: {ex.Message}";
        }

        private async Task ShowErrorAlert(string title, string message)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}