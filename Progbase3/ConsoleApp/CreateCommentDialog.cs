using Terminal.Gui;
using System;

namespace ConsoleApp
{
    public class CreateCommentDialog : Dialog
    {

        public bool canceled;
        protected TextView commentTextInput;

        public CreateCommentDialog()
        {
            this.Title = "Create comment";
            Button okBtn = new Button("Ok");
            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;
            okBtn.Clicked += OnCreateButtonSubmit;
            this.AddButton(cancelBtn);
            this.AddButton(okBtn);
            int rightColumn = 25;
            int coordinateX = 2;

            Label commentTextLbl = new Label(coordinateX, 2, "Enter text");
            commentTextInput = new TextView()
            {
                X = rightColumn,
                Y = Pos.Top(commentTextLbl),
                Width = 40,
                Height = 8,

            };
            this.Add(commentTextInput, commentTextLbl);


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

        public Comment GetCommentFromFields()
        {
            Comment comment = new Comment();
            if (!this.commentTextInput.Text.IsEmpty)
            {
                comment.commentText = this.commentTextInput.Text.ToString();
                comment.commentedAt = DateTime.Now;
                return comment;
            }

            return null;
        }

    }
}