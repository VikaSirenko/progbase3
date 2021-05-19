using Terminal.Gui;

namespace ConsoleApp
{
    public class OpenPostDialog : Dialog
    {
        public bool deleted;
        public bool updated;
        private Post post;
        private TextView publicationTextOutput;
        private Label publishedAtOutput;


        public Post GetPost()
        {
            return post;
        }

        public OpenPostDialog()
        {
            this.Title = "Open post";
            int rightColumn = 25;
            int coordinateX = 2;
            Button backBtn = new Button("Back");
            Button editBtn = new Button("Edit");
            Button deleteBtn = new Button("Delete");
            backBtn.Clicked += OnOpenDialogBack;
            editBtn.Clicked += OnOpenDialogEdit;
            deleteBtn.Clicked += OnOpenDialogDelete;
            this.AddButton(editBtn);
            this.AddButton(deleteBtn);
            this.AddButton(backBtn);

            Label publicationTextLbl = new Label(coordinateX, 2, "Publicatiion text:");
            publicationTextOutput = new TextView()
            {
                X = rightColumn,
                Y = Pos.Top(publicationTextLbl),
                Width = 40,
                Height = 8,
                ReadOnly = true,
            };
            this.Add(publicationTextLbl, publicationTextOutput);



            Label publisherAtLbl = new Label(coordinateX, 11, "Published at:");
            publishedAtOutput = new Label()
            {
                X = rightColumn,
                Y = Pos.Top(publisherAtLbl),
            };
            this.Add(publisherAtLbl, publishedAtOutput);



        }

        public void SetPost(Post post)
        {
            this.post = post;
            this.publicationTextOutput.Text = post.publicationText;
            this.publishedAtOutput.Text = post.publishedAt.ToString();

            //??? NEED TO DO output pinned comment text 
        }

        private void OnOpenDialogBack()
        {
            Application.RequestStop();
        }

        private void OnOpenDialogEdit()
        {
            EditPostDialog dialog = new EditPostDialog();
            dialog.SetPost(this.post);
            Application.Run(dialog);
            if (dialog.canceled == false)
            {
                Post updatedPost = dialog.GetPostFromFields();
                if (updatedPost == null)
                {
                    this.updated = false;
                }
                else
                {
                    updatedPost.publishedAt = post.publishedAt;
                    updatedPost.userId = post.userId;
                    updatedPost.pinCommentId = post.pinCommentId;
                    this.SetPost(updatedPost);
                    this.updated = true;
                }
            }
        }

        private void OnOpenDialogDelete()
        {
            int index = MessageBox.Query("Delete post", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                Application.RequestStop();
                deleted = true;
            }
        }


    }
}