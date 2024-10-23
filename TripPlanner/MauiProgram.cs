using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using TripPlanner.View;
using TripPlanner.ViewModel;
using GoogleApi;
using GoogleApi.Extensions;
//using task1.ViewModel;

namespace TripPlanner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            

            builder.Services.AddSingleton<Dashboard>();
            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddTransient<TripViewModel>();
            builder.Services.AddTransient<EntryViewModel>();
            builder.Services.AddTransient<TripDetailsPage>();
            builder.Services.AddTransient<EditEntryPage>();
            builder.Services.AddGoogleApiClients();



            return builder.Build();
        }
    }
}
