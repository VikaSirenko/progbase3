using Microsoft.Data.Sqlite;



class Program
{
    static void Main(string[] args)
    {
        string databaseFileName = "../../data/database";
        SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");
        UserRepository userRepository = new UserRepository(connection);
        PostRepository postRepository = new PostRepository(connection);
        CommentRepository commentRepository = new CommentRepository(connection);

        Authentication.DoAuthorization(userRepository, postRepository, commentRepository);


        //Generator.GenerateEntities();

    }

}


