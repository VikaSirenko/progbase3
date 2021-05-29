using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;


public class PostRepository
{
    private SqliteConnection connection;
    public PostRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    //checks if there is a post in the database
    public bool PostExists(long id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM posts WHERE id=$id";
        command.Parameters.AddWithValue("$id", id);
        SqliteDataReader reader = command.ExecuteReader();
        bool result = reader.Read();
        connection.Close();
        return result;
    }

    //returns the number of all posts in the database
    public long GetCount()
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM posts";
        long count = (long)command.ExecuteScalar();
        connection.Close();
        return count;
    }

    //adds a comment to the database
    public long Insert(Post post)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
        @"
        INSERT INTO posts (publicationText, publishedAt, userId, pinCommentId , imported)
        VALUES ($publicationText, $publishedAt, $userId, $pinCommentId, $imported);
        SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("$publicationText", post.publicationText);
        command.Parameters.AddWithValue("$publishedAt", post.publishedAt.ToString("o"));
        command.Parameters.AddWithValue("$userId", post.userId);
        command.Parameters.AddWithValue("$pinCommentId", post.pinCommentId);
        command.Parameters.AddWithValue("$imported", post.imported.ToString());
        long newId = (long)command.ExecuteScalar();
        connection.Close();
        return newId;
    }

    //returns a list of posts created by the user
    public List<Post> GetAllByUserId(long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM posts WHERE userId = $userId";
        command.Parameters.AddWithValue("$userId", userId);
        SqliteDataReader reader = command.ExecuteReader();
        List<Post> posts = new List<Post>();

        while (reader.Read())
        {
            Post post = new Post();
            post = ParsePostData(reader, post);
            posts.Add(post);
        }

        reader.Close();
        connection.Close();
        return posts;

    }

    //returns the post by its ID
    public Post GetByPostId(long id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM posts WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        SqliteDataReader reader = command.ExecuteReader();


        if (reader.Read())
        {

            Post post = new Post();
            post = ParsePostData(reader, post);
            connection.Close();
            return post;

        }

        reader.Close();
        connection.Close();
        return null;

    }


    //parses post data from database
    private Post ParsePostData(SqliteDataReader reader, Post post)
    {
        bool isId = long.TryParse(reader.GetString(0), out post.id);
        bool isPublishedAt = DateTime.TryParse(reader.GetString(2), out post.publishedAt);
        bool isPinnedCommentId = long.TryParse(reader.GetString(4), out post.pinCommentId);
        bool isUserId = long.TryParse(reader.GetString(3), out post.userId);
        bool isImported = bool.TryParse(reader.GetString(5), out post.imported);
        if (isId && isUserId && isPublishedAt && isPinnedCommentId && isImported)
        {
            post.id = long.Parse(reader.GetString(0));
            post.publicationText = reader.GetString(1);
            post.publishedAt = DateTime.Parse(reader.GetString(2));
            post.userId = long.Parse(reader.GetString(3));
            post.pinCommentId = long.Parse(reader.GetString(4));
            post.imported = bool.Parse(reader.GetString(5));
            return post;
        }
        else
        {
            throw new FormatException("Values cannot be parsed");

        }

    }

    //updates the post
    public bool Update(Post post, long postId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE posts SET publicationText=$publicationText , pinCommentId=$pinCommentId WHERE id=$id";
        command.Parameters.AddWithValue("$id", postId);
        command.Parameters.AddWithValue("$publicationText", post.publicationText);
        command.Parameters.AddWithValue("$pinCommentId", post.pinCommentId);
        int nChanged = command.ExecuteNonQuery();
        connection.Close();
        return nChanged == 1;

    }


    //deletes the post by its id
    public bool Delete(long postId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM posts WHERE id=$id";
        command.Parameters.AddWithValue("$id", postId);
        int nChanges = command.ExecuteNonQuery();
        connection.Close();
        return nChanges == 1;

    }


    //returns a list of IDs of all posts (needed to generate comments)
    public List<long> GetListOfPostsId(long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM posts WHERE userId = $userId";
        command.Parameters.AddWithValue("$userId", userId);
        SqliteDataReader reader = command.ExecuteReader();
        List<long> postListId = new List<long>();
        while (reader.Read())
        {
            Post post = new Post();
            post = ParsePostData(reader, post);
            postListId.Add(post.id);
        }
        reader.Close();
        connection.Close();
        return postListId;
    }


    //deletes all user-created posts
    public void DeleteAllByUserId(long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM posts WHERE userId=$userId";
        command.Parameters.AddWithValue("$userId", userId);
        int nChanges = command.ExecuteNonQuery();
        connection.Close();

    }


    //returns the number of pages of all posts
    public int GetTotalPages(int pageLength)
    {
        return (int)Math.Ceiling(this.GetCount() / (double)pageLength);
    }

    //returns a page with posts
    public List<Post> GetPageOfPosts(int pageNumber, int pageLength)
    {
        connection.Open();

        if (pageNumber < 1)
        {
            throw new ArgumentException(nameof(pageNumber));
        }

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM  posts LIMIT $pageLength OFFSET $pageLength *($pageNumber -1 ) ";
        command.Parameters.AddWithValue("$pageLength", pageLength);
        command.Parameters.AddWithValue("$pageNumber", pageNumber);
        SqliteDataReader reader = command.ExecuteReader();
        List<Post> postsList = new List<Post>();

        while (reader.Read())
        {
            Post post = new Post();
            post = ParsePostData(reader, post);
            postsList.Add(post);
        }

        reader.Close();
        connection.Close();
        return postsList;

    }


    //returns a list with posts that are filtered by matching text
    public List<Post> GetListOfFilteredPosts(string value)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM posts WHERE posts.publicationText LIKE '%' || $value || '%'  ";
        command.Parameters.AddWithValue("$value", value);

        SqliteDataReader reader = command.ExecuteReader();
        List<Post> postsList = new List<Post>();

        while (reader.Read())
        {
            Post post = new Post();
            post = ParsePostData(reader, post);
            postsList.Add(post);
        }

        reader.Close();
        connection.Close();
        return postsList;
    }


    //returns the user's posts page
    public List<Post> GetPageOfUserPosts(long userId, int pageNumber, int pageLength)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException(nameof(pageNumber));
        }
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM posts WHERE  userId=$userId  LIMIT $pageLength OFFSET $pageLength *($pageNumber -1 )";
        command.Parameters.AddWithValue("$userId", userId);
        command.Parameters.AddWithValue("$pageLength", pageLength);
        command.Parameters.AddWithValue("$pageNumber", pageNumber);
        SqliteDataReader reader = command.ExecuteReader();

        List<Post> posts = new List<Post>();
        while (reader.Read())
        {
            Post post = new Post();
            post = ParsePostData(reader, post);
            posts.Add(post);
        }

        reader.Close();
        connection.Close();
        return posts;
    }



    ////returns a page with posts that are filtered by matching text
    public List<Post> GetPageOfFilteredPosts(string value, int pageNumber, int pageLength)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException(nameof(pageNumber));
        }
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM posts WHERE  posts.publicationText LIKE '%' || $value || '%'  LIMIT $pageLength OFFSET $pageLength *($pageNumber -1 )";
        command.Parameters.AddWithValue("$value", value);
        command.Parameters.AddWithValue("$pageLength", pageLength);
        command.Parameters.AddWithValue("$pageNumber", pageNumber);
        SqliteDataReader reader = command.ExecuteReader();

        List<Post> posts = new List<Post>();
        while (reader.Read())
        {
            Post post = new Post();
            post = ParsePostData(reader, post);
            posts.Add(post);
        }

        reader.Close();
        connection.Close();
        return posts;

    }



}
