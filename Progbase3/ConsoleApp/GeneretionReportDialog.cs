using Terminal.Gui;
using System.IO;
public class GeneretionReportDialog : Dialog
{

    private DateField startDate;
    private DateField endDate;
    private Label pathToFolder;
    private TextField nameOfImage;
    private TextField nameOfReport;
    private bool isGenereteImageClicked = false;
    private User currentUser;
    private PostRepository postRepository;
    private CommentRepository commentRepository;
    public GeneretionReportDialog()
    {
        this.Title = "Generete Report";
        Button okBtn = new Button(40, 18, "Generete report");
        Button cencelBtn = new Button(25, 18, "Cencel");
        okBtn.Clicked += OnGenereteReportClicked;
        cencelBtn.Clicked += OnCencelCkicked;
        this.Add(okBtn, cencelBtn);
        int rightColumn = 25;
        int coordinateX = 2;

        Label startLbl = new Label(coordinateX, 4, "Start date");
        startDate = new DateField()
        {
            X = rightColumn,
            Y = Pos.Top(startLbl),
            Width = 40,
        };
        this.Add(startLbl, startDate);

        Label endLbl = new Label(coordinateX, 6, "End date");
        endDate = new DateField()
        {
            X = rightColumn,
            Y = Pos.Top(endLbl),
            Width = 40,
        };
        this.Add(endLbl, endDate);

        Button selectedFolderBtn = new Button(coordinateX, 8, "Select a folder");
        selectedFolderBtn.Clicked += SelectFolder;

        pathToFolder = new Label("")
        {
            X = Pos.Right(selectedFolderBtn) + 2,
            Y = Pos.Top(selectedFolderBtn),
            Width = Dim.Fill(),
        };
        this.Add(selectedFolderBtn, pathToFolder);

        Label nameOfImageLbl = new Label(coordinateX, 10, "Image name:");
        nameOfImage = new TextField()
        {
            X = 25,
            Y = Pos.Top(nameOfImageLbl),
            Width = 40,
            Height = 8,

        };
        this.Add(nameOfImageLbl, nameOfImage);

        Button genereteImage = new Button(coordinateX, 12, "Generete image");
        genereteImage.Clicked += OnGenereteImage;
        this.Add(genereteImage);

        Label nameOfReportLbl = new Label(coordinateX, 14, "Report name:");
        nameOfReport = new TextField()
        {
            X = 25,
            Y = Pos.Top(nameOfReportLbl),
            Width = 40,
            Height = 8,

        };
        this.Add(nameOfReport, nameOfReportLbl);


    }

    public void SetData(User currentUser, PostRepository postRepository, CommentRepository commentRepository)
    {
        this.currentUser = currentUser;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    private void SelectFolder()
    {
        OpenDialog dialog = new OpenDialog("Open folder", "Open?");
        dialog.DirectoryPath = "/home/vika/projects/progbase3/data/reportData";
        Application.Run(dialog);

        if (!dialog.Canceled)
        {
            NStack.ustring filePath = dialog.FilePath;
            pathToFolder.Text = filePath;
        }
        else
        {
            pathToFolder.Text = "";
        }

    }

    private void OnGenereteImage()
    {
        if (!startDate.Text.IsEmpty && !endDate.Text.IsEmpty && !pathToFolder.Text.IsEmpty)
        {
            if (!File.Exists(pathToFolder.Text.ToString() + "/" + nameOfImage.Text.ToString() + ".png") && isGenereteImageClicked == false)
            {
                isGenereteImageClicked = true;
                //TODO generete
            }
            else
            {
                MessageBox.ErrorQuery("ERROR", "Image file already exists. Rename the file", "OK");
            }

        }
        else
        {
            MessageBox.ErrorQuery("Error", "Not all fields are filled", "OK");
        }

    }

    private void OnGenereteReportClicked()
    {
        string imagePath = pathToFolder.Text.ToString() + "/" + nameOfImage.Text.ToString() + ".png";
        string reportPath = pathToFolder.Text.ToString() + "/" + nameOfReport.Text.ToString() + ".docx";
        if (!startDate.Text.IsEmpty && !endDate.Text.IsEmpty && !pathToFolder.Text.IsEmpty)
        {
            try
            {
                if (!File.Exists(reportPath))
                {
                    ReportGeneration.GenereteReport(startDate.Text.ToString(), endDate.Text.ToString(),
                             imagePath, reportPath, postRepository, commentRepository, currentUser);
                    MessageBox.Query("Report generation", $"Report saved in: '{reportPath}'", "OK");
                    Application.RequestStop();
                }
                else
                {
                    MessageBox.ErrorQuery("ERROR", "Report file already exists. Rename the file", "OK");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("ERROR", $"Unable to save report: '{ex.Message.ToString()}'", "OK");
            }

        }
        else
        {
            MessageBox.ErrorQuery("Error", "Not all fields are filled", "OK");
        }

    }

    private void OnCencelCkicked()
    {
        Application.RequestStop();
    }
}
