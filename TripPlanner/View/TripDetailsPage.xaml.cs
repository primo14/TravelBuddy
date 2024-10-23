using AndroidX.Lifecycle;
using CommunityToolkit.Maui.Views;
using System.Security.Cryptography.X509Certificates;
using TripPlanner.Services;
using TripPlanner.View.Controls;
using TripPlanner.ViewModel;
using static Android.App.Assist.AssistStructure;

namespace TripPlanner.View;

public partial class TripDetailsPage : ContentPage
{

	TripViewModel _viewModel;
	public TripDetailsPage(TripViewModel viewModel)
    {
		_viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();

	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.GetEntriesAsync();
        await _viewModel.UpdateWeather();
        await _viewModel.GetEntriesAsync();
    }
   
}