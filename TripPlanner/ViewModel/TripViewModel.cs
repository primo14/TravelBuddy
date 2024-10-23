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
using CommunityToolkit.Maui.Views;
using TripPlanner.View.Controls;
using Java.Util.Zip;
using Microsoft.Maui.Controls;
using AndroidX.Lifecycle;

namespace TripPlanner.ViewModel
{
    [QueryProperty(nameof(Trip), "Trip")]
    public partial class TripViewModel : BaseViewModel
    {

        [ObservableProperty]
        public Trip trip;

        public ObservableCollection<TripEntryGroup> GroupedEntries { get; } = new();
        [ObservableProperty]
        bool isRefreshing;

       
        public TripViewModel()
        {
            Title = "Trip Details";
            //Task.Run(() => GetEntriesAsync()).Wait();
            
        }

        public async Task UpdateWeather()
        {
            if (IsBusy) return;

            try
            {
                List<TripEntry> entries = new();
                foreach (var group in GroupedEntries)
                {

                    foreach (TripEntry entry in group)
                    {
                        if (!(entry.StartLocation is null))
                        {
                            
                            Dictionary<string, string> StartWeather;
                            if (entry.StartPlace_id.Contains(","))
                            {
                                StartWeather = WeatherApiService.GetWeather(entry.StartPlace_id + "/" + entry.StartDate.Date.ToString("yyy-MM-dd")).Result;
                            }
                            else
                            {
                                StartWeather = WeatherApiService.GetWeather(entry.StartLocation + "/" + entry.StartDate.Date.ToString("yyy-MM-dd")).Result;
                                if (StartWeather["StatusCode"].Contains("BadRequest"))
                                {
                                    entry.StartPlace_id = GooglePlaceServices.GetLatLongAsync(entry.StartPlace_id).Result;
                                    StartWeather = WeatherApiService.GetWeather(entry.StartPlace_id + "/" + entry.StartDate.Date.ToString("yyy-MM-dd")).Result;

                                }
                            }
                            if (StartWeather["StatusCode"].Equals("OK"))
                            {
                                StartWeather.TryGetValue("tempmax", out string TempMax);
                                entry.StartWeatherHigh = TempMax;
                                StartWeather.TryGetValue("tempmin", out string TempMin);
                                entry.StartWeatherLow = TempMin;
                                StartWeather.TryGetValue("icon", out string TempIcon);
                                entry.StartWeatherIcon = TempIcon?.Replace("-", "_") + ".svg";
                            }

                        }
                        


                        if (!(entry.EndLocation is null) & entry.EndLocation?.Length>0)
                        {
                           
                            Dictionary<string, string> EndWeather;
                            if (entry.EndPlace_Id.Contains(","))
                            {
                                EndWeather = WeatherApiService.GetWeather(entry.EndPlace_Id + "/" + entry.EndDate.Date.ToString("yyy-MM-dd")).Result;
                            }
                            else
                            {
                                EndWeather = WeatherApiService.GetWeather(entry.EndLocation + "/" + entry.EndDate.Date.ToString("yyy-MM-dd")).Result;
                                if (EndWeather["StatusCode"].Contains("BadRequest"))
                                {
                                    entry.EndPlace_Id = GooglePlaceServices.GetLatLongAsync(entry.EndPlace_Id).Result;
                                    EndWeather = WeatherApiService.GetWeather(entry.EndPlace_Id + "/" + entry.EndDate.Date.ToString("yyy-MM-dd")).Result;

                                }
                            }

                            if (EndWeather["StatusCode"].Equals("OK"))
                            {
                                EndWeather.TryGetValue("tempmax", out string TempMax);
                                entry.EndWeatherHigh = TempMax;
                                EndWeather.TryGetValue("tempmin", out string TempMin);
                                entry.EndWeatherLow = TempMin;
                                EndWeather.TryGetValue("icon", out string TempIcon);
                                entry.EndWeatherIcon = TempIcon?.Replace("-", "_") + ".svg";
                            }
                        }
                       
                        await DatabaseService.UpdateEntryWeather(entry.Id, entry.StartWeatherHigh, entry.StartWeatherLow, entry.StartWeatherIcon, entry.EndWeatherHigh, entry.EndWeatherLow,
                               entry.EndWeatherIcon);
                    }


                }

                Trip = await DatabaseService.GetTrip(Trip.Id);
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException);
            }
            finally
            {

                IsBusy = false; IsRefreshing = false;
            }

        }



