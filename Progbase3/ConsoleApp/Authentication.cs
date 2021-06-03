using System;
using System.Security.Cryptography;
using System.Text;
using Terminal.Gui;

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


    public static void DoAuthorization(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
    {
        while (true)
        {
            Application.Init();
            Toplevel top = Application.Top;
            AuthenticationWindow window = new AuthenticationWindow();
            window.SetRepository(userRepository, postRepository, commentRepository);
            top.Add(window);
            Application.Run();
        }

    }


}
