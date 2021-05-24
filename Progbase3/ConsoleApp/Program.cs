using Microsoft.Data.Sqlite;
using Terminal.Gui;


class Program
{
    static void Main(string[] args)
    {

        string databaseFileName = "/home/vika/projects/progbase3/data/database";
        SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");
        UserRepository userRepository = new UserRepository(connection);
        PostRepository postRepository = new PostRepository(connection);
        CommentRepository commentRepository = new CommentRepository(connection);

        Authentication.DoAuthorization(userRepository, postRepository, commentRepository);


        //ConsoleApp.Generator.GenerateEntities();

    }

}


