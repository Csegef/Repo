using SimpleTodo.Models;

namespace SimpleTodo
{
    public partial class EditPage : ContentPage
    {
        private Film editingFilm;

        public EditPage()
        {
            InitializeComponent();

            // ÚJ film felvitele
            editingFilm = new Film(); // itt létrejön, nem lesz null
        }

        public EditPage(Film film)
        {
            InitializeComponent();

            if (film == null)
            {
                // ha null, akkor is új példányt készítünk
                editingFilm = new Film();
            }
            else
            {
                editingFilm = film;

                // elõre kitöltjük a mezõket
                TitleEntry.Text = film.cim;
                GenreEntry.Text = film.mufaj;
                AgeEntry.Text = film.korhatar;
                DateEntry.Text = film.vetites_datum;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            editingFilm.cim = TitleEntry.Text;
            editingFilm.mufaj = GenreEntry.Text;
            editingFilm.korhatar = AgeEntry.Text;
            editingFilm.vetites_datum = DateEntry.Text;

            await App.Database.SaveFilmAsync(editingFilm);
            await Navigation.PopAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (editingFilm.id != 0) // csak meglévõt törlünk
            {
                bool confirmed = await DisplayAlert("Törlés", "Biztosan törlöd?", "Igen", "Nem");

                if (confirmed)
                {
                    await App.Database.DeleteFilmAsync(editingFilm);
                }
            }

            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
