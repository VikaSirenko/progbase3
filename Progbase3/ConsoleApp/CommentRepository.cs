using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class CommentRepository
{
    private SqliteConnection connection;
    public CommentRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }


    public long Insert(Comment comment)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
        @"
        INSERT INTO comments (commentText, commentedAt, userId, postId)
        VALUES ($commentText, $commentedAt, $userId, $postId);
        SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("$commentText", comment.commentText);
        command.Parameters.AddWithValue("$commentedAt", comment.commentedAt);
        command.Parameters.AddWithValue("$userId", comment.userId);
        command.Parameters.AddWithValue("$postId", comment.postId);
        long newId = (long)command.ExecuteScalar();
        connection.Close();
        return newId;
    }

    public List<Comment> GetAllByUserId(long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM comments WHERE userId = $userId";
        command.Parameters.AddWithValue("$userId", userId);
        SqliteDataReader reader = command.ExecuteReader();
        List<Comment> comments = new List<Comment>();

        while (reader.Read())
        {
            Comment comment = new Comment();
            comment = ParseCommentData(reader, comment);
            comments.Add(comment);
        }

        reader.Close();
        connection.Close();
        return comments;

    }

    public List<Comment> GetAllByPostId(long postId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM comments WHERE postId = $postId";
        command.Parameters.AddWithValue("$userId", postId);
        SqliteDataReader reader = command.ExecuteReader();
        List<Comment> comments = new List<Comment>();

        while (reader.Read())
        {
            Comment comment = new Comment();
            comment = ParseCommentData(reader, comment);
            comments.Add(comment);
        }

        reader.Close();
        connection.Close();
        return comments;

    }

    private Comment ParseCommentData(SqliteDataReader reader, Comment comment)
    {
        bool isId = long.TryParse(reader.GetString(0), out comment.id);
        bool isCommentedAt = DateTime.TryParse(reader.GetString(2), out comment.commentedAt);
        bool isPostId = long.TryParse(reader.GetString(4), out comment.postId);
        bool isUserId = long.TryParse(reader.GetString(3), out comment.userId);
        if (isId && isUserId && isCommentedAt && isPostId)
        {
            comment.id = long.Parse(reader.GetString(0));
            comment.commentText = reader.GetString(1);
            comment.commentedAt = DateTime.Parse(reader.GetString(2));
            comment.userId = long.Parse(reader.GetString(3));
            comment.postId = long.Parse(reader.GetString(4));
            return comment;
        }
        else
        {
            throw new FormatException("Values cannot be parsed");

        }
    }



    public Comment GetByCommentId(long id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM comments WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {

            Comment comment = new Comment();
            comment = ParseCommentData(reader, comment);
            connection.Close();
            return comment;

        }

        reader.Close();
        connection.Close();
        return null;

    }




    public bool Update(Comment comment, long commentId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE comments SET  commentText=$commentText  WHERE id=$id";
        command.Parameters.AddWithValue("$id", commentId);
        command.Parameters.AddWithValue("$commentText", comment.commentText);
        int nChanged = command.ExecuteNonQuery();
        connection.Close();
        return nChanged == 1;

    }

    public bool Delete(long commentId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM comments WHERE id=$id";
        command.Parameters.AddWithValue("$id", commentId);
        int nChanges = command.ExecuteNonQuery();
        connection.Close();
        return nChanges == 1;
    }
     

}