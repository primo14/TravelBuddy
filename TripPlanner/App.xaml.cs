using TripPlanner.Services;
using TripPlanner.Test;

namespace TripPlanner
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            
        }

        protected override async void OnStart()
        {
            base.OnStart();
            
            await Shell.Current.DisplayAlert("Welcome", "Welcome!\nTo Delete/Edit/View Trips,Trip Items and Notifications, swipe either right or left. ", "OK");
        }

    }
}
