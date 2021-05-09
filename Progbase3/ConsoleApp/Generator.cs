using Microsoft.Data.Sqlite;
using System;
using System.IO;
using static System.Console;
using System.Collections.Generic;


namespace ConsoleApp
{
    public static class Generator
    {
        public static void GenerateEntities()
        {
            string databaseFileName = "/home/vika/projects/progbase3/data/database";
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");
            bool run = true;
            while (run)
            {
                WriteLine("Enter the entity you want to generate");
                string entity = ReadLine();

                switch (entity)
                {
                    case "user":
                        GenereteUsers(connection);
                        break;
                    case "post":
                        GeneretePosts(connection);
                        break;
                    case "comment":
                        GenereteComments(connection);
                        break;
                    case "exit":
                        run = false;
                        break;
                    default:
                        Console.Error.WriteLine($"[{entity}] cannot be generated,because such an entity is not listed. Try again.");
                        break;

                }

                WriteLine();
            }
        }



        private static void GenereteUsers(SqliteConnection connection)
        {

            string usernamesPath = "/home/vika/projects/progbase3/data/generator/usernames.csv";
            int usernamesLines = 150000;
            string passwordsPath = "/home/vika/projects/progbase3/data/generator/passwords.csv";
            int passwordsLines = 150000;
            string namesPath = "/home/vika/projects/progbase3/data/generator/names.csv";
            int namesLines = 2000;
            string surnamePath = "/home/vika/projects/progbase3/data/generator/surnames.csv";
            int surnameLines = 1800;

            WriteLine("How many users do you want to generate?");
            int numberOfUsers;
            bool isNumberOfUsers = int.TryParse(ReadLine(), out numberOfUsers);

            if (isNumberOfUsers == false)
            {
                Console.Error.WriteLine("The number of users is incorrect. Try again");
                return;
            }

            while (numberOfUsers != 0)
            {
                string[] userName = (FindRandomLineInFile(usernamesPath, usernamesLines)).Split(",");
                string password = FindRandomLineInFile(passwordsPath, passwordsLines);
                string[] surname = FindRandomLineInFile(surnamePath, surnameLines).Split(",");
                string[] name = FindRandomLineInFile(namesPath, namesLines).Split(",");
                string fullname = name[0] + " " + surname[1];
                Random random = new Random();
                int isModeratorNum = default;                                //TODO
                //int isModeratorNum = random.Next(0, 2);
                User user = new User(userName[0], password, fullname, isModeratorNum);
                UserPepository userPepository = new UserPepository(connection);

                if (userPepository.UserExists(user.userName, user.passwordHash) == false)
                {
                    userPepository.Insert(user);
                    numberOfUsers--;
                }
            }
        }


        private static void GeneretePosts(SqliteConnection connection)
        {
            if (ValidateCountUsers(connection) == false)
            {
                return;
            }

            WriteLine("How many posts do you want to generate?");
            int numberOfPosts;
            bool isNumberOfPosts = int.TryParse(ReadLine(), out numberOfPosts);

            if (isNumberOfPosts == false)
            {
                Console.Error.WriteLine("The number of posts is incorrect. Try again.");
                return;
            }

            WriteLine("Set a numeric interval for the date of publication of posts in the format: dd.mm.yyyy-dd.mm.yyyy");
            string timeInterval = ReadLine();
            DateTime[] times = ParseDateTime(timeInterval);

            if (times != null)
            {
                while (numberOfPosts != 0)
                {
                    UserPepository userPepository = new UserPepository(connection);
                    Post post = new Post();
                    post.userId = GetRandomUserId(userPepository);

                    if (userPepository.UserExistsById(post.userId) == true)
                    {
                        string postPath = "/home/vika/projects/progbase3/data/generator/posts.csv";
                        int postsLines = 100000;
                        string[] publicationText = (FindRandomLineInFile(postPath, postsLines)).Split(",");
                        post.publicationText = publicationText[1];
                        post.publishedAt = RandomDateTime(times);
                        post.pinCommentId = default;                                   //TODO
                        PostRepository postRepository = new PostRepository(connection);
                        postRepository.Insert(post);
                        numberOfPosts--;
                    }
                }
            }
        }




