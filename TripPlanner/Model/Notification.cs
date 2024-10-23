using SQLite;

namespace TripPlanner.Model
{
    [Table("Notifications")]
    public class Notification
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }


        public Notification()
        {
            Title = "New Notification";
            Description = "This is a new notification!";
        }

        public Notification(string title, string description)
        {
           Title= title;
            Description= description;
        }

    }
}
