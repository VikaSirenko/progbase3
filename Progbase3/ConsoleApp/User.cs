using System;
using System.Collections.Generic;


public class User
{
    public long id;
    public string userName;
    public string passwordHash;
    public string fullname;
    public bool isModerator;

    public List<Post> posts;
    public List<Comment> comments;

    public User()
    {
        this.userName = default;
        this.passwordHash = default;
        this.fullname = default;
        this.isModerator = default;
    }

    public User(string userName, string password, string fullname, int moderatorNum)
    {
        this.userName = userName;
        this.passwordHash = Authentication.ConvertToHash(password);
        this.fullname = fullname;
        this.isModerator = IsModerator(moderatorNum);
    }


    public bool IsModerator(int moderatorNum)
    {
        if (moderatorNum == 0)
        {
            return false;
        }
        else if (moderatorNum == 1)
        {
            return true;
        }

        throw new ArgumentException($"Only the numbers [1] and [0] can be in the `isModerator` field, but the number [{moderatorNum}] is written there");

    }

    public override string ToString()
    {
        return $"[{id}] | User name:'{userName}' | Full name: '{fullname}'";
    }

}

