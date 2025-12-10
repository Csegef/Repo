using SQLite;
using SimpleTodo.Models;

namespace SimpleTodo.Services
{
    public class FilmDatabase
    {
        private readonly SQLiteAsyncConnection database;

        public FilmDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Film>().Wait();
        }

        public async Task<List<Film>> GetFilmsAsync()
        {
            return await database.Table<Film>().ToListAsync();
        }

        public async Task<int> SaveFilmAsync(Film film)
        {
            if (film.id == 0)
            {
                // ÚJ rekord
                return await database.InsertAsync(film);
            }
            else
            {
                // Meglévő módosítása
                return await database.UpdateAsync(film);
            }
        }

        public async Task<int> DeleteFilmAsync(Film film)
        {
            return await database.DeleteAsync(film);
        }
    }
}
