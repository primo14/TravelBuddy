using SQLite;

namespace TripPlanner.Model
{


    public class TripEntryGroup : List<TripEntry>
    {
        public DateTime GroupDate { get; set; }
        public string Outfits { get; set; }

        public TripEntryGroup(DateTime groupDate, List<TripEntry> groupList, string outfits) : base(groupList)
        {
            GroupDate = groupDate;
            Outfits = outfits;
        }

        public TripEntryGroup() 
        {
            
        }

    }

    public class TripEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TripId { get; set; }   //Foreign Key
        public String Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public String Type { get; set; }
        public string Reservation { get; set; }
        public string StartPlace_id { get; set; }        //Foreign Key
        public string EndPlace_Id { get; set; }      //Foreign Key
        public string StartLocation { get; set; }
        public string StartWeatherHigh { get; set; }
        public string StartWeatherLow { get; set; }
        public string StartWeatherIcon { get; set; }
        public string EndWeatherHigh {get;set;}
        public string EndWeatherLow { get; set; }
        public string EndWeatherIcon { get; set; }
        public string EndLocation { get; set; }

        public string Notes { get; set; }

        public TripEntry()
        {
            TripId = 0;
            Title = "Empire State Building";
            StartDate = new DateTime();
            EndDate = new DateTime();
            Type = "Activity";
            Reservation = "";
            Notes = "";

        }

        public TripEntry(int tripId, string name,string type, DateTime start, DateTime end,string reservation,string notes)
        {
            TripId = tripId;
            Title = name;
            Type = type;
            StartDate = start;
            EndDate = end;
            Reservation = reservation;
            Notes = notes;
            StartPlace_id = "";
            EndPlace_Id = "";
            StartWeatherHigh = "";
            StartWeatherLow = "";
            StartWeatherIcon = "";
            EndWeatherHigh = "";
            EndWeatherLow = "";
            EndWeatherIcon = "";
            StartLocation = "";
            EndLocation = "";
        }

    }
}
