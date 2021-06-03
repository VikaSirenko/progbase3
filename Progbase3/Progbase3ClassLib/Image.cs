using System;
using ScottPlot;
using System.Collections.Generic;



public static class Image
{
    public static void GenereteImage(PostRepository postRepository, CommentRepository commentRepository, DateTime firstDay, DateTime lastDay, string savePath, User currentUser)
    {

        int a = firstDay.Year - lastDay.Year;
        if (a == 0)
        {
            GenerateImageGraphicsForOneYear(firstDay, currentUser, postRepository, commentRepository, savePath);
        }
        else
        {
            GenerateGraphicImagesForSeveralYears(firstDay, lastDay, currentUser, postRepository, commentRepository, savePath);
        }

    }


    private static void GenerateImageGraphicsForOneYear(DateTime firstDay, User currentUser, PostRepository postRepository,
            CommentRepository commentRepository, string savePath)
    {
        var plt = new ScottPlot.Plot(600, 400);
        string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        double[] xs = DataGen.Consecutive(months.Length);
        double[] posts = new double[months.Length];
        double[] comments = new double[months.Length];
        for (int i = 0; i < months.Length; i++)
        {
            DateTime startDay = new DateTime(firstDay.Year, i + 1, 1);
            DateTime endDay;
            if (i == 11)
            {
                endDay = new DateTime(firstDay.Year + 1, 1, 1);
            }
            else
            {
                endDay = new DateTime(firstDay.Year, i + 2, 1);
            }
            int postsCount = ReportGeneration.GetListOfPostsOnTimeInterval(startDay, endDay, currentUser, postRepository).Count;
            posts[i] = postsCount;
            int commentsCount = ReportGeneration.GetListOfCommentsOnTimeInterval(startDay, endDay, currentUser, commentRepository).Count;
            comments[i] = commentsCount;
        }
        plt.PlotScatter(xs, posts, label: "Posts");
        plt.PlotScatter(xs, comments, label: "Comments");
        plt.Legend();
        plt.Ticks(xTickRotation: 45);
        plt.XTicks(xs, months);
        plt.SaveFig(savePath);
    }

    private static void GenerateGraphicImagesForSeveralYears(DateTime firstDay, DateTime lastDay, User currentUser,
            PostRepository postRepository, CommentRepository commentRepository, string savePath)
    {
        var plt = new ScottPlot.Plot(600, 400);
        int yearsCount = lastDay.Year - firstDay.Year + 2;
        string[] years = new string[yearsCount];
        for (int i = 0; i < yearsCount; i++)
        {
            years[i] = (firstDay.Year + i).ToString();
        }

        double[] xs = DataGen.Consecutive(years.Length);
        double[] posts = new double[years.Length];
        double[] comments = new double[years.Length];
        List<Post> postList = ReportGeneration.GetListOfPostsOnTimeInterval(firstDay, lastDay, currentUser, postRepository);
        List<Comment> commentList = ReportGeneration.GetListOfCommentsOnTimeInterval(firstDay, lastDay, currentUser, commentRepository);
        for (int i = 0; i < years.Length; i++)
        {

            int postCount = default;
            int commentCount = default;
            if (postList.Count != 0)
            {
                for (int j = 0; j < postList.Count; j++)
                {
                    if (firstDay.Year + i == postList[j].publishedAt.Year)
                    {
                        postCount++;
                    }

                }
            }

            if (commentList.Count != 0)
            {
                for (int j = 0; j < commentList.Count; j++)
                {
                    if (firstDay.Year + i == commentList[j].commentedAt.Year)
                    {
                        commentCount++;
                    }

                }

            }

            posts[i] = postCount;
            comments[i] = commentCount;
        }

        plt.PlotScatter(xs, posts, label: "Posts");
        plt.PlotScatter(xs, comments, label: "Comments");
        plt.Legend();
        plt.Ticks(xTickRotation: 45);
        plt.XTicks(xs, years);
        plt.SaveFig(savePath);


    }


}
