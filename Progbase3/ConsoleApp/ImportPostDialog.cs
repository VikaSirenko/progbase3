using Terminal.Gui;
using System.IO;

public class ImportPostDialog : Dialog
{
    private PostRepository postRepository;
    private CommentRepository commentRepository;
    private Label fileLabel;

    public ImportPostDialog()
    {
        this.Title = "Import posts";
        Button selectedBtn = new Button(4, 4, "Open folder");
        selectedBtn.Clicked += SelectFile;

        fileLabel = new Label("")
        {
            X = Pos.Right(selectedBtn) + 2,
            Y = Pos.Top(selectedBtn),
            Width = Dim.Fill(),
        };

        this.Add(selectedBtn, fileLabel);

        Button importButton = new Button(4, 6, "Do import from folder");
        importButton.Clicked += DoImportPosts;
        this.Add(importButton);

        Button cancelBtn = new Button(15, 12, "Cancel");
        cancelBtn.Clicked += OnCancelClicked;
        this.Add(cancelBtn);

    }

    private void OnCancelClicked()
    {
        Application.RequestStop();
    }

    private void DoImportPosts()
    {
        if (File.Exists(fileLabel.Text.ToString() + "/posts.xml") && File.Exists(fileLabel.Text.ToString() + "/comments.xml") && !fileLabel.Text.IsEmpty)
        {
            try
            {
                ExportAndImport.DoImportOfPosts(fileLabel.Text.ToString() + "/posts.xml", postRepository);
                ExportAndImport.DoImportOfComments(fileLabel.Text.ToString() + "/comments.xml", commentRepository);
                MessageBox.Query("Import posts and comments", $"Posts and comments are imported", "OK");
                Application.RequestStop();
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("ERROR", $"Unable to read data from file: [{fileLabel.Text.ToString()}].  " + ex.Message.ToString(), "OK");
            }

        }
        else
        {
            MessageBox.ErrorQuery("ERROR", $"Unable to read data from file: [{fileLabel.Text.ToString()}] ", "OK");
        }

    }

    private void SelectFile()
    {
        OpenDialog dialog = new OpenDialog("Open XML file", "Open?");
        dialog.DirectoryPath = "/home/vika/projects/progbase3/data/importAndExportFiles";
        Application.Run(dialog);

        if (!dialog.Canceled)
        {
            NStack.ustring filePath = dialog.FilePath;
            fileLabel.Text = filePath;
        }
        else
        {
            fileLabel.Text = "";
        }

    }


    public void SetData(PostRepository postRepository, CommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }



}
