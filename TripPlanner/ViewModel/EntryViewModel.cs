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
using Kotlin;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core;
using TripPlanner.View.Controls;
using Location = TripPlanner.Model.Location;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Java.Util.Zip;
using AndroidX.Lifecycle;
using System.Globalization;


namespace TripPlanner.ViewModel
{
    [QueryProperty(nameof(TripEntryParameters), "TripEntryParameters")]
    public partial class EntryViewModel : BaseViewModel
    {

        public String[] Types { get; } = ["Activity", "Eatery", "Lodging", "Flight", "Car Rental", "Other Transportation"];

        public ObservableCollection<Attachments> AttachmentList { get; } = new();
        [ObservableProperty]
        TripEntry tripentry;
       

        [ObservableProperty]
        bool isRefreshing;

       [ObservableProperty]
        public TimeSpan startTime;
        [ObservableProperty]
        public TimeSpan endTime;

       

        [ObservableProperty]
        public Location startLoc;
        [ObservableProperty]
        public Location endLoc;

       

        public List<Attachments> tempList = new();


        private Dictionary<string, object> tripEntryParameters;
        public Dictionary<string, object> TripEntryParameters
        {
            get => tripEntryParameters;
            set
            {
                tripEntryParameters = value;
                InitializeTripEntry(tripEntryParameters);
            }
        }



        public async void InitializeTripEntry(Dictionary<string, object> parameters)
        {
            if (TripEntryParameters != null)
            {
                var tempEnd = parameters["EndDate"];
                var tempStart = parameters["StartDate"];
                Tripentry = new TripEntry
                {
                    Id = Convert.ToInt32(parameters["Id"]),
                    TripId = Convert.ToInt32(parameters["TripId"]),
                    Title = parameters["Title"].ToString(),
                    StartDate = DateTime.Parse(parameters["StartDate"].ToString()),
                    EndDate = DateTime.Parse(parameters["EndDate"].ToString()),
                    Type = parameters["Type"].ToString(),
                    Reservation = parameters["Reservation"].ToString(),
                    StartPlace_id = parameters["StartPlace_id"].ToString(),
                    EndPlace_Id = parameters["EndPlace_Id"].ToString(),
                    StartLocation = parameters["StartLocation"].ToString(),
                    StartWeatherHigh = parameters["StartWeatherHigh"].ToString(),
                    StartWeatherLow = parameters["StartWeatherLow"].ToString(),
                    StartWeatherIcon = parameters["StartWeatherIcon"].ToString(),
                    EndWeatherHigh = parameters["EndWeatherHigh"].ToString(),
                    EndWeatherLow = parameters["EndWeatherLow"].ToString(),
                    EndWeatherIcon = parameters["EndWeatherIcon"].ToString(),
                    EndLocation = parameters["EndLocation"].ToString(),
                    Notes = parameters["Notes"].ToString()
                };
                var TimeStringStart = tempStart.ToString().Split(" ")[1];
                StartTime = TimeSpan.Parse(TimeStringStart);
                var TimeString = tempEnd.ToString().Split(" ")[1];
                EndTime = TimeSpan.Parse(TimeString);
                StartLoc = new Model.Location(Tripentry.StartLocation, Tripentry.StartPlace_id);
                EndLoc = new Model.Location(Tripentry.EndLocation, Tripentry.EndPlace_Id);
                await GetAttachmentsAsync();
            }
            else
            {
                // Handle the case where parameters are null
                
                StartLoc = new Model.Location();
                EndLoc = new Model.Location();
                Debug.WriteLine("Parameters are null. Cannot initialize TripEntry.");
            }
        }
        public EntryViewModel()
        {
            Title = "Trip Details";


        }
        

        [RelayCommand]
        public async Task AddAttachment()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var result = await FilePicker.PickAsync();

                if(result == null) return;

