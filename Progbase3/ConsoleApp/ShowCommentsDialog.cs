using Terminal.Gui;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class ShowCommentsDialog : Dialog
    {
        private ListView allCommentsListView;

        private int pageLength = 10;
        private int currentPage = 1;
        private Label currentPageLbl;
        private Label allPagesLbl;
        private Button prevPageButton;
        private Button nextPageButton;
        private CommentRepository commentRepository;
        private Label isEmptyListLbl;
        public ShowCommentsDialog()
        {
            this.Title = "Show comments";
            allCommentsListView = new ListView(new List<Comment>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),

            };


            Button createCommentBtn = new Button(4, 18, "Create comment");
            createCommentBtn.Clicked += OnCreateCommentClicked;
            this.Add(createCommentBtn);

            allCommentsListView.OpenSelectedItem += OnOpenComment;
            prevPageButton = new Button(22, 14, "<");
            nextPageButton = new Button(38, 14, ">");
            this.currentPageLbl = new Label(30, 14, "?");
            Label slash = new Label(32, 14, "/");
            this.allPagesLbl = new Label(34, 14, "?");

            nextPageButton.Clicked += OnNextButtonClicked;
            prevPageButton.Clicked += OnPrevButtonClicked;

            this.Add(prevPageButton, nextPageButton, currentPageLbl, allPagesLbl, slash);


            FrameView frameView = new FrameView("Comments")
            {
                X = 4,
                Y = 2,
                Width = Dim.Fill() - 4,
                Height = pageLength + 2,
            };

            frameView.Add(allCommentsListView);
            this.Add(frameView);

            isEmptyListLbl = new Label("There is no comments.");
            frameView.Add(isEmptyListLbl);
            isEmptyListLbl.Visible = false;

            Button backBtn = new Button(30, 21, "Back");
            this.Add(backBtn);
            backBtn.Clicked += OnOkButtonClicked;


        }

        private void OnOkButtonClicked()
        {
            Application.RequestStop();
        }

        private void OnCreateCommentClicked()
        {

            CreateCommentDialog dialog = new CreateCommentDialog();
            Application.Run(dialog);

            if (dialog.canceled == false)
            {
                Comment comment = dialog.GetCommentFromFields();
                if (comment == null)
                {
                    MessageBox.ErrorQuery("Create comment", "Can not create comment.\nAll fields must be filled in the correct format", "OK");
                }

                else
                {
                    comment.userId = default; //TODO
                    comment.postId = default; //TODO

                    long id = commentRepository.Insert(comment);
                    comment.id = id;
                    ShowCurrentPage();
                }
            }

        }

        public void SetRepository(CommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
            ShowCurrentPage();
        }

        private void OnOpenComment(ListViewItemEventArgs args)
        {

            Comment comment = (Comment)args.Value;
            OpenCommentDialog dialog = new OpenCommentDialog();
            dialog.SetComment(comment);

            Application.Run(dialog);

            if (dialog.deleted == true)
            {
                ProcessDeleteComment(comment);

            }

            else if (dialog.updated == true)
            {
                ProcessEditComment(dialog, comment);
            }


        }

        private void OnNextButtonClicked()
        {
            int totalPages = commentRepository.GetTotalPages(pageLength);
            if (currentPage >= totalPages)
            {
                return;
            }

            this.currentPage++;
            ShowCurrentPage();

        }

        private void OnPrevButtonClicked()
        {

            int totalPages = commentRepository.GetTotalPages(pageLength);
            if (currentPage <= 1)
            {
                return;
            }

            this.currentPage--;
            ShowCurrentPage();
        }

        private void ShowCurrentPage()
        {
            this.currentPageLbl.Text = currentPage.ToString();
            int totalPages = commentRepository.GetTotalPages(pageLength);

            if (totalPages == 0)
            {
                totalPages = 1;
            }

            this.allPagesLbl.Text = totalPages.ToString();

            this.allCommentsListView.SetSource(commentRepository.GetPageOfComments(currentPage, pageLength));

            prevPageButton.Visible = (currentPage != 1);
            nextPageButton.Visible = (currentPage != int.Parse(this.allPagesLbl.Text.ToString()));

            if (commentRepository.GetPageOfComments(currentPage, pageLength).Count == 0)
            {
                isEmptyListLbl.Visible = true;
            }

            else
            {
                isEmptyListLbl.Visible = false;
            }

        }

        private void ProcessDeleteComment(Comment comment)
        {
            bool isDeleted = commentRepository.Delete(comment.id);
            if (isDeleted)
            {
                int countOfPages = commentRepository.GetTotalPages(pageLength);
                if (currentPage > countOfPages && currentPage > 1)
                {
                    currentPage--;
                }
                ShowCurrentPage();
            }

            else
            {
                MessageBox.ErrorQuery("Delete comment", "Can not delete comment.", "OK");
            }

        }

        private void ProcessEditComment(OpenCommentDialog dialog, Comment comment)
        {
            Comment updatedComment = dialog.GetComment();
            if (commentRepository.Update(updatedComment, comment.id))
            {
                ShowCurrentPage();
            }

            else
            {
                MessageBox.ErrorQuery("Edit comment", "Can not edit comment.", "OK");
            }

        }

    }
}
