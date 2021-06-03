using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public class UserRepository
{
    private SqliteConnection connection;
    public UserRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    //returns the number of users in the database
    public long GetCount()
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM users";
        long count = (long)command.ExecuteScalar();
        connection.Close();
        return count;

    }

    //checks if the user exists by his ID (necessary for generation)
    public bool UserExists(long id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM users WHERE id=$id";
        command.Parameters.AddWithValue("$id", id);
        SqliteDataReader reader = command.ExecuteReader();
        bool result = reader.Read();
        connection.Close();
        return result;

    }


    //adds a new user to the database
    public long Insert(User user)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText =
        @"
        INSERT INTO users (username, passwordHash, fullname, isModerator )
        VALUES ($username, $passwordHash, $fullname, $isModerator);
        SELECT last_insert_rowid();
        ";
        command.Parameters.AddWithValue("$username", user.userName);
        command.Parameters.AddWithValue("$passwordHash", user.passwordHash);
        command.Parameters.AddWithValue("$fullname", user.fullname);
        command.Parameters.AddWithValue("$isModerator", user.isModerator);
        long newId = (long)command.ExecuteScalar();
        connection.Close();
        return newId;
    }


    //finds the user in the database by his username and password and returns it
    public User GetUser(string userName, string passwordHash)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM users WHERE username = $username AND passwordHash = $passwordHash";
        command.Parameters.AddWithValue("$username", userName);
        command.Parameters.AddWithValue("$passwordHash", passwordHash);
        SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            User user = ParseUser(reader);
            connection.Close();
            return user;
        }
        reader.Close();
        connection.Close();
        return null;

    }


    //deletes the user by his ID
    public bool Delete(long id)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM users WHERE id=$id";
        command.Parameters.AddWithValue("$id", id);
        int nChanges = command.ExecuteNonQuery();
        connection.Close();
        return nChanges == 1;
    }


    //updates the user
    public bool Update(User user, long userId)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE users SET username=$username , passwordHash=$passwordHash , fullname=$fullname , isModerator=$isModerator WHERE id=$id";
        command.Parameters.AddWithValue("$id", userId);
        command.Parameters.AddWithValue("$username", user.userName);
        command.Parameters.AddWithValue("$passwordHash", user.passwordHash);
        command.Parameters.AddWithValue("$fullname", user.fullname);
        int moderatorNum = default;
        if (user.isModerator == true)
            moderatorNum = 1;
        else
            moderatorNum = 0;
        command.Parameters.AddWithValue("$isModerator", moderatorNum);
        int nChanged = command.ExecuteNonQuery();
        connection.Close();
        return nChanged == 1;

    }


    //parses user data that came from the database
    private User ParseUser(SqliteDataReader reader)
    {
        User user = new User();
        bool isId = long.TryParse(reader.GetString(0), out user.id);
        int moderatorNum;
        bool itIsModerator = int.TryParse(reader.GetString(4), out moderatorNum);

        if (isId && itIsModerator)
        {
            user.id = long.Parse(reader.GetString(0));
            user.userName = reader.GetString(1);
            user.passwordHash = reader.GetString(2);
            user.fullname = reader.GetString(3);
            user.isModerator = user.IsModerator(moderatorNum);
            return user;
        }
        else
        {
            throw new FormatException("Values cannot be parsed");
        }

    }


    //returns a list of user IDs
    public List<long> GetListOfUsersId()
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM users";
        SqliteDataReader reader = command.ExecuteReader();
        List<long> userListId = new List<long>();
        while (reader.Read())
        {
            User user = ParseUser(reader);
            userListId.Add(user.id);
        }
        reader.Close();
        connection.Close();
        return userListId;

    }


    //counts how many user pages there are
    public int GetTotalPages(int pageLength)
    {
        return (int)Math.Ceiling(this.GetCount() / (double)pageLength);
    }


    //returns a page with users
    public List<User> GetPageOfUsers(int pageNumber, int pageLength)
    {
        connection.Open();

        if (pageNumber < 1)
        {
            throw new ArgumentException(nameof(pageNumber));
        }

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM  users LIMIT $pageLength OFFSET $pageLength *($pageNumber -1 ) ";
        command.Parameters.AddWithValue("$pageLength", pageLength);
        command.Parameters.AddWithValue("$pageNumber", pageNumber);
        SqliteDataReader reader = command.ExecuteReader();
        List<User> usersList = new List<User>();

        while (reader.Read())
        {
            User user = ParseUser(reader);
            usersList.Add(user);
        }

        reader.Close();
        connection.Close();
        return usersList;

    }



}
