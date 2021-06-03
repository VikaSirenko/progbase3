using Microsoft.Data.Sqlite;
using Terminal.Gui;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string databaseFileName = "../../data/database";
        if (File.Exists(databaseFileName))
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");

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
        else
        {
            Application.Init();
            Toplevel top = Application.Top;
            Window window = new Window();
            top.Add(window);
            MessageBox.ErrorQuery("ERROR", $"There is no connection to the database.Try again later.", "OK");
            
        }

    }

}


