using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO.Compression;
using System;
using System.IO;

public static class ReportGeneration
{

    public static void GenereteReport(string start, string end, string imagePath, string savePath, PostRepository postRepository, CommentRepository commentRepository, User currentUser)
    {
        DateTime startDate = DateTime.Parse(start);
        DateTime endDate = DateTime.Parse(end);
        string zipPath = "/home/vika/projects/progbase3/data/reportData/reportTemplate.docx";
        string extractPath = "/home/vika/projects/progbase3/data/reportData/reportExtract";
        if (!File.Exists(extractPath + "/mimetype"))
        {
            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }
        List<Post> postsOnInterval = GetListOfPostsOnTimeInterval(startDate, endDate, currentUser, postRepository);
        int postNum = postsOnInterval.Count;
        int comNum = GetListOfCommentsOnTimeInterval(startDate, endDate, currentUser, commentRepository).Count;
        Post postWithMaxCom = FindPostWithTheMostComments(postsOnInterval, commentRepository);
        XElement root = XElement.Load(extractPath + "/content.xml");
        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            {"{start}", start.ToString()},
            {"{end}", end.ToString()},
            {"{postsNum}", postNum.ToString()},
            {"{commentsNum}", comNum.ToString()},
            {"{postWithMaxCom}", postWithMaxCom.ToString()},
        };
        FindAndReplace(root, dict);
        root.Save(extractPath + "/content.xml");
        ZipFile.CreateFromDirectory(extractPath, savePath);

    }

    private static List<Post> GetListOfPostsOnTimeInterval(DateTime start, DateTime end, User currentUser, PostRepository postRepository)
    {
        List<Post> postsOnInterval = new List<Post>();
        currentUser.posts = postRepository.GetAllByUserId(currentUser.id);

        if (currentUser.posts.Count != 0)
        {
            for (int i = 0; i < currentUser.posts.Count; i++)
            {
                if (currentUser.posts[i].publishedAt >= start && currentUser.posts[i].publishedAt <= end)
                {
                    postsOnInterval.Add(currentUser.posts[i]);
                }

            }
        }
        return postsOnInterval;

    }

    private static List<Comment> GetListOfCommentsOnTimeInterval(DateTime start, DateTime end, User currentUser, CommentRepository commentRepository)
    {
        List<Comment> commentsOnInterval = new List<Comment>();
        currentUser.comments = commentRepository.GetAllByUserId(currentUser.id);
        if (currentUser.comments.Count != 0)
        {
            for (int i = 0; i < currentUser.comments.Count; i++)
            {
                if (currentUser.comments[i].commentedAt >= start && currentUser.comments[i].commentedAt <= end)
                {
                    commentsOnInterval.Add(currentUser.comments[i]);
                }
            }
        }
        return commentsOnInterval;
    }

    private static Post FindPostWithTheMostComments(List<Post> posts, CommentRepository commentRepository)
    {
        Post post = new Post();
        int postCount = default;
        if (posts.Count != 0)
        {
            for (int i = 0; i < posts.Count; i++)
            {
                posts[i].comments = commentRepository.GetAllByPostId(posts[i].id);
                if (posts[i].comments.Count > postCount)
                {
                    post = posts[i];
                }

            }
            return post;
        }
        return post;


    }

    static void FindAndReplace(XElement node, Dictionary<string, string> dict)
    {
        if (node.FirstNode != null
            && node.FirstNode.NodeType == XmlNodeType.Text)
        {
            string replecement;
            if (dict.TryGetValue(node.Value, out replecement))
            {
                node.Value = replecement;
            }

        }

        foreach (XElement el in node.Elements())
        {
            FindAndReplace(el, dict);
        }
    }




}
