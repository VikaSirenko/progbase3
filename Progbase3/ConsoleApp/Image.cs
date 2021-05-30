using System;

public static class Image
{
    public static void GenereteImage(int countOfPosts, int countOfComments, DateTime firstDay, string savePath)
    {
        var plt = new ScottPlot.Plot(600, 400);

        double[] datesOfPosts = new double[countOfPosts];
        if (countOfPosts != 0)
        {
            for (int i = 0; i < countOfPosts; i++)
                datesOfPosts[i] = firstDay.AddDays(i).ToOADate();
        }

        double[] valuesOfPosts = new double[countOfPosts];
        Random rand = new Random(0);
        if (countOfPosts != 0)
        {
            for (int i = 1; i < countOfPosts; i++)
                valuesOfPosts[i] = valuesOfPosts[i - 1] + rand.NextDouble();
        }

        double[] datesOfComments = new double[countOfComments];
        if (countOfComments != 0)
        {
            for (int i = 0; i < countOfComments; i++)
                datesOfComments[i] = firstDay.AddDays(i).ToOADate();
        }

        double[] valuesOfComments = new double[countOfComments];
        if (countOfComments != 0)
        {
            for (int i = 1; i < countOfComments; i++)
                valuesOfComments[i] = valuesOfComments[i - 1] + rand.NextDouble();
        }
        plt.AddScatter(datesOfPosts, valuesOfPosts);
        plt.AddScatter(datesOfComments, valuesOfComments);
        plt.XAxis.DateTimeFormat(true);

        plt.XAxis.ManualTickSpacing(1, ScottPlot.Ticks.DateTimeUnit.Day);
        plt.XAxis.TickLabelStyle(rotation: 45);


        plt.XAxis.SetSizeLimit(min: 50);


        plt.SaveFig(savePath);





    }
}
