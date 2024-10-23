using CommunityToolkit.Maui.Views;
using GoogleApi.Entities.Search.Common;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TripPlanner.Model;
using TripPlanner.Services;

namespace TripPlanner.View.Controls;

public partial class SearchPopup : Popup
{
    bool isBusy = false;
	public SearchPopup()
	{
		InitializeComponent();
        SearchList.ItemsSource = null;
	}

    
   

    private async void ImageButton_Pressed(object sender, EventArgs e)
    {
        if (!isBusy)
        {
            isBusy= true;
            if (SearchTripName.Text.Length == 0)
            {
                SearchList.ItemsSource = null;
                return;
            }
            var trips = await DatabaseService.GetTrips(SearchTripName.Text);
            List<Trip> current = new List<Trip>();
            foreach (var trip in trips)
                current.Add(trip);
            Debug.WriteLine(current.ToArray().ToString());
            SearchList.ItemsSource = current;
        }
        isBusy = false;
    }

    private async void SearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SearchList.ItemsSource = null;
        Trip? trip = e.CurrentSelection.FirstOrDefault() as Trip;
        await Shell.Current.GoToAsync($"{nameof(TripDetailsPage)}",
                    new Dictionary<string, object>
                    {
                        {"Trip",trip }
                    });
        this.Close();;
    }

    private void SearchTripName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length == 0)
        {
            SearchList.ItemsSource = null;
        }
    }
}