using Terminal.Gui;
using System.Collections.Generic;

public class DialogOfSelectedPostComments : Dialog
{
    private ListView allCommentsListView;
    private int pageLength = 10;
    private int currentPage = 1;
    private Label currentPageLbl;
    private Label allPagesLbl;
    private Button prevPageButton;
    private Button nextPageButton;
    private CommentRepository commentRepository;
    public Comment selectedComment;
    private Label isEmptyListLbl;
    private long selectedPostId;
    private User currentUser;

    public DialogOfSelectedPostComments()
    {
        this.Title = "Comments";
        allCommentsListView = new ListView(new List<Comment>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),

        };

        allCommentsListView.OpenSelectedItem += OnOpenCommentClicked;
        prevPageButton = new Button(22, 14, "<");
        nextPageButton = new Button(38, 14, ">");
        this.currentPageLbl = new Label(30, 14, "?");
        Label slash = new Label(32, 14, "/");
        this.allPagesLbl = new Label(34, 14, "?");
        this.Add(prevPageButton, nextPageButton, currentPageLbl, slash, allPagesLbl);


        Button backBtn = new Button(30, 20, "Back");
        this.Add(backBtn);
        backBtn.Clicked += OnBackBtnClicked;


        nextPageButton.Clicked += OnNextButtonClicked;
        prevPageButton.Clicked += OnPrevButtonClicked;

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

        Button commentedBtn = new Button(4, 18, "Leave a comment");
        commentedBtn.Clicked += OnLeaveComment;
        this.Add(commentedBtn);
    }

    private void OnOpenCommentClicked(ListViewItemEventArgs args)
    {
        Comment comment = (Comment)args.Value;
        OpenCommentDialog dialog = new OpenCommentDialog(currentUser, comment);
        dialog.SetData(comment);

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

    private void OnLeaveComment()
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
                comment.userId = currentUser.id;
                comment.postId = selectedPostId;

                long id = commentRepository.Insert(comment);
                comment.id = id;
                ShowCurrentPage();
            }
        }
    }

    private void OnBackBtnClicked()
    {
        Application.RequestStop();
    }

    public void SetData(CommentRepository commentRepository, long postId, User currentUser)
    {
        this.currentUser = currentUser;
        this.selectedPostId = postId;
        this.commentRepository = commentRepository;
        ShowCurrentPage();
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
        int totalPages = commentRepository.GetTotalPagesOfFilterComments(pageLength, selectedPostId);

        if (totalPages == 0)
        {
            totalPages = 1;
        }

        this.allPagesLbl.Text = totalPages.ToString();

        this.allCommentsListView.SetSource(commentRepository.GetPageOfCommentsOfSelectedPost(currentPage, pageLength, selectedPostId));

        prevPageButton.Visible = (currentPage != 1);
        nextPageButton.Visible = (currentPage != int.Parse(this.allPagesLbl.Text.ToString()));

        if (commentRepository.GetPageOfCommentsOfSelectedPost(currentPage, pageLength, selectedPostId).Count == 0)
        {
            isEmptyListLbl.Visible = true;
        }

        else
        {
            isEmptyListLbl.Visible = false;
        }

    }

}
