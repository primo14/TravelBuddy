using SQLite;

namespace TripPlanner.Model
{
    [Table("Outfits")]
    public class Outfits
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TripId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }


        public Outfits()
        {
            Content = "";
            Date = new DateTime();
        }

        public Outfits(int tripId, DateTime date, string content)
        {
            TripId = tripId;
            Date = date;
            Content = content;
        }

    }
}
