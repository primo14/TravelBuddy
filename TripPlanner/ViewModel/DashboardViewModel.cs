using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Services;
using TripPlanner.Model;
using System.Diagnostics;
using TripPlanner.View;
using TripPlanner.View.Controls;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui;
using Java.Util.Zip;

namespace TripPlanner.ViewModel
{
    public partial class DashboardViewModel : BaseViewModel
    {
        public ObservableCollection<Trip> Trips { get; } = new();
        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        FileImageSource notifSource = "notifications_icon.svg";


        public DashboardViewModel()
        {
            Title = "Dashboard";
            

        }

        [RelayCommand]
        public async Task GetTripsAsync()
        {
            //await DatabaseService.ClearDatabase();
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var trips = await DatabaseService.GetTrips();
                if (Trips.Count != 0)
                {
                    Trips.Clear();
                }
                foreach (var trip in trips)
                    Trips.Add(trip);

            }
            catch (Exception ex)
            {

                await Shell.Current.DisplayAlert("Error", ex.Message + "\n" + ex.StackTrace, "OK");
            }
            finally { IsBusy = false; IsRefreshing = false;
               bool added = await DatabaseService.CheckForNotifications();
                if(added)
                    NotifSource = new FileImageSource { File = "notification_unread_icon.png" };
                
            }

        }

        [RelayCommand]
        async Task AddTrip()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                Trip newTrip = new Trip("New Trip", DateTime.Today, DateTime.Today.AddDays(7));
                AddTripPopup popup = new AddTripPopup(newTrip,"Add",new List<TripEntry>());
                Shell.Current.ShowPopup(popup);
                
                
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                await GetTripsAsync();
                Debug.WriteLine("I have finished adding");
            }


        }

        [RelayCommand]
        async Task SearchTrip()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                
                SearchPopup popup = new SearchPopup();
                Shell.Current.ShowPopup(popup);


            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                await GetTripsAsync();
                Debug.WriteLine("I have finished adding");
            }


        }

        [RelayCommand]
        async Task GetReport()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                ReportPopup popup = new ReportPopup();
                Shell.Current.ShowPopup(popup);


            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                await GetTripsAsync();
                Debug.WriteLine("I have finished adding");
            }


        }

        [RelayCommand]
        async Task DeleteTrip(Trip t)
        {
            Debug.WriteLine("I am trying to Delete!");
            if (IsBusy)
                return;
            try
            {

                IsBusy = true;
                var result = await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this trip?", "Yes", "No");
                if(!result)
                {
                    return;
                }
                Debug.WriteLine("Id is " + t.Id);
                var rows = await DatabaseService.RemoveTrip(t);
                Debug.WriteLine("Number of Rows Deleted? " + rows);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                await GetTripsAsync();
                Debug.WriteLine("I have finished Deleting");
            }

        }
        [RelayCommand]
        async Task SaveTrip(Trip t)
        {
            Debug.WriteLine("I am trying to Delete!");
            if (IsBusy)
                return;
            try
            {

                IsBusy = true;
                if (t.Title.Length == 0)
                {
                    await Shell.Current.DisplayAlert("Missing!", "Title is required", "OK");
                    return;
                }
                else if (t.EndDate < t.StartDate)
                {
                    await Shell.Current.DisplayAlert("Error!", "The End Date has to be after the Start date", "OK");
                    return;
                }

                Debug.WriteLine("Id is " + t.Id);
                await DatabaseService.UpdateTrip(t.Id, t.Title, t.StartDate, t.EndDate);
               
                await Shell.Current.DisplayAlert("Saved!", "Trip sucessfully saved!", "OK");
                

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                await GetTripsAsync();
                Debug.WriteLine("I have finished saving");
            }

        }
        [RelayCommand]
        async Task GoToTripDetails(Trip trip)
        {

            if (IsBusy | (trip is null))
            {
                Debug.WriteLine("Uh oh did not try to view trip");
                return;

            }
            try
            {

                IsBusy = true;
                Debug.WriteLine("------------------Right before changing pages!");
                await Shell.Current.GoToAsync($"{nameof(TripDetailsPage)}",
                    new Dictionary<string, object>
                    {
                        {"Trip",trip }
                    });

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("Viewing Trip");
            }

        }

        [RelayCommand]
        async Task DisplayNotifications()
        {
            if (IsBusy) return;
            try
            {

                NotifSource.File = "notifications_icon.svg";
                NotificationPopUp popup = new NotificationPopUp();
               
               await Shell.Current.CurrentPage.ShowPopupAsync<NotificationPopUp>(popup);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("Viewing Notifications");
            }
        }
    }
}
