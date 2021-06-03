using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


public static class ExportAndImport
{
    public static void DoExportOfPosts(List<Post> posts, string outputFile)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Post>));
        StreamWriter output = new StreamWriter(outputFile);
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.NewLineHandling = NewLineHandling.Entitize;
        XmlWriter writer = XmlWriter.Create(output, settings);
        serializer.Serialize(output, posts);
        output.Close();
    }
    public static void DoExportOfComments(List<Comment> comments, string outputFile)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Comment>));
        StreamWriter output = new StreamWriter(outputFile);
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.NewLineHandling = NewLineHandling.Entitize;
        XmlWriter writer = XmlWriter.Create(output, settings);
        serializer.Serialize(output, comments);
        output.Close();
    }

    public static void DoImportOfPosts(string inputFile, PostRepository postRepository)
    {

        XmlSerializer serializer = new XmlSerializer(typeof(List<Post>));
        StreamReader reader = new StreamReader(inputFile);
        List<Post> posts = (List<Post>)serializer.Deserialize(reader);
        reader.Close();
        AddPostsToBd(posts, postRepository);


    }

    public static void DoImportOfComments(string inputFile, CommentRepository commentRepository)
    {

        XmlSerializer serializer = new XmlSerializer(typeof(List<Comment>));
        StreamReader reader = new StreamReader(inputFile);
        List<Comment> comments = (List<Comment>)serializer.Deserialize(reader);
        reader.Close();
        AddCommentsToBd(comments, commentRepository);

    }

    private static void AddPostsToBd(List<Post> posts, PostRepository postRepository)
    {
        if (posts.Count != 0)
        {
            for (int i = 0; i < posts.Count; i++)
            {
                if (!postRepository.PostExists(posts[i].id))
                {
                    posts[i].imported = true;
                    postRepository.Insert(posts[i]);
                }
            }
        }

    }

    private static void AddCommentsToBd(List<Comment> comments, CommentRepository commentRepository)
    {
        if (comments.Count != 0)
        {
            for (int i = 0; i < comments.Count; i++)
            {
                if (!commentRepository.CommentExists(comments[i].id))
                {
                    comments[i].imported = true;
                    commentRepository.Insert(comments[i]);
                }
            }
        }
    }


}
