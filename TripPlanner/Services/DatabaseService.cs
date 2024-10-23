using Android.Database;
using Android.Support.Customtabs.Trusted;
using Javax.Annotation;
using Kotlin;
using SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using TripPlanner.Model;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;
using static Android.Icu.Text.CaseMap;
using static Android.Security.Identity.CredentialDataResult;

namespace TripPlanner.Services
{
    public static class DatabaseService
    {
        private static SQLiteAsyncConnection _db;

        static async Task Init()
        {
            if (_db != null)
            {

                return;
            }

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "TripPlanner.db");
            _db = new SQLiteAsyncConnection(databasePath);

            string results = "";
            //Add tables below for all the Model Classes
            results += await _db.CreateTableAsync<Trip>();
            results += await _db.CreateTableAsync<TripEntry>();
            results += await _db.CreateTableAsync<Model.Notification>();
            results += await _db.CreateTableAsync<Attachments>();
            results += await _db.CreateTableAsync<Outfits>();
            //await LoadTestingData();
        }
        public static async Task ClearDatabase()
        {
           await Init();
            await _db.DropTableAsync<Trip>();
            await _db.DropTableAsync<TripEntry>();
            await _db.DropTableAsync<Attachments>();
            await _db.DropTableAsync<Outfits>();
            await _db.DropTableAsync<Model.Notification>();
            await _db.CreateTableAsync<Trip>();
            await _db.CreateTableAsync<TripEntry>();
            await _db.CreateTableAsync<Attachments>();
            await _db.CreateTableAsync<Outfits>();
            await _db.CreateTableAsync<Model.Notification>();

            

        }

       

        #region Trip

        public static async Task<Trip> AddTrip(string name, DateTime start, DateTime end)
        {
            await Init();
            Debug.WriteLine("I am starting to add trip:" + name);
            var nTrip = new Trip(name, start, end);
            var num = await _db.InsertAsync(nTrip);
            var id = nTrip.Id;
            Debug.WriteLine("Number of Added rows:" + num + " and id is " + nTrip.Id);
            return nTrip;

        }
        public static async Task AddTrip(Trip trip)
        {
            await Init();
            await _db.InsertAsync(trip);

        }
        public static async Task<int> RemoveTrip(Trip t)
        {
            await Init();
            var rows = await _db.DeleteAsync(t);
           rows+= await _db.ExecuteAsync("DELETE FROM TripEntry WHERE TripId = ?", t.Id);
            return rows;
        }
        public static async Task<IEnumerable<Trip>> GetTrips()
        {
            await Init();
            var trips = await _db.QueryAsync<Trip>("SELECT * FROM Trip ORDER BY StartDate;"); 
            return trips;
        }

        public static async Task<IEnumerable<Trip>> GetTrips(string name)
        {
            await Init();
            var trips = await _db.QueryAsync<Trip>("SELECT * FROM Trip WHERE Title LIKE '%"+name+"%';");
            return trips;
        }

        public static async Task<List<Trip>> GetTrips(DateTime date)
        {
            await Init();
            List<Trip> send = [];
            var trips = await _db.QueryAsync<Trip>("SELECT * FROM Trip;");
            foreach (Trip trip in trips)
            {
                if (trip.StartDate.Year.Equals(date.Year))
                {
                    send.Add(trip);
                }
            }
            
            return send;
        }


        public static async Task UpdateTrip(int id, string name, DateTime start, DateTime end)
        {
            await Init();

            var tripQuery = await _db.Table<Trip>().Where(i => i.Id == id).FirstOrDefaultAsync();

            if (tripQuery != null)
            {
                tripQuery.Title = name;
                tripQuery.StartDate = start;
                tripQuery.EndDate = end;
                await _db.UpdateAsync(tripQuery);
            }
            
        }
        public static async Task<int> CheckChangeTrip(Trip trip)
        {
            await Init();
            try
            {
                int count = 0;
                int id = trip.Id;
                var entries = await _db.QueryAsync<TripEntry>("SELECT * FROM TripEntry WHERE TripId = ?;", trip.Id);
                
                return count;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                return 0;
            }
            
        }

        public static async Task<Trip> GetTrip(int id)
        {
            await Init();

            return await _db.Table<Trip>().Where(i => i.Id == id).FirstOrDefaultAsync();


        }

        #endregion

