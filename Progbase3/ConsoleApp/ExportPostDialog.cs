using Terminal.Gui;
using System.Collections.Generic;
public class ExportPostDialog : Dialog
{
    private TextField coincidenceInput;
    private PostRepository postRepository;
    private Label fileLabel;

    public ExportPostDialog()
    {
        this.Title = "Export posts";
        Button okBtn = new Button("Ok");
        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCancelClicked;
        okBtn.Clicked += OnOkClicked;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        Button selectedBtn = new Button(4, 8, "Open file");
        selectedBtn.Clicked += SelectFile;

        fileLabel = new Label("")
        {
            X = Pos.Right(selectedBtn) + 2,
            Y = Pos.Top(selectedBtn),
            Width = Dim.Fill(),
        };

        this.Add(selectedBtn, fileLabel);

        Label coincidenceLbl = new Label(4, 4, "Enter text to filter:");
        coincidenceInput = new TextField()
        {
            X = 25,
            Y = Pos.Top(coincidenceLbl),
            Width = 40,
        };
        this.Add(coincidenceInput, coincidenceLbl);


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
        this.DoExportPost();
        Application.RequestStop();
    }

    private void DoExportPost()
    {
        if (!coincidenceInput.Text.IsEmpty && !fileLabel.Text.IsEmpty)
        {
            List<Post> filtredPosts = postRepository.FilterByPostText(coincidenceInput.Text.ToString());
            try
            {
                ExportAndImport.DoExport(filtredPosts, fileLabel.Text.ToString());
                MessageBox.Query("Exported", $"Data exported. File path:'{fileLabel.Text.ToString()}'", "OK");
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

}
