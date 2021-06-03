using System.Security.Cryptography;
using System.Text;


public static class Authentication
{
    public static string ConvertToHash(string password)
    {
        SHA256 sha256Hash = SHA256.Create();
        string passwordHash = GetHash(sha256Hash, password);
        sha256Hash.Dispose();
        return passwordHash;
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

    public static User DoRegistration(UserRepository userRepository, User user)
    {
        User currentUser = userRepository.GetUser(user.userName, user.passwordHash);
        if (currentUser == null)
        {
            long id = userRepository.Insert(user);
            user.id = id;
            return user;
        }
        return null;
    }

    public static User DoLogIn(UserRepository userRepository, string passwordInput, string userNameInput)
    {
        string passwordHash = ConvertToHash(passwordInput);
        User currentUser = userRepository.GetUser(userNameInput, passwordHash);
        return currentUser;
    }



}
