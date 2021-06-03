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


    // adds a comment to the database
    public long Insert(Comment comment)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
        @"
        INSERT INTO comments (commentText, commentedAt, userId, postId, imported)
        VALUES ($commentText, $commentedAt, $userId, $postId, $imported);
        SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("$commentText", comment.commentText);
        command.Parameters.AddWithValue("$commentedAt", comment.commentedAt.ToString("o"));
        command.Parameters.AddWithValue("$userId", comment.userId);
        command.Parameters.AddWithValue("$postId", comment.postId);
        command.Parameters.AddWithValue("$imported", comment.imported.ToString());
        long newId = (long)command.ExecuteScalar();
        connection.Close();
        return newId;
    }

    //returns a list of comments created by the user
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


    //returns a list of comments belonging to the post
    public List<Comment> GetAllByPostId(long postId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM comments WHERE postId = $postId";
        command.Parameters.AddWithValue("$postId", postId);
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

    //returns a list of comments for the filtered posts
    public List<Comment> GetAllFiltredComments(long postId, List<Comment> comments)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM comments WHERE postId = $postId";
        command.Parameters.AddWithValue("$postId", postId);
        SqliteDataReader reader = command.ExecuteReader();

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


    // parses comment data from a database
    private Comment ParseCommentData(SqliteDataReader reader, Comment comment)
    {
        bool isId = long.TryParse(reader.GetString(0), out comment.id);
        bool isCommentedAt = DateTime.TryParse(reader.GetString(2), out comment.commentedAt);
        bool isPostId = long.TryParse(reader.GetString(4), out comment.postId);
        bool isUserId = long.TryParse(reader.GetString(3), out comment.userId);
        bool isImported = bool.TryParse(reader.GetString(5), out comment.imported);
        if (isId && isUserId && isCommentedAt && isPostId && isImported)
        {
            comment.id = long.Parse(reader.GetString(0));
            comment.commentText = reader.GetString(1);
            comment.commentedAt = DateTime.Parse(reader.GetString(2));
            comment.userId = long.Parse(reader.GetString(3));
            comment.postId = long.Parse(reader.GetString(4));
            comment.imported = bool.Parse(reader.GetString(5));
            return comment;
        }
        else
        {
            throw new FormatException("Values cannot be parsed");

        }
    }



    //returns a comment on its id
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



    // updates the comment
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


    //removes the comment by id
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


    //returns the number of all comments
    public long GetCount()
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM comments";
        long count = (long)command.ExecuteScalar();
        connection.Close();
        return count;
    }


    //returns the number of comments that belong to filtered posts
    public long GetCountOfFilterComments(long postId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM comments WHERE postId=$postId";
        command.Parameters.AddWithValue("$postId", postId);
        long count = (long)command.ExecuteScalar();
        connection.Close();
        return count;
    }



    //returns the number of pages of all posts belonging to filtered posts
    public int GetTotalPagesOfFilterComments(int pageLength, long postId)
    {
        return (int)Math.Ceiling(this.GetCountOfFilterComments(postId) / (double)pageLength);
    }


    ////returns the number of pages of all posts
    public int GetTotalPages(int pageLength)
    {
        return (int)Math.Ceiling(this.GetCount() / (double)pageLength);
    }

    //returns a comment page
    public List<Comment> GetPageOfComments(int pageNumber, int pageLength)
    {
        connection.Open();

        if (pageNumber < 1)
        {
            throw new ArgumentException(nameof(pageNumber));
        }

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM  comments LIMIT $pageLength OFFSET $pageLength *($pageNumber -1 ) ";
        command.Parameters.AddWithValue("$pageLength", pageLength);
        command.Parameters.AddWithValue("$pageNumber", pageNumber);
        SqliteDataReader reader = command.ExecuteReader();
        List<Comment> commentsList = new List<Comment>();

        while (reader.Read())
        {
            Comment comment = new Comment();
            comment = ParseCommentData(reader, comment);
            commentsList.Add(comment);
        }

        reader.Close();
        connection.Close();
        return commentsList;

    }


    //returns a page of comments belonging to the selected post
    public List<Comment> GetPageOfCommentsOfSelectedPost(int pageNumber, int pageLength, long postId)
    {
        connection.Open();

        if (pageNumber < 1)
        {
            throw new ArgumentException(nameof(pageNumber));
        }

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM  comments WHERE postId=$postId LIMIT $pageLength OFFSET $pageLength *($pageNumber -1  ) ";
        command.Parameters.AddWithValue("$pageLength", pageLength);
        command.Parameters.AddWithValue("$pageNumber", pageNumber);
        command.Parameters.AddWithValue("$postId", postId);

        SqliteDataReader reader = command.ExecuteReader();
        List<Comment> commentsList = new List<Comment>();

        while (reader.Read())
        {
            Comment comment = new Comment();
            comment = ParseCommentData(reader, comment);
            commentsList.Add(comment);
        }

        reader.Close();
        connection.Close();
        return commentsList;

    }


    //deletes all comments belonging to the user
    public void DeleteAllByUserId(long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM comments WHERE userId=$userId";
        command.Parameters.AddWithValue("$userId", userId);
        int nChanges = command.ExecuteNonQuery();
        connection.Close();
    }


    //deletes all comments belonging to the post
    public void DeleteAllByPostId(long postId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM comments WHERE postId=$postId";
        command.Parameters.AddWithValue("$postId", postId);
        int nChanges = command.ExecuteNonQuery();
        connection.Close();
    }


    //checks if this comment is in the database
    public bool CommentExists(long id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM comments WHERE id=$id";
        command.Parameters.AddWithValue("$id", id);
        SqliteDataReader reader = command.ExecuteReader();
        bool result = reader.Read();
        connection.Close();
        return result;
    }


    //returns a page of comments that have been created by the user
    public List<Comment> GetPageOfUserComments(long userId, int pageNumber, int pageLength)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException(nameof(pageNumber));
        }
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM comments WHERE  userId=$userId  LIMIT $pageLength OFFSET $pageLength *($pageNumber -1 )";
        command.Parameters.AddWithValue("$userId", userId);
        command.Parameters.AddWithValue("$pageLength", pageLength);
        command.Parameters.AddWithValue("$pageNumber", pageNumber);
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


}
