using CommunityToolkit.Maui.Views;
using Java.Security;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TripPlanner.Model;
using TripPlanner.Services;
using TripPlanner.Test;
using Xamarin.Google.Crypto.Tink.Proto;

namespace TripPlanner.View.Controls;

public partial class ReportPopup : Popup
{
    bool isBusy = false;
   
    public ReportPopup()
    {
        InitializeComponent();
        SearchList.ItemsSource = null;
        ReportTimeStamp.Text = DateTime.Now.ToString();
        
    }



    private async void SearchTripYear_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string pattern = @"^\d{4}$";
            string year = e.NewTextValue;
            if (year.Length == 0 || !Regex.IsMatch(year, pattern))
            {
                SearchList.ItemsSource = null;
                TripNumber.Text = "";
                TripsLabel.Text = "";
            }
            else if (Regex.IsMatch(year, pattern))
            {
                TripsLabel.Text = $"Trips Planned in {year} :";
                ReportTimeStamp.Text = DateTime.Now.ToString();
                var trips = await DatabaseService.GetTrips(new DateTime(int.Parse(year), 1, 1));
                TripNumber.Text = trips.Count() + "";
                List<Trip> current = new List<Trip>();
                foreach (var trip in trips)
                    current.Add(trip);
                SearchList.ItemsSource = current;
                //await ReportTesting.TestResult1(year, current);
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message + " \n" + ex.Source);
        }

    }

    private void ClosePopup(object sender, TappedEventArgs e)
    {
        this.Close();
    }
}