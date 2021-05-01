using System.Security.Cryptography;
using System;
using System.Text;
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
        this.fullname=default;
        this.isModerator = default;
    }

    public User(string userName, string password, string fullname, int moderatorNum)
    {
        this.userName = userName;
        ConvertToHash(password);
        this.fullname= fullname;
        this.isModerator = IsModerator(moderatorNum);
    }

    public void ConvertToHash(string password)
    {
        SHA256 sha256Hash = SHA256.Create();
        this.passwordHash = GetHash(sha256Hash, password);
        sha256Hash.Dispose();

    }


    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {

        byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }


    private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
    {
        var hashOfInput = GetHash(hashAlgorithm, input);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        return comparer.Compare(hashOfInput, hash) == 0;
    }






    /*
        public int GetHashCode(string password) //Change 
        {
            int hash = default;

            for (int i = 0; i < password.Length; i++)
            {
                char c = password[i];
                hash += c;
            }

            return hash;
        }
        */


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

}
