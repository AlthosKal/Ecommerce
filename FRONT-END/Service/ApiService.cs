using LIBRARY.Shared.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FRONT_END.Service
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7261/api/v1/country";
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
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
                try
                {
                    Debug.WriteLine($"Fetching countries from: {_baseUrl}");
                    var response = await _httpClient.GetAsync(_baseUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {// Read the response content as a string
                            var json = await response.Content.ReadAsStringAsync();
                            return JsonSerializer.Deserialize<List<Country>>(json, _jsonSerializerOptions);
                        }
                        catch (JsonException ex)
                        {
                            Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                            throw;
                        }
                    }
                    else
                    {
                        throw new Exception($"Error fetching countries: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception: {ex.Message}");
                    throw;
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Request Error: {ex.Message}");
                string errorMessage = $"An error occurred while fetching countries: {ex.Message}";
                if (ex.Message.Contains("certificate") || ex.Message.Contains("SSL"))
                {
                    errorMessage = "SSL certificate error. Please check your server's SSL configuration.";
                }
                else if (ex.Message.Contains("connection"))
                {
                    errorMessage = "Connection error. Please check your network connection.";
                }
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"An unexpected error occurred: ${ex.Message} Please try again later.",
                    "OK");
                throw;
            }
        }

        public async Task<Country> GetVountryAsyncById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Country>(json, _jsonSerializerOptions);
                }
                else
                {
                    throw new Exception($"Error fetching country with ID {id}: {response.StatusCode}");
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
                var json = JsonSerializer.Serialize(country, _jsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseUrl, content);
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
                var json = JsonSerializer.Serialize(country, _jsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/{country.Id}", content);
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
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }
}