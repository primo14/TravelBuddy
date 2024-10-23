using AndroidX.Lifecycle;
using Microsoft.Maui.Controls;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using TripPlanner.Model;
using TripPlanner.Services;
using TripPlanner.ViewModel;

namespace TripPlanner.View;

public partial class EditEntryPage : ContentPage
{
    EntryViewModel _viewModel;
    public EditEntryPage(EntryViewModel viewModel)
    {

       
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);        
       
        
        predictionListStart.ItemsSource = null;
        predictionListEnd.ItemsSource = null;
        


    }
    

    private async void EntryStart_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = e.NewTextValue;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var predictions = await GooglePlaceServices.GetPlacesPredictionAsync(search);
            predictionListStart.ItemsSource = predictions;
            ;
        }
        else
            predictionListStart.ItemsSource = null;
    }

    private void ListViewStart_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if(e.Item is Prediction prediction)
        {
            StartLocationEntry.Text = prediction.description;
            _viewModel.StartLoc.Place_Id=prediction.place_id;
            predictionListStart.ItemsSource = null;
        }
    }
    private async void EntryEnd_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = e.NewTextValue;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var predictions = await GooglePlaceServices.GetPlacesPredictionAsync(search);
            predictionListEnd.ItemsSource = predictions;
            ;
        }
        else
            predictionListEnd.ItemsSource = null;
    }

    private void ListViewEnd_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Prediction prediction)
        {
            EndLocationEntry.Text = prediction.description;
            _viewModel.EndLoc.Place_Id = prediction.place_id;
            predictionListEnd.ItemsSource = null;
            EndLocationEntry.CursorPosition = 0;
        }
    }

    void ChangeCursor()
    {

    }
}