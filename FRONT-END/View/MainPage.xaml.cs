using FRONT_END.ViewModels;

namespace FRONT_END.View;

public partial class MainPage : ContentPage
{
	private readonly CountryViewModel _countryViewModel;
    public MainPage(CountryViewModel countryViewModel)
	{
		InitializeComponent();
        _countryViewModel = countryViewModel;
        BindingContext = _countryViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _countryViewModel.LoadCountries();
    }
}