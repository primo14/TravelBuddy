using AndroidX.Lifecycle;
using TripPlanner.Services;
using TripPlanner.Test;
using TripPlanner.ViewModel;

namespace TripPlanner.View;

public partial class Dashboard : ContentPage
{
    DashboardViewModel _viewModel;
	public Dashboard(DashboardViewModel viewmodel)
	{
		InitializeComponent();
        _viewModel = viewmodel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        //await ReportTesting.loadTestingData1();
        await _viewModel.GetTripsAsync();
    }
}