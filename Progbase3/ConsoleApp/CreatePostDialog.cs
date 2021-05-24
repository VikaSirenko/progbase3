using Terminal.Gui;
using System;


public class CreatePostDialog : Dialog
{
    public bool canceled;
    protected TextView publicationTextInput;


    public CreatePostDialog()
    {
        this.Title = "Create post";
        Button okBtn = new Button("Ok");
        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCreateDialogCanceled;
        okBtn.Clicked += OnCreateButtonSubmit;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);
        int rightColumn = 25;
        int coordinateX = 2;

        Label publicationTextLbl = new Label(coordinateX, 2, "Enter text");
        publicationTextInput = new TextView()
        {
            X = rightColumn,
            Y = Pos.Top(publicationTextLbl),
            Width = 40,
            Height = 8,

        };
        this.Add(publicationTextInput, publicationTextLbl);


    }

    private void OnCreateDialogCanceled()
    {
        this.canceled = true;
        Application.RequestStop();
    }

    private void OnCreateButtonSubmit()
    {
        this.canceled = false;
        Application.RequestStop();
    }

    public Post GetPostFromFields()
    {
        Post post = new Post();
        if (!this.publicationTextInput.Text.IsEmpty)
        {
            post.publicationText = this.publicationTextInput.Text.ToString();
            post.publishedAt = DateTime.Now;
            return post;
        }

        return null;
    }
}
