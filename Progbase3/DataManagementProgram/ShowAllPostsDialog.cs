using Terminal.Gui;
using System.Collections.Generic;

public class ShowAllPostsDialog : Dialog
{
    private ListView allPostsListView;
    private int pageLength = 10;
    private int currentPage = 1;
    private Label currentPageLbl;
    private Label allPagesLbl;
    private Button prevPageButton;
    private Button nextPageButton;
    private PostRepository postRepository;
    private CommentRepository commentRepository;
    private Label isEmptyListLbl;
    private User currentUser;
    public ShowAllPostsDialog(User currentUser)
    {
        this.currentUser = currentUser;
        this.Title = "Show posts";
        allPostsListView = new ListView(new List<Post>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),

        };
        Button backBtn = new Button(30, 21, "Back");
        backBtn.Clicked += OnShowDialogBack;
        this.Add(backBtn);


        Button createPostBtn = new Button(4, 18, "Create post");
        createPostBtn.Clicked += OnCreatePostClicked;
        this.Add(createPostBtn);


        allPostsListView.OpenSelectedItem += OnOpenPost;
        prevPageButton = new Button(22, 14, "<");
        nextPageButton = new Button(38, 14, ">");
        this.currentPageLbl = new Label(30, 14, "?");
        Label slash = new Label(32, 14, "/");
        this.allPagesLbl = new Label(34, 14, "?");


        nextPageButton.Clicked += OnNextButtonClicked;
        prevPageButton.Clicked += OnPrevButtonClicked;
        this.Add(prevPageButton, nextPageButton, currentPageLbl, allPagesLbl, slash);


        FrameView frameView = new FrameView("Posts")
        {
            X = 4,
            Y = 2,
            Width = Dim.Fill() - 4,
            Height = pageLength + 2,
        };

        frameView.Add(allPostsListView);
        this.Add(frameView);

        isEmptyListLbl = new Label("There is no posts.");
        frameView.Add(isEmptyListLbl);
        isEmptyListLbl.Visible = false;

    }

    private void OnCreatePostClicked()
    {

        CreatePostDialog dialog = new CreatePostDialog();
        Application.Run(dialog);

        if (dialog.canceled == false)
        {
            Post post = dialog.GetPostFromFields();
            if (post == null)
            {
                MessageBox.ErrorQuery("Create post", "Can not create post.\nAll fields must be filled in the correct format", "OK");
            }

            else
            {
                post.pinCommentId = default;
                post.userId = currentUser.id;

                long id = postRepository.Insert(post);
                post.id = id;
                ShowCurrentPage();
            }
        }

    }
    private void OnShowDialogBack()
    {
        Application.RequestStop();
    }

    public void SetData(PostRepository postRepository, CommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        ShowCurrentPage();
    }

    private void OnOpenPost(ListViewItemEventArgs args)
    {
        Post post = (Post)args.Value;
        OpenPostDialog dialog = new OpenPostDialog(currentUser, post);
        dialog.SetData(post, commentRepository);

        Application.Run(dialog);

        if (dialog.deleted == true)
        {
            ProcessDeletePost(post);

        }

        else if (dialog.updated == true)
        {
            ProcessEditPost(dialog, post);
        }

    }

    private void OnNextButtonClicked()
    {
        int totalPages = postRepository.GetTotalPages(pageLength);
        if (currentPage >= totalPages)
        {
            return;
        }

        this.currentPage++;
        ShowCurrentPage();

    }

    private void OnPrevButtonClicked()
    {

        int totalPages = postRepository.GetTotalPages(pageLength);
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
        int totalPages = postRepository.GetTotalPages(pageLength);

        if (totalPages == 0)
        {
            totalPages = 1;
        }

        this.allPagesLbl.Text = totalPages.ToString();

        this.allPostsListView.SetSource(postRepository.GetPageOfPosts(currentPage, pageLength));

        prevPageButton.Visible = (currentPage != 1);
        nextPageButton.Visible = (currentPage != int.Parse(this.allPagesLbl.Text.ToString()));

        if (postRepository.GetPageOfPosts(currentPage, pageLength).Count == 0)
        {
            isEmptyListLbl.Visible = true;
        }

        else
        {
            isEmptyListLbl.Visible = false;
        }

    }

    private void ProcessDeletePost(Post post)
    {
        bool isDeleted = postRepository.Delete(post.id);
        if (isDeleted)
        {
            commentRepository.DeleteAllByPostId(post.id);
            int countOfPages = postRepository.GetTotalPages(pageLength);
            if (currentPage > countOfPages && currentPage > 1)
            {
                commentRepository.DeleteAllByPostId(post.id);
                currentPage--;
            }
            ShowCurrentPage();
        }

        else
        {
            MessageBox.ErrorQuery("Delete post", "Can not delete post.", "OK");
        }

    }

    private void ProcessEditPost(OpenPostDialog dialog, Post post)
    {
        Post updatedPost = dialog.GetPost();
        if (postRepository.Update(updatedPost, post.id))
        {
            ShowCurrentPage();
        }

        else
        {
            MessageBox.ErrorQuery("Edit post", "Can not edit post.", "OK");
        }

    }

}

