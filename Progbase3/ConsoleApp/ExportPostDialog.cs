using Terminal.Gui;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
public class ExportPostDialog : Dialog
{
    private TextField coincidenceInput;
    private PostRepository postRepository;
    private Label pathToFolder;
    private TextField nameOfZip;


    public ExportPostDialog()
    {
        this.Title = "Export posts";
        Button okBtn = new Button("Ok");
        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCancelClicked;
        okBtn.Clicked += OnOkClicked;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        Button selectedFolderBtn = new Button(4, 12, "Select a folder");
        selectedFolderBtn.Clicked += SelectFolder;

        pathToFolder = new Label("")
        {
            X = Pos.Right(selectedFolderBtn) + 2,
            Y = Pos.Top(selectedFolderBtn),
            Width = Dim.Fill(),
        };

        Label nameOfZipLbl = new Label(4, 8, "Archive name");
        nameOfZip = new TextField()
        {
            X = 25,
            Y = Pos.Top(nameOfZipLbl),
            Width = 40,
            Height = 8,

        };
        this.Add(nameOfZipLbl, nameOfZip);


        this.Add(selectedFolderBtn, pathToFolder);

        Label coincidenceLbl = new Label(4, 4, "Enter text to filter:");
        coincidenceInput = new TextField()
        {
            X = 25,
            Y = Pos.Top(coincidenceLbl),
            Width = 40,
        };
        this.Add(coincidenceInput, coincidenceLbl);


    }

    private void SelectFolder()
    {
        OpenDialog dialog = new OpenDialog("Open folder", "Open?");
        dialog.DirectoryPath = "/home/vika/projects/progbase3/data/importAndExportFiles";
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

    public void SetData(PostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    private void OnCancelClicked()
    {
        Application.RequestStop();
    }

    private void OnOkClicked()
    {
        this.DoExport();
        Application.RequestStop();
    }

    private void DoExport()
    {
        if (!coincidenceInput.Text.IsEmpty && !pathToFolder.Text.IsEmpty && !nameOfZip.Text.IsEmpty)
        {
            List<Post> filtredPosts = postRepository.FilterByPostText(coincidenceInput.Text.ToString());
            try
            {
                ExportAndImport.DoExportOfPosts(filtredPosts, pathToFolder.Text.ToString() + "/posts.xml");
                List<Comment> filtredComments = FilterComments(filtredPosts);
                ExportAndImport.DoExportOfComments(filtredComments, pathToFolder.Text.ToString() + "/comments.xml");
                this.PerformArchiving();

            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("ERROR", $"Data cannot be exported:'{ex.Message.ToString()}'", "OK");
            }

        }
        else
        {
            MessageBox.ErrorQuery("ERROR", "Fields are not filled", "OK");
        }

    }

    private List<Comment> FilterComments(List<Post> filtredPosts)
    {
        List<Comment> filtredComments = new List<Comment>();
        for (int i = 0; i < filtredPosts.Count; i++)
        {
            for (int j = 0; j < filtredPosts[i].comments.Count; j++)
            {
                filtredComments.Add(filtredPosts[i].comments[j]);
            }
        }
        return filtredComments;
    }

    private void PerformArchiving()
    {
        string startPath = pathToFolder.Text.ToString();
        string resultPath = "/home/vika/projects/progbase3/data/importAndExportFiles/" + nameOfZip.Text.ToString() + ".zip";
        if (!File.Exists(resultPath))
        {
            ZipFile.CreateFromDirectory(startPath, resultPath);
            MessageBox.Query("Exported", $"Data exported. File path:'{resultPath}'", "OK");
        }
        else
        {
            MessageBox.ErrorQuery("ERROR", "File already exists. Rename the file", "OK");
        }
    }




}
