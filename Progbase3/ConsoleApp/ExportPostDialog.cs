using Terminal.Gui;
using System.Collections.Generic;
public class ExportPostDialog : Dialog
{
    private TextField coincidenceInput;
    private TextField exportFileNameInput;
    private PostRepository postRepository;

    public ExportPostDialog()
    {
        this.Title = "Export posts";
        Button okBtn = new Button("Ok");
        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCancelClicked;
        okBtn.Clicked += OnOkClicked;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        Label coincidenceLbl = new Label(4, 4, "Enter text to filter:");
        coincidenceInput = new TextField()
        {
            X = 25,
            Y = Pos.Top(coincidenceLbl),
            Width = 40,
        };
        this.Add(coincidenceInput, coincidenceLbl);

        Label exportFileName = new Label(4, 8, "Export file name");
        exportFileNameInput = new TextField()
        {
            X = 25,
            Y = Pos.Top(exportFileName),
            Width = 40,
        };
        this.Add(exportFileName, exportFileNameInput);
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
        if (!coincidenceInput.Text.IsEmpty && !exportFileNameInput.Text.IsEmpty)
        {
            string outputFile = $"/home/vika/projects/progbase3/data/importAndExportFiles/{exportFileNameInput.Text}.xml";
            List<Post> filtredPosts = postRepository.FilterByPostText(coincidenceInput.Text.ToString());
            try
            {
                ExportAndImport.DoExport(filtredPosts, outputFile);
                MessageBox.Query("Exported", $"Data exported. File path:'{outputFile}'", "OK");
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