        #region TripEntry


        public static async Task<TripEntry> AddEntry(int tripId, string name, string type, DateTime start, DateTime end, string reservation, int startId, int endId, string notes)

        {
            await Init();
            Debug.WriteLine("I am starting to add Trip Entry:" + name);

            var nEntry = new TripEntry(tripId, name,type, start, end,reservation,notes);
            var num = await _db.InsertAsync(nEntry);
            var id = nEntry.Id;
            Debug.WriteLine("Number of Added rows:" + num + " and id is " + nEntry.Id);
            return nEntry;

        }
        public static async Task<int> RemoveEntry(TripEntry e)
        {
            await Init();
            return await _db.DeleteAsync(e);
        }

        public static async Task<Dictionary<string, TimeSpan>> GetTimes(TripEntry e)
        {
            await Init();
            var dict = new Dictionary<string, TimeSpan>
            {
                ["Start"] = new TimeSpan(),
                ["End"] = new TimeSpan()
            };

            var entry =  GetEntry(e.Id).Result;
            dict["Start"] = entry.StartDate.TimeOfDay;
            dict["End"] = entry.EndDate.TimeOfDay;
            return dict;
        }
        public static async Task<IEnumerable<TripEntryGroup>> GetEntries(int tripId,List<DateTime> days)
        {
            try
            {
                await Init();
                Debug.WriteLine("------------Runing Query-------------");
                List<TripEntryGroup> groups = new List<TripEntryGroup>();
                
                var entries = await _db.QueryAsync<TripEntry>("SELECT * FROM TripEntry WHERE TripId = ? ORDER BY StartDate;", tripId);
                var outfits = await GetOutfitsAsync(tripId);

                foreach ( var day in days) 
                {
                   var outfit = outfits.Where( outfit => outfit.Date.Equals(day)).FirstOrDefault();
                    if(outfit is null)
                       outfit = await AddOutfit(tripId,day,"");

                    TripEntryGroup group = new TripEntryGroup(day,new List<TripEntry>(),outfit.Content);
                    foreach ( var entry in entries)
                    { 
                        if (entry.StartDate.Date.Equals(day.Date))
                            group.Add(entry);
                    }
                    groups.Add(group);
                }
               
                return groups;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message + " " + e.StackTrace);
                return Enumerable.Empty<TripEntryGroup>();
            }
           
        }

        public static async Task UpdateEntry(int id, string name, string type, DateTime start, DateTime end, string startLocation, string startPlace_id, 
            string endLocation, string endPlace_id,string reservation, string notes)
        {
            await Init();

            var entryQuery = await _db.Table<TripEntry>().Where(i => i.Id == id).FirstOrDefaultAsync();

            if (entryQuery != null)
            {
                entryQuery.Title = name;
                entryQuery.StartDate = start;
                entryQuery.EndDate = end;
                entryQuery.Type = type;
                entryQuery.Reservation = reservation;
                entryQuery.Notes = notes;
                entryQuery.StartPlace_id = startPlace_id;
                entryQuery.StartLocation = startLocation;
                entryQuery.EndPlace_Id = endPlace_id;
                entryQuery.EndLocation = endLocation;
                await _db.UpdateAsync(entryQuery);
            }
        }
        public static async Task UpdateEntry(TripEntry paramEntry)
        {
            await Init();

            var entryQuery = await _db.Table<TripEntry>().Where(i => i.Id == paramEntry.Id).FirstOrDefaultAsync();

            if (entryQuery != null)
            {
                
                await _db.UpdateAsync(paramEntry);
            }
            else
            {
                await _db.InsertAsync(paramEntry);
            }
           // return _db.Table<TripEntry>().
        }

        public static async Task UpdateEntryWeather(int id,  string startWeatherHigh, string startWeatherLow, string startWeatherIcon, string endWeatherHigh, string endWeatherLow, string endWeatherIcon)
        {
            await Init();

            var entryQuery = await _db.Table<TripEntry>().Where(i => i.Id == id).FirstOrDefaultAsync();

            if (entryQuery != null)
            {
                

                entryQuery.StartWeatherHigh = startWeatherHigh;
                entryQuery.StartWeatherLow = startWeatherLow;
                entryQuery.StartWeatherIcon = startWeatherIcon;

                entryQuery.EndWeatherHigh = endWeatherHigh;
                entryQuery.EndWeatherLow = endWeatherLow;
                entryQuery.EndWeatherIcon = endWeatherIcon;
                await _db.UpdateAsync(entryQuery);
            }
        }
        public static async Task<TripEntry> GetEntry(int id)
        {
            await Init();
            try
            {
               

               var entries = await _db.QueryAsync<TripEntry>("SELECT * FROM TripeEntry WHERE Id = ?", id);
                 var entry= entries.FirstOrDefault();
                return entry;
                
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + " " + e.StackTrace);
                return new TripEntry();
            }



        }

        public static async Task<int> GetLastEntry(int tripId)
        {
            await Init();

            var entries = await _db.QueryAsync<TripEntry>("SELECT * FROM TripEntry WHERE TripId = ? ORDER BY Id DESC LIMIT 1;",tripId);
            return entries.FirstOrDefault().Id;


        }
        #endregion

        #region Attachments
        public static async Task<IEnumerable<Attachments>> GetAttachmentsAsync(int entryId)
        {
           

            try
            {
                await Init();
                Debug.WriteLine("Running Query");
                var attachments = await _db.QueryAsync<Attachments>("SELECT * FROM Attachments WHERE EntryId = ?;", entryId);
                return attachments;
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return new List<Attachments>();
            }

        }
        public static async Task AddAttachment(Attachments attach)
        {
            await Init();
            Debug.WriteLine("I am starting to add Attachment:" + attach.Name);
            
            var num = await _db.InsertAsync(attach);
            
            Debug.WriteLine("Number of Added rows:" + num + " and id is " + attach.Id);

        }

        public static async Task<int> RemoveAttachment(Attachments attachment)
        {
            await Init();
            return await _db.DeleteAsync(attachment);
        }

        #endregion




        #region Outfits
        public static async Task<List<Outfits>> GetOutfitsAsync(int tripId)
        {
            await Init();

            try
            {
                await Init();
                Debug.WriteLine("Runing Query");
                var outfits = await _db.QueryAsync<Outfits>("SELECT * FROM Outfits WHERE TripId = ? ;", tripId);
                return outfits;
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return new List<Outfits>();
            }

        }
        public static async Task UpdateOutfits(int tripId, string content, DateTime date)
        {
            await Init();

            var outfitQuery = await _db.Table<Outfits>().Where(i => (i.TripId == tripId) &(i.Date.Equals(date))).FirstOrDefaultAsync();

            if (outfitQuery != null)
            {
                outfitQuery.Content = content;
                await _db.UpdateAsync(outfitQuery);
            }
            else
            {
                var nOutfit = new Outfits(tripId, date, content);
                var num = await _db.InsertAsync(nOutfit);
                Debug.WriteLine("Number of Added rows:" + num + " and id is " + nOutfit.Id);
            }

        }

        public static async Task<Outfits> AddOutfit(int tripId, DateTime date, string content)
        {
            await Init();
            Debug.WriteLine("I am starting to add Attachment:" + tripId + " " + date);
            var outfitQuery = await _db.Table<Outfits>().Where(i => (i.TripId == tripId) & (i.Date.Equals(date))).FirstOrDefaultAsync();
            if(outfitQuery is null)
            {
                var nOutfit = new Outfits(tripId, date, content);
                var num = await _db.InsertAsync(nOutfit);
                Debug.WriteLine("Number of Added rows:" + num + " and id is " + nOutfit.Id);
                return nOutfit;
            }
            return outfitQuery;

        }
        public static async Task<int> RemoveOutfits(Outfits outfits)
        {
            await Init();
            return await _db.DeleteAsync(outfits);
        }

        #endregion

        #region Notification


        public static async Task<bool> CheckForNotifications()
        {
            Debug.WriteLine("Checking for Notifications");
            bool added = false;
            var trips = await _db.Table<Trip>().ToListAsync();
            var entries = await _db.Table<TripEntry>().ToListAsync();
            foreach (Trip trip in trips)
            {
                if (trip.StartDate <= DateTime.Today.AddDays(2) & trip.StartDate >= DateTime.Now)
                    added = await AddNotification("Upcoming Trip!", $"Get Ready! {trip.Title} starts {trip.StartDate.ToLongDateString()}!");
            }
            foreach (TripEntry entry in entries)
            {
                if ((entry.StartDate.Subtract(DateTime.Now) <= new TimeSpan(2, 0, 0)) & entry.StartDate>=DateTime.Now)
                {
                    bool one=false,two=false, three = false;
                    if (entry.Type.Equals("Lodging"))
                        one = await AddNotification($"Upcoming Booking!", $"The check-in time for {entry.Title} is at {entry.StartDate.ToString("MMM dd, yyy hh:mm tt")} ");
                    else if (entry.Type.Equals("Other Transportation"))
                        two = await AddNotification($"Upcoming Transportation Planned!", $"You have planned for {entry.Title} at {entry.StartDate.ToString("MMM dd, yyy hh:mm tt")} ");
                    else
                       three = await AddNotification($"Upcoming {entry.Type}!", $"{entry.Title} starts at {entry.StartDate.ToString("MMM dd, yyy hh:mm tt")}");
                    if (!added)
                        added = one || two || three;
                }
                if ((entry.EndDate.Subtract(DateTime.Now) <= new TimeSpan(2,0,0)) & entry.EndDate >= DateTime.Now)
                {
                    bool one = false, two = false, three = false, four = false;
                    if (entry.Type.Equals("Lodging"))
                        one = await AddNotification($"Check Out Time Approaching!", $"The check-out time for {entry.Title} is at {entry.EndDate.ToString("MMM dd, yyy hh:mm tt")} ");
                    else if (entry.Type.Equals("Activity"))
                       two =  await AddNotification($"{entry.Title} Ending Soon!", $"{entry.Title} will be ending at {entry.EndDate.ToString("MMM dd, yyy hh:mm tt")} ");
                    else if (entry.Type.Equals("Car Rental"))
                       three=  await AddNotification($"Car Rental Ending", $"Rental  for {entry.Title} will be ending at {entry.EndDate.ToString("MMM dd, yyy hh:mm tt")} ");
                    else
                       four =  await AddNotification($"{entry.Type}", $"{entry.Title} ends at {entry.StartDate.ToString("MMM dd, yyy hh:mm tt")}");
                    if (!added)
                        added = one || two || three || four;
                }


            }
            return added;
        }
        public static async Task<List<Model.Notification>> GetNotificationsAsync()
        {
            await Init();

            try
            {
                await Init();
                Debug.WriteLine("Runing Query");
                var notifications = await _db.QueryAsync<Model.Notification>("SELECT * FROM Notifications;");
                return notifications;
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return new List<Notification>();
            }

        }

        public static async Task<bool> AddNotification(string title, string description)
        {
            await Init();
            Debug.WriteLine("I am starting to add Attachment: " + title);
            
            var check = await _db.QueryAsync<Notification>("SELECT * FROM Notifications WHERE Title = ? AND Description = ?", [title,description]);
            if (check.Count != 0)
                return false;
            var nNotif = new Model.Notification(title, description);
            var num = await _db.InsertAsync(nNotif);
            
            Debug.WriteLine("Number of Added rows:" + num + " and id is " + nNotif.Id);
            return true;

        }

        public static async Task<int> RemoveNotification(Model.Notification notification)
        {
            await Init();
            return await _db.DeleteAsync(notification);
        }

        #endregion


        #region Location
        public static async Task<Model.Location> GetLocationsAsync(int id)
        {
            await Init();

            try
            {
                await Init();
                Debug.WriteLine("Runing Query");
                var location = await _db.QueryAsync<Model.Location>("SELECT * FROM Location WHERE Id = ?;", id);
               
                return location.FirstOrDefault(new Model.Location());
            }
            catch (System.Exception ex)
            {
               
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return new Model.Location();
            }

        }

        public static async Task AddLocation(string name, string place_id)
        {
            await Init();
            Debug.WriteLine("I am starting to add Location: " + name);
            var check = await _db.QueryAsync<Model.Location>("SELECT * FROM Location WHERE Name = ? AND Place_Id = ?", [name,place_id]);
            if (check.Count != 0)
                return;
            var location = new Model.Location( name, place_id);
            var num = await _db.InsertAsync(location);
            var id = location.Id;
            Debug.WriteLine("Number of Added rows:" + num + " and id is " + location.Id);


        }

        public static async Task<int> RemoveLocation(Model.Location location)
        {
            await Init();
            return await _db.DeleteAsync(location);
        }

        #endregion


       

    }
}
