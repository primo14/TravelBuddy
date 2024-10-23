using Android.Support.Customtabs.Trusted;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Java.Util.Zip;
using Microsoft.Maui.Controls;
using System.Text.RegularExpressions;
using TripPlanner.Model;
using TripPlanner.Services;
namespace TripPlanner.View.Controls;

public partial class AddTripPopup : Popup
{
    
	public Trip currentTrip= new();
    public string type;
    public List<TripEntry> entries;
	public AddTripPopup(Trip trip, string type, List<TripEntry> entries)
	{
		InitializeComponent();
        currentTrip.Id = trip.Id;
		currentTrip.Title = trip.Title;
        currentTrip.StartDate= trip.StartDate;
        currentTrip.EndDate= trip.EndDate;
        this.type = type;
        this.entries = entries;
        if (type.Equals("Update"))
            PopupTitle.Text = "Update Trip";
        
        else if (type.Equals("Add"))
            PopupTitle.Text = "Add Trip";

        EntryTitle.Text = currentTrip.Title;
        StartPicker.Date = currentTrip.StartDate;
        EndPicker.Date = currentTrip.EndDate;

	}

    private int checkDates()
    {
        int count=0;
        if(entries is null || entries.Count==0) { return 0; }
        entries.ForEach(entry =>
        {
            if (entry.StartDate > EndPicker.Date || entry.StartDate < StartPicker.Date || entry.EndDate < StartPicker.Date || entry.EndDate > EndPicker.Date)
            {
                count++;
            }
        });
        return count;
    }

        private async void Button_Pressed(object sender, EventArgs e)
    {
        string ErrorMessage = "";
        bool validated = true;
        var count = 0;
        if (type.Equals("Update"))
        {
            count = checkDates();
            if(count!=0)
                validated = false;
        }

        if (validated)
        {
            currentTrip.Title = EntryTitle.Text;
            currentTrip.StartDate = StartPicker.Date;
            currentTrip.EndDate = EndPicker.Date;

            if (currentTrip.Title is null || currentTrip.Title.Length == 0)
            {

                ErrorMessage += " - Title is Missing!\n";
                validated = false;
            }

            if (currentTrip.EndDate < currentTrip.StartDate)
            {
                ErrorMessage += " - Start Date cannot be after End Date!\n";
                validated = false;
            }
            if (!Regex.IsMatch(currentTrip.Title, @"^[\w\s/,]*$")) { 
                ErrorMessage += " -- Title can only contain letters, numbers, spaces, commas and backslashes\n";
                validated = false;
            }

            if (!validated)
            {
                await Shell.Current.DisplayAlert("Can't Save", "Fix the following problems to save:\n" + ErrorMessage, "OK");
                return;
            }
        }
        if (type.Equals("Update"))
        {
;            if(count != 0)
            {
               await Shell.Current.DisplayAlert("Can't Save",$" - Changed dates exclude {count} Trip Item(s)!\n   Delete the items that will interfere with your new dates before\n   changing Trip dates\n","OK");
                
                return;
            }
            else if(count == 0)
            {

                await DatabaseService.UpdateTrip(currentTrip.Id, currentTrip.Title, currentTrip.StartDate, currentTrip.EndDate);
                await Shell.Current.DisplayAlert("Saved", "Trip Successfully Saved!", "OK");
                this.Close();
            }

        }
       
        else if (type.Equals("Add"))
        {
            await DatabaseService.AddTrip(currentTrip);
            await Shell.Current.DisplayAlert("Saved", "Trip Successfully Saved!", "OK");
            await Shell.Current.GoToAsync($"{nameof(TripDetailsPage)}",
                       new Dictionary<string, object>
                       {
                        {"Trip",currentTrip }
                       });
            this.Close();
        }
        //this.Reset();

        
    }

    private  void CancelButton_Pressed(object sender, EventArgs e)
    {
        this.Close();
    }
}