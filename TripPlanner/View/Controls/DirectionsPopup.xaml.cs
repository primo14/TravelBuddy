using Android.Service.Notification;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TripPlanner.Model;
using TripPlanner.Services;

namespace TripPlanner.View.Controls;

public partial class DirectionsPopup : Popup
{
    
    public string Location;
    public DirectionsPopup(string location)
    {
        InitializeComponent();
        Location= location;
        EndLocation.Text = Location;
    }

    private async void EndLocationEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = e.NewTextValue;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var predictions = await GooglePlaceServices.GetPlacesPredictionAsync(search);
            predictionListEnd.ItemsSource = predictions;
            ;
        }
        else
            predictionListEnd.ItemsSource = null;
    }

    private void predictionListEnd_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Prediction prediction)
        {
            StartLocation.Text = prediction.description;
            predictionListEnd.ItemsSource = null;
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string start = StartLocation.Text.Replace(" ", "+").Replace(",","");
        string end = EndLocation.Text.Replace(" ", "+").Replace(",", "");
        string path = "https://google.com/maps/dir/?api=1&origin="+ start + "&destination=" + end ;

        bool result = await Launcher.OpenAsync(path);
        if (!result)
            await Shell.Current.DisplayAlert("Error","Directions could not be found.", "OK");
    }
}