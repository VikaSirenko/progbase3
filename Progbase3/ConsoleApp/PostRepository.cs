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


    public void DeleteAllByUserId(long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM posts WHERE userId=$userId";
        command.Parameters.AddWithValue("$userId", userId);
        connection.Close();

    }

    public int GetTotalPages(int pageLength)
    {
        return (int)Math.Ceiling(this.GetCount() / (double)pageLength);
    }

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



    public List<Post> FilterByPostText(string value)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM posts, comments WHERE posts.publicationText LIKE '%' || $value || '%'  AND  posts.id=comments.postId ";
        command.Parameters.AddWithValue("$value", value);

        SqliteDataReader reader = command.ExecuteReader();

        List<Post> posts = new List<Post>();
        Post post = new Post();
        while (reader.Read())
        {
            post = ParsePostData(reader, post);
            if (!posts.Contains(post))                  //fix
            {
                posts.Add(post);
                post.comments = new List<Comment>();
            }
            Comment comment = new Comment();
            comment = ParseCommentData(reader, comment);
            post.comments.Add(comment);
        }

        reader.Close();
        connection.Close();
        return posts;
    }

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


    private Comment ParseCommentData(SqliteDataReader reader, Comment comment)
    {
        bool isId = long.TryParse(reader.GetString(5), out comment.id);
        bool isCommentedAt = DateTime.TryParse(reader.GetString(7), out comment.commentedAt);
        bool isUserId = long.TryParse(reader.GetString(8), out comment.userId);
        bool isPostId = long.TryParse(reader.GetString(9), out comment.postId);
        if (isId && isUserId && isCommentedAt && isPostId)
        {
            comment.id = long.Parse(reader.GetString(5));
            comment.commentText = reader.GetString(6);
            comment.commentedAt = DateTime.Parse(reader.GetString(7));
            comment.userId = long.Parse(reader.GetString(8));
            comment.postId = long.Parse(reader.GetString(9));
            return comment;
        }
        else
        {
            throw new FormatException("Values cannot be parsed");

        }
    }

    public long GetCountOfSelectedPosts(long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM posts WHERE userId=$userId";
        command.Parameters.AddWithValue("$userId", userId);
        long count = (long)command.ExecuteScalar();
        connection.Close();
        return count;
    }
    public int GetTotalPagesOfSelectedPosts(int pageLength, long userId)
    {
        return (int)Math.Ceiling(this.GetCountOfSelectedPosts(userId) / (double)pageLength);
    }

}
