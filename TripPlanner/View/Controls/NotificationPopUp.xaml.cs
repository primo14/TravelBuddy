using Android.Service.Notification;
using CommunityToolkit.Maui.Views;

using System.Collections.ObjectModel;
using TripPlanner.Model;
using TripPlanner.Services;

namespace TripPlanner.View.Controls;

public partial class NotificationPopUp : Popup
{
	public ObservableCollection<Notification> Notifs = new();
	public NotificationPopUp()
	{
		InitializeComponent();
        
        
	}

    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        //await DatabaseService.AddNotification("Upcoming Flight", "YOu have a flight at 1:30");
        await updateList();
        NotificationList.ItemsSource = Notifs;
    }
    private async Task updateList()
    {
        //await DatabaseService.AddNotification("Upcoming Flight", "YOu have a flight at 1:30");
        var list = await DatabaseService.GetNotificationsAsync();
        if (Notifs.Count != 0)
            Notifs.Clear();
        foreach (Notification notif in list)
            Notifs.Add(notif);
    }
    private async void SwipeItemView_Invoked(object sender, EventArgs e)
    {
        SwipeItemView? current = sender as SwipeItemView;
        var notif = current?.BindingContext as Notification;
        await DatabaseService.RemoveNotification(notif);
        await updateList();

    }
}