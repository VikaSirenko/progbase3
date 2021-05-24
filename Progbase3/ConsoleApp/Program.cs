using System;
using Microsoft.Data.Sqlite;
using Terminal.Gui;
using ConsoleApp;
class Program
{
    static void Main(string[] args)
    {
        
        string databaseFileName = "/home/vika/projects/progbase3/data/database";
        SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");
        UserRepository userRepository = new UserRepository(connection);
        PostRepository postRepository= new PostRepository(connection);
        CommentRepository commentRepository=new CommentRepository(connection);

        Application.Init();

        Toplevel top = Application.Top;
        MainWindow window = new MainWindow();
        window.SetRepository(userRepository, postRepository, commentRepository);
        top.Add(window);

        Application.Run();
        

        //ConsoleApp.Generator.GenerateEntities();

    }

}