        private static void GenereteComments(SqliteConnection connection)
        {
            if (ValidateCountUsers(connection) == false || ValidateCountPosts(connection) == false)
            {
                return;
            }

            WriteLine("How many comments do you want to generate?");
            int numberOfComments;
            bool isNumberOfComments = int.TryParse(ReadLine(), out numberOfComments);

            if (isNumberOfComments == false)
            {
                Console.Error.WriteLine("The number of comments is incorrect. Try again.");
                return;
            }

            WriteLine("Set a numeric interval for the date of publication of comments in the format: dd.mm.yyyy-dd.mm.yyyy");
            string timeInterval = ReadLine();
            DateTime[] times = ParseDateTime(timeInterval);

            if (times != null)
            {
                while (numberOfComments != 0)
                {
                    UserPepository userPepository = new UserPepository(connection);
                    PostRepository postRepository = new PostRepository(connection);
                    Comment comment = new Comment();
                    comment.userId = GetRandomUserId(userPepository);
                    long postId = GetRandomPostId(postRepository, comment.userId);
                    
                    if (postId == 0)
                    {
                        continue;
                    }

                    comment.postId = postId;

                    if (userPepository.UserExistsById(comment.userId) == true && postRepository.PostExists(comment.postId) == true)
                    {
                        string commentsPath = "/home/vika/projects/progbase3/data/generator/comments.csv";
                        int commentsLines = 100000;
                        string[] commentText = (FindRandomLineInFile(commentsPath, commentsLines)).Split(",");
                        comment.commentText = commentText[1];                  //TODO
                        comment.commentedAt = RandomDateTime(times);
                        Post post = postRepository.GetByPostId(comment.userId);
                        CommentRepository commentRepository = new CommentRepository(connection);
                        commentRepository.Insert(comment);
                        numberOfComments--;

                    }
                }
            }
        }


        private static DateTime RandomDateTime(DateTime[] timeInterval)
        {
            Random random = new Random();
            int range = (timeInterval[1] - timeInterval[0]).Days;
            DateTime randomDate = timeInterval[0].AddDays(random.Next(range)).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60)).AddSeconds(random.Next(0, 60));
            return randomDate;
        }


        private static DateTime[] ParseDateTime(string timeInterval)
        {
            string[] interval = timeInterval.Split("-");

            if (interval.Length != 2)
            {
                Console.Error.WriteLine("The time interval is set in the wrong format.");
                return null;
            }

            string[] startTime = interval[0].Split(".");
            string[] endTime = interval[1].Split(".");

            if (startTime.Length != 3 || endTime.Length != 3)
            {
                Console.Error.WriteLine("The time interval is set in the wrong format.");
                return null;
            }


            try
            {
                DateTime startDate = new DateTime(int.Parse(startTime[2]), int.Parse(startTime[1]), int.Parse(startTime[0]));
                DateTime endDate = new DateTime(int.Parse(endTime[2]), int.Parse(endTime[1]), int.Parse(endTime[0]));
                DateTime[] time = new DateTime[] { startDate, endDate };
                if (time[0] > time[1])
                {
                    Console.Error.WriteLine("Start date is greater than end date. This cannot be. Enter the time interval in the correct format");
                    return null;
                }

                return time;
            }

            catch
            {
                Console.Error.WriteLine("Date cannot be parsed.The time interval is set in the wrong format.");
                return null;
            }
        }


        private static string FindRandomLineInFile(string path, int num)
        {
            Random random = new Random();
            int numberOfLine = random.Next(1, num);
            StreamReader finder = new StreamReader(path);
            string search_line = "";

            while (numberOfLine > 0)
            {
                search_line = finder.ReadLine();
                numberOfLine--;
            }

            finder.Close();
            return search_line;
        }



        private static bool ValidateCountUsers(SqliteConnection connection)
        {
            UserPepository userPepository = new UserPepository(connection);
            long usersCount = userPepository.GetCount();

            if (usersCount != 0)
            {
                return true;
            }

            Console.Error.WriteLine("Users who could create posts and write comments do not exist. Generate users first.");
            return false;

        }

        private static bool ValidateCountPosts(SqliteConnection connection)
        {
            PostRepository postRepository = new PostRepository(connection);
            long postsCount = postRepository.GetCount();

            if (postsCount != 0)
            {
                return true;
            }

            Console.Error.WriteLine("Posts under which you can leave a comment does not exist. First generate posts.");
            return false;

        }

        private static long GetRandomUserId(UserPepository userPepository)
        {
            Random random = new Random();
            List<long> userListId = userPepository.GetListOfUsersId();
            int index = random.Next(0, userListId.Count);
            long userId = userListId[index];
            return userId;
        }

        private static long GetRandomPostId(PostRepository postRepository, long userId)
        {

            Random random = new Random();
            List<long> postListId = postRepository.GetListOfPostsId(userId);

            if (postListId.Count == 0)
            {
                return 0;
            }

            int index = random.Next(0, postListId.Count);
            long postId = postListId[index];
            return postId;

        }

    }
}