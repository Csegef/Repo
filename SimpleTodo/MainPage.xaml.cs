using SimpleTodo.Models;

namespace SimpleTodo;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        FilmCollection.ItemsSource = await App.Database.GetFilmsAsync();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditPage(null));
    }

    private async void OnFilmSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Film selected)
        {
            await Navigation.PushAsync(new EditPage(selected));
        }

        FilmCollection.SelectedItem = null;
    }
}
