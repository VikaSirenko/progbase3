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
        private TextField pinnedComment;
        private CommentRepository commentRepository;
        private Comment comment;


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



            Label publisherAtLbl = new Label(coordinateX, 10, "Published at:");
            publishedAtOutput = new Label()
            {
                X = rightColumn,
                Y = Pos.Top(publisherAtLbl),
            };
            this.Add(publisherAtLbl, publishedAtOutput);

            Label pinnedComLbl = new Label(coordinateX, 12, "Pin comment");
            pinnedComment = new TextField()
            {
                X = rightColumn,
                Y = Pos.Top(pinnedComLbl),
                Width = 40,
                Height = 8,
                ReadOnly = true,
            };
            this.Add(pinnedComLbl, pinnedComment);

        }

        public void SetPost(Post post, CommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
            this.post = post;
            this.publicationTextOutput.Text = post.publicationText;
            this.publishedAtOutput.Text = post.publishedAt.ToString();
            if (post.pinCommentId != default)
            {
                this.comment = commentRepository.GetByCommentId(post.pinCommentId);
                this.pinnedComment.Text = this.comment.commentText;
            }
            else
            {
                pinnedComment.Text = "None";
            }
        }

        private void OnOpenDialogBack()
        {
            Application.RequestStop();
        }

        private void OnOpenDialogEdit()
        {
            EditPostDialog dialog = new EditPostDialog();
            dialog.SetPost(this.post, commentRepository);
            Application.Run(dialog);
            if (dialog.canceled == false)
            {
                Post updatedPost = dialog.GetPostFromFields();
                if (dialog.selectedComment != null)
                {
                    updatedPost.pinCommentId = dialog.selectedComment.id;
                    this.comment = dialog.selectedComment;
                }

                if (updatedPost == null)
                {
                    this.updated = false;
                }
                else
                {
                    updatedPost.publishedAt = post.publishedAt;
                    updatedPost.userId = post.userId;
                    updatedPost.id=post.id;
                    this.SetPost(updatedPost, commentRepository);
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