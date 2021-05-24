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


    private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
    {
        var hashOfInput = GetHash(hashAlgorithm, input);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        return comparer.Compare(hashOfInput, hash) == 0;
    }


    public static void DoAuthorization(UserRepository userRepository, PostRepository postRepository, CommentRepository commentRepository)
    {
        Application.Init();

        Toplevel top = Application.Top;
        AuthenticationWindow window = new AuthenticationWindow();
        window.SetRepository(userRepository, postRepository, commentRepository);
        top.Add(window);

        Application.Run();

    }


}
