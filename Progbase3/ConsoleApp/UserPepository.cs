using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public class UserPepository
{
    private SqliteConnection connection;
    public UserPepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public bool UserExists(string userName, string passwordHash)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM users WHERE username = $username OR passwordHash = $passwordHash";
        command.Parameters.AddWithValue("$username", userName);
        command.Parameters.AddWithValue("$passwordHash", passwordHash);
        SqliteDataReader reader = command.ExecuteReader();
        bool result = reader.Read();
        connection.Close();
        return result;
    }
    public long GetCount()
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM users";
        long count = (long)command.ExecuteScalar();
        connection.Close();
        return count;

    }

    public bool UserExistsById(long id)
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

    public User GetUser(string userName, int passwordHash)
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



    public bool Delete(string userName, int passwordHash)
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"DELETE FROM users WHERE username =$username AND passwordHash=$passwordHash";
        command.Parameters.AddWithValue("$username", userName);
        command.Parameters.AddWithValue("$passwordHash", passwordHash);
        int nChanges = command.ExecuteNonQuery();
        connection.Close();
        return nChanges == 1;
    }

    public bool Update(User user, long userId)
    {
        // bool isExist = this.UserExists(user.userName, user.passwordHash);
        // if (isExist == false)
        // {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE users SET username=$username , passwordHash=$passwordHash , fullname=$fullname WHERE id=$id";
        command.Parameters.AddWithValue("$id", userId);
        command.Parameters.AddWithValue("$username", user.userName);
        command.Parameters.AddWithValue("$passwordHash", user.passwordHash);
        command.Parameters.AddWithValue("$fullname", user.fullname);
        int nChanged = command.ExecuteNonQuery();
        connection.Close();
        return nChanged == 1;
        // }

    }

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


    public List<long> GetListOfUsersId()
    {
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM users";
        SqliteDataReader reader = command.ExecuteReader();
        List<long> userListId=new List<long>();
        while (reader.Read())
        {
            User user =ParseUser(reader);
            userListId.Add(user.id);
        }
        reader.Close();
        connection.Close();
        return  userListId;

    }


}
