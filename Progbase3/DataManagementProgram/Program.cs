using Microsoft.Data.Sqlite;
using Terminal.Gui;

class Program
{
    static void Main(string[] args)
    {
        string databaseFileName = "../../data/database";
        SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");
        try
        {
            UserRepository userRepository = new UserRepository(connection);
            PostRepository postRepository = new PostRepository(connection);
            CommentRepository commentRepository = new CommentRepository(connection);
            while (true)
            {
                Application.Init();
                Toplevel top = Application.Top;
                AuthenticationWindow window = new AuthenticationWindow();
                window.SetRepository(userRepository, postRepository, commentRepository);
                top.Add(window);
                Application.Run();
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.ErrorQuery("ERROR", $"There is no connection to the database: {ex.Message.ToString()}.Try again later.", "OK");
        }

    }

}


