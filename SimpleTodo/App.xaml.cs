using SimpleTodo.Services;

namespace SimpleTodo;

public partial class App : Application
{
    public static FilmDatabase Database { get; private set; }

    public App()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "mozi.db");

        if (!File.Exists(dbPath))
        {
            string sourceFile = Path.Combine(Environment.CurrentDirectory, "mozi.db");
            if (File.Exists(sourceFile))
                File.Copy(sourceFile, dbPath);
        }

        Database = new FilmDatabase(dbPath);

        MainPage = new NavigationPage(new MainPage());
    }
}
