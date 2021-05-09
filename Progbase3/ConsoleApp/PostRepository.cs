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
    public long GetCount()
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM posts";
        long count = (long)command.ExecuteScalar();
        connection.Close();
        return count;


    }

    public long Insert(Post post)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
        @"
        INSERT INTO posts (publicationText, publishedAt, userId, pinCommentId )
        VALUES ($publicationText, $publishedAt, $userId, $pinCommentId);
        SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("$publicationText", post.publicationText);
        command.Parameters.AddWithValue("$publishedAt", post.publishedAt.ToString("o"));
        command.Parameters.AddWithValue("$userId", post.userId);
        command.Parameters.AddWithValue("$pinCommentId", post.pinCommentId);
        long newId = (long)command.ExecuteScalar();
        connection.Close();
        return newId;
    }

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


    private Post ParsePostData(SqliteDataReader reader, Post post)
    {
        bool isId = long.TryParse(reader.GetString(0), out post.id);
        bool isPublishedAt = DateTime.TryParse(reader.GetString(2), out post.publishedAt);
        bool isPinnedCommentId = long.TryParse(reader.GetString(4), out post.pinCommentId);
        bool isUserId = long.TryParse(reader.GetString(3), out post.userId);
        if (isId && isUserId && isPublishedAt && isPinnedCommentId)
        {
            post.id = long.Parse(reader.GetString(0));
            post.publicationText = reader.GetString(1);
            post.publishedAt = DateTime.Parse(reader.GetString(2));
            post.userId = long.Parse(reader.GetString(3));
            post.pinCommentId = long.Parse(reader.GetString(4));
            return post;
        }
        else
        {
            throw new FormatException("Values cannot be parsed");

        }

    }


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


    //???
    public void DeleteAllByUserId(long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM posts WHERE userId=$userId";
        command.Parameters.AddWithValue("$userId", userId);
        connection.Close();

    }







}