        [RelayCommand]
        public async Task UpdateTrip()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                List<TripEntry> entries = new();
                foreach (var group in GroupedEntries) {

                    foreach (TripEntry entry in group)
                    {
                        entries.Add(entry);
                    }
                }
                AddTripPopup popup = new AddTripPopup(Trip, "Update", entries);
                await Shell.Current.ShowPopupAsync(popup);
                IsBusy = false;
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException); }
                
            finally {
                
                IsBusy = false;
                await GetEntriesAsync();
            }

        }

      

        [RelayCommand]
        public async Task GetEntriesAsync()
        {
            //await DatabaseService.ClearDatabase();
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Trip = await DatabaseService.GetTrip(Trip.Id);
                
                List<DateTime> days = new List<DateTime>();
                DateTime current = Trip.StartDate;
                while(current<=Trip.EndDate) 
                    {
                    days.Add(current);
                    current =current.AddDays(1);
                    /*groups.Add(new TripEntryGroup(current, new List<TripEntry>()));
                    current = current.AddDays(1);*/
                    }
               var groupedEntries = await DatabaseService.GetEntries(Trip.Id,days);
                if (GroupedEntries.Count != 0)
                {
                    GroupedEntries.Clear();
                }
                foreach (var entry in groupedEntries)
                    GroupedEntries.Add(entry);
               

            }
            catch (Exception ex){ Debug.WriteLine(ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException); }
            finally { IsBusy = false; IsRefreshing = false; }

        }
        [RelayCommand]
        async Task SaveOutfit(TripEntryGroup group)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                await DatabaseService.UpdateOutfits(Trip.Id, group.Outfits, group.GroupDate);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("I have finished adding");
            }


        }

        [RelayCommand]
        async Task GetDirections(TripEntry entry)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                DirectionsPopup popup = new DirectionsPopup(entry.StartLocation);
                await Shell.Current.CurrentPage.ShowPopupAsync<DirectionsPopup>(popup);
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("I have finished getting Directions");
            }


        }


        [RelayCommand]
        async Task AddEntry()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                TripEntry tripentry = new TripEntry(Trip.Id, "", "", Trip.StartDate, Trip.StartDate, "", "");
                var tripEntryDict = new Dictionary<string, object>
                {
                    {"Id", tripentry.Id},
                    {"TripId", tripentry.TripId},
                    {"Title", tripentry.Title},
                    {"StartDate", tripentry.StartDate.ToString("yyyy-MM-dd HH:mm")}, 
                    {"EndDate", tripentry.EndDate.ToString("yyyy-MM-dd HH:mm")},
                    {"Type", tripentry.Type},
                    {"Reservation", tripentry.Reservation},
                    {"StartPlace_id", tripentry.StartPlace_id},
                    {"EndPlace_Id", tripentry.EndPlace_Id},
                    {"StartLocation", tripentry.StartLocation},
                    {"StartWeatherHigh", tripentry.StartWeatherHigh},
                    {"StartWeatherLow", tripentry.StartWeatherLow},
                    {"StartWeatherIcon", tripentry.StartWeatherIcon},
                    {"EndWeatherHigh", tripentry.EndWeatherHigh},
                    {"EndWeatherLow", tripentry.EndWeatherLow},
                    {"EndWeatherIcon", tripentry.EndWeatherIcon},
                    {"EndLocation", tripentry.EndLocation},
                    {"Notes", tripentry.Notes}
                };

                await Shell.Current.GoToAsync($"{nameof(EditEntryPage)}", new Dictionary<string, object>
                                                                            {
                                                                                { "TripEntryParameters", tripEntryDict }
                                                                            });

            }

            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message + ex.Source + "\n" + ex.StackTrace + "\n" + ex.TargetSite);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("I have finished adding");
            }


        }

        [RelayCommand]
        async Task DeleteEntry(TripEntry e)
        {
            Debug.WriteLine("I am trying to Delete!");
            if (IsBusy)
                return;
            try
            {

                IsBusy = true;
                var result = await Shell.Current.DisplayAlert("Delete", "Are you sure you want to delete this Trip Itinerary Item?", "Yes", "No");
                if (!result)
                {
                    return;
                }
                Debug.WriteLine("Id is " + e.Id);
                var rows = await DatabaseService.RemoveEntry(e);
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
                await GetEntriesAsync();
                Debug.WriteLine("I have finished Deleting");
            }

        }
        [RelayCommand]
        async Task GoToEntryDetails(TripEntry tripentry)
        {

            if (IsBusy | (tripentry is null))
            {
                Debug.WriteLine("Uh oh did not try to view entry");
                return;

            }
            try
            {

                IsBusy = true;
                Debug.WriteLine("------------------Right before changing pages!");
                 var tripEntryDict = new Dictionary<string, object>
        {
            {"Id", tripentry.Id},
            {"TripId", tripentry.TripId},
            {"Title", tripentry.Title},
            {"StartDate", tripentry.StartDate.ToString("yyyy-MM-dd HH:mm")}, 
            {"EndDate", tripentry.EndDate.ToString("yyyy-MM-dd HH:mm")},
            {"Type", tripentry.Type},
            {"Reservation", tripentry.Reservation},
            {"StartPlace_id", tripentry.StartPlace_id},
            {"EndPlace_Id", tripentry.EndPlace_Id},
            {"StartLocation", tripentry.StartLocation},
            {"StartWeatherHigh", tripentry.StartWeatherHigh},
            {"StartWeatherLow", tripentry.StartWeatherLow},
            {"StartWeatherIcon", tripentry.StartWeatherIcon},
            {"EndWeatherHigh", tripentry.EndWeatherHigh},
            {"EndWeatherLow", tripentry.EndWeatherLow},
            {"EndWeatherIcon", tripentry.EndWeatherIcon},
            {"EndLocation", tripentry.EndLocation},
            {"Notes", tripentry.Notes}
        };

                await Shell.Current.GoToAsync($"{nameof(EditEntryPage)}", new Dictionary<string, object>
                                                                            {
                                                                                { "TripEntryParameters", tripEntryDict }
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
        
    }
}