                Attachments attachments = new Attachments(Tripentry.Id,result.FileName,result.FullPath);
                AttachmentList.Add(attachments);
                tempList.Add(attachments);
               await GetAttachmentsAsync();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException);
            }
            finally { IsBusy = false; IsRefreshing = false; }
        }

        [RelayCommand]
        public async Task GetAttachmentsAsync()
        {
            //await DatabaseService.ClearDatabase();
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var list = await DatabaseService.GetAttachmentsAsync(Tripentry.Id);
                if (AttachmentList.Count != 0)
                {
                    AttachmentList.Clear();
                }
                foreach (var doc in list)
                    AttachmentList.Add(doc);


            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException);
            }
            finally { IsBusy = false; IsRefreshing = false; }

        }

        [RelayCommand]
        public async Task OpenAttachment(Attachments selected)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                string path = selected.Path;
                OpenFileRequest request = new OpenFileRequest
                {
                    File = new ReadOnlyFile(path)
                };
                
                bool result = await Launcher.OpenAsync(request);
                if (!result)
                    await Shell.Current.DisplayAlert("File Not Found!", "This file may have been deleted or moved! Add the file back again.", "OK");
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException);
            }
            finally { IsBusy = false; IsRefreshing = false; }
        }

        [RelayCommand]
       public async Task DeleteAttachment(Attachments a)
        {
            Debug.WriteLine("I am trying to Delete!");
            if (IsBusy)
                return;
            try
            {

                IsBusy = true;
                Debug.WriteLine("Id is " + a.Id);
                var rows = 0;
                if(tempList.Count ==0 | !tempList.Contains(a))
                {
                     rows = await DatabaseService.RemoveAttachment(a);

                }
                else
                    tempList.Remove(a);
                AttachmentList.Remove(a);
                //await GetAttachmentsAsync();
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
                Debug.WriteLine("I have finished Deleting");
            }

        }

        [RelayCommand]
        async Task CancelEntry()
        {
            if (IsBusy)
                return;
            try
            {
                Shell.Current.SendBackButtonPressed();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("I have finished Deleting");
            }

        }

         public async Task<string> Validation()
        {
            string errorMessage = "";
            Trip trip = await DatabaseService.GetTrip(Tripentry.TripId);
            if (Tripentry.Title.Length == 0)
               errorMessage +=" -- Title is required\n";
            if (Tripentry.EndDate < Tripentry.StartDate)
                 errorMessage += " -- The End Date has to be after the Start date\n";
            if (Tripentry.StartLocation is null || Tripentry.StartLocation.Length == 0)
                errorMessage += " -- Starting Location is required!\n";
            if (Tripentry.Type is null || Tripentry.Type.Length == 0)
                errorMessage += " -- Type is required!\n";
            if(trip.StartDate.Date >  Tripentry.StartDate.Date || trip.EndDate.Date < Tripentry.StartDate.Date)
                errorMessage += $" -- {Tripentry.Type} start date has to be between Trip start and end dates\n";
            if (trip.StartDate.Date > Tripentry.EndDate.Date || trip.EndDate.Date < Tripentry.EndDate.Date)
                errorMessage += $" -- {Tripentry.Type} end date has to be between Trip start and end dates\n";
            if(!Regex.IsMatch(Tripentry.Title, @"^[\w\s/,]*$"))
                errorMessage += " -- Title can only contain letters, numbers, spaces, commas and backslashes\n";
            if (!Regex.IsMatch(Tripentry.Reservation, @"^[\w\s-]*$"))
                errorMessage += " -- Title can only contain letters, numbers, spaces and dashes\n";
            return errorMessage;
        }

        [RelayCommand]
        async Task SaveEntry()
        {
            Debug.WriteLine("I am trying to Save!");
            if (IsBusy)
                return;
            try
            {

                IsBusy = true;
               
                String Error = "";
                //await Shell.Current.DisplayAlert("ksdf", Tripentry.Title, "OK");
               
                   
                    Tripentry.StartDate = Tripentry.StartDate.Date;
                    Tripentry.StartDate += StartTime;
                    Tripentry.StartLocation = StartLoc.Name;
                    Tripentry.StartPlace_id = StartLoc.Place_Id;
                    
                    Tripentry.EndDate = Tripentry.EndDate.Date;
                    Tripentry.EndDate += EndTime;
                    Tripentry.EndLocation = EndLoc.Name;
                    Tripentry.EndPlace_Id = EndLoc.Place_Id;

               
                

                Error = await Validation();
                if (Error.Length != 0)
                {
                    await Shell.Current.DisplayAlert("Error", "The following errors need to be fixed before saving:\n" + Error, "OK");
                    return;
                }

                if (!(Tripentry.StartLocation is null) && Tripentry.StartLocation.Length != 0)
                {
                    Dictionary<string, string> StartWeather;
                    if (Tripentry.StartPlace_id.Contains(","))
                    {
                        StartWeather = WeatherApiService.GetWeather(Tripentry.StartPlace_id + "/" + Tripentry.StartDate.Date.ToString("yyy-MM-dd")).Result;
                    }
                    else
                    {
                        StartWeather = WeatherApiService.GetWeather(Tripentry.StartLocation + "/" + Tripentry.StartDate.Date.ToString("yyy-MM-dd")).Result;
                        if (StartWeather["StatusCode"].Contains("BadRequest"))
                        {
                            Tripentry.StartPlace_id = GooglePlaceServices.GetLatLongAsync(Tripentry.StartPlace_id).Result;
                            StartWeather = WeatherApiService.GetWeather(Tripentry.StartPlace_id + "/" + Tripentry.StartDate.Date.ToString("yyy-MM-dd")).Result;

                        }
                    }
                    if (StartWeather["StatusCode"].Equals("OK"))
                    {
                        StartWeather.TryGetValue("tempmax", out string TempMax);
                        Tripentry.StartWeatherHigh = TempMax;
                        StartWeather.TryGetValue("tempmin", out string TempMin);
                        Tripentry.StartWeatherLow = TempMin;
                        StartWeather.TryGetValue("icon", out string TempIcon);
                        Tripentry.StartWeatherIcon = TempIcon?.Replace("-", "_") + ".svg";
                    }

                }


                if (!(Tripentry.EndLocation is null) && Tripentry.EndLocation.Length!=0)
                {
                    Dictionary<string, string> EndWeather;
                    if (Tripentry.EndPlace_Id.Contains(","))
                    {
                        EndWeather = WeatherApiService.GetWeather(Tripentry.EndPlace_Id + "/" + Tripentry.EndDate.Date.ToString("yyy-MM-dd")).Result;
                    }
                    else
                    {
                        EndWeather = WeatherApiService.GetWeather(Tripentry.EndLocation + "/" + Tripentry.EndDate.Date.ToString("yyy-MM-dd")).Result;
                        if (EndWeather["StatusCode"].Contains("BadRequest"))
                        {
                            Tripentry.EndPlace_Id = GooglePlaceServices.GetLatLongAsync(Tripentry.EndPlace_Id).Result;
                            EndWeather = WeatherApiService.GetWeather(Tripentry.EndPlace_Id + "/" + Tripentry.EndDate.Date.ToString("yyy-MM-dd")).Result;

                        }
                    }

                    if (EndWeather["StatusCode"].Equals("OK"))
                    {
                        EndWeather.TryGetValue("tempmax", out string TempMax);
                        Tripentry.EndWeatherHigh = TempMax;
                        EndWeather.TryGetValue("tempmin", out string TempMin);
                        Tripentry.EndWeatherLow = TempMin;
                        EndWeather.TryGetValue("icon", out string TempIcon);
                        Tripentry.EndWeatherIcon = TempIcon?.Replace("-", "_") + ".svg";
                    }
                }
                await DatabaseService.UpdateEntry(Tripentry);
                var newTripEntryId = Tripentry.Id;
                if(newTripEntryId == 0)
                    newTripEntryId = await  DatabaseService.GetLastEntry(Tripentry.Id); 

                foreach (Attachments attach in tempList)
                {
                    attach.EntryId = newTripEntryId;
                    await DatabaseService.AddAttachment(attach);
                }

                await Shell.Current.DisplayAlert("Saved!", "Trip Item sucessfully saved!", "OK");
                Shell.Current.SendBackButtonPressed();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                tempList.Clear();
                Debug.WriteLine("I have finished Saving");
            }

        }
    }
}
