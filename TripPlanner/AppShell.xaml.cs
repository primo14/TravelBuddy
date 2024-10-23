using TripPlanner.View;

namespace TripPlanner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Dashboard), typeof(Dashboard));
            Routing.RegisterRoute(nameof(TripDetailsPage), typeof(TripDetailsPage));
            Routing.RegisterRoute(nameof(EditEntryPage), typeof(EditEntryPage));

        }
    }
}
