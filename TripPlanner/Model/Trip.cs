using SQLite;

namespace TripPlanner.Model
{
    public class Trip
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Trip()
        {
            Title = "";
            StartDate = new DateTime();
            EndDate = new DateTime();
        }
        public Trip(string name, DateTime start, DateTime end)
        {
            Title = name;
            StartDate = start;
            EndDate = end;

        }

    }
}
