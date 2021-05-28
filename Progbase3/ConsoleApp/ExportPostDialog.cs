using Terminal.Gui;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
public class ExportPostDialog : Dialog
{
    private TextField coincidenceInput;
    private PostRepository postRepository;
    private Label pathToFolder;
    private TextField nameOfZip;
    private ListView allPostsListView;
    private int pageLength = 10;
    private int currentPage = 1;
    private Label currentPageLbl;
    private Label allPagesLbl;
    private Button prevPageButton;
    private Button nextPageButton;
    private CommentRepository commentRepository;
    private Label isEmptyListLbl;
    private User currentUser;
    private bool isEnterPressed;



    public ExportPostDialog()
    {
        this.Title = "Export posts";
        Button okBtn = new Button("Do export");
        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCancelClicked;
        okBtn.Clicked += OnOkClicked;
        this.AddButton(cancelBtn);
        this.AddButton(okBtn);

        allPostsListView = new ListView(new List<Post>())
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),

        };

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

        Button selectedFolderBtn = new Button(4, 22, "Select a folder");
        selectedFolderBtn.Clicked += SelectFolder;

        pathToFolder = new Label("")
        {
            X = Pos.Right(selectedFolderBtn) + 2,
            Y = Pos.Top(selectedFolderBtn),
            Width = Dim.Fill(),
        };

        Label nameOfZipLbl = new Label(4, 20, "Archive name:");
        nameOfZip = new TextField()
        {
            X = 25,
            Y = Pos.Top(nameOfZipLbl),
            Width = 40,
            Height = 8,

        };
        this.Add(nameOfZipLbl, nameOfZip);


        this.Add(selectedFolderBtn, pathToFolder);

        Label coincidenceLbl = new Label(4, 18, "Enter text to filter:");
        coincidenceInput = new TextField()
        {
            X = 25,
            Y = Pos.Top(coincidenceLbl),
            Width = 40,
        };
        coincidenceInput.KeyPress += OnSearchEnter;
        this.Add(coincidenceInput, coincidenceLbl);


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

    private void OnSearchEnter(KeyEventEventArgs args)
    {
        if (args.KeyEvent.Key == Key.Enter)
        {
            isEnterPressed = true;
            ShowCurrentPage();
        }

    }

    private void SelectFolder()
    {
        OpenDialog dialog = new OpenDialog("Open folder", "Open?");
        dialog.DirectoryPath = "/home/vika/projects/progbase3/data/importAndExportFiles";
        Application.Run(dialog);

        if (!dialog.Canceled)
        {
            NStack.ustring filePath = dialog.FilePath;
            pathToFolder.Text = filePath;
        }
        else
        {
            pathToFolder.Text = "";
        }
    }

    public void SetData(PostRepository postRepository, CommentRepository commentRepository, User currentUser)
    {
        this.postRepository = postRepository;
        this.currentUser = currentUser;
        this.commentRepository = commentRepository;
        ShowCurrentPage();

    }

    private void OnCancelClicked()
    {
        Application.RequestStop();
    }

    private void OnOkClicked()
    {
        this.DoExport();
        Application.RequestStop();
    }

    private void DoExport()
    {
        if (!coincidenceInput.Text.IsEmpty && !pathToFolder.Text.IsEmpty && !nameOfZip.Text.IsEmpty)
        {
            List<Post> filtredPosts = postRepository.GetListOfFilteredPosts(coincidenceInput.Text.ToString());
            try
            {
                ExportAndImport.DoExportOfPosts(filtredPosts, pathToFolder.Text.ToString() + "/posts.xml");
                List<Comment> filtredComments = new List<Comment>();
                for (int i = 0; i < filtredPosts.Count; i++)
                {
                    filtredComments = commentRepository.GetAllFiltredComments(filtredPosts[i].id, filtredComments);
                }
                ExportAndImport.DoExportOfComments(filtredComments, pathToFolder.Text.ToString() + "/comments.xml");
                this.PerformArchiving();

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

    private void PerformArchiving()
    {
        string startPath = pathToFolder.Text.ToString();
        string resultPath = "/home/vika/projects/progbase3/data/importAndExportFiles/" + nameOfZip.Text.ToString() + ".zip";
        if (!File.Exists(resultPath))
        {
            ZipFile.CreateFromDirectory(startPath, resultPath);
            MessageBox.Query("Exported", $"Data exported. File path:'{resultPath}'", "OK");
        }
        else
        {
            MessageBox.ErrorQuery("ERROR", "File already exists. Rename the file", "OK");
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
        if (isEnterPressed == false)
        {
            int totalPages = postRepository.GetTotalPages(pageLength);

            if (totalPages == 0)
            {
                totalPages = 1;
            }

            this.allPagesLbl.Text = totalPages.ToString();

            this.allPostsListView.SetSource(postRepository.GetPageOfPosts(currentPage, pageLength));


            if (postRepository.GetPageOfPosts(currentPage, pageLength).Count == 0)
            {
                isEmptyListLbl.Visible = true;
            }

            else
            {
                isEmptyListLbl.Visible = false;
            }
        }
        else
        {
            List<Post> filtredPosts = postRepository.GetListOfFilteredPosts(coincidenceInput.Text.ToString());
            int totalPages = (int)System.Math.Ceiling(filtredPosts.Count / (double)pageLength);

            if (totalPages == 0)
            {
                totalPages = 1;
            }

            this.allPagesLbl.Text = totalPages.ToString();

            this.allPostsListView.SetSource(postRepository.GetPageOfFilteredPosts(coincidenceInput.Text.ToString(), currentPage, pageLength));


            if (postRepository.GetPageOfFilteredPosts(coincidenceInput.Text.ToString(), currentPage, pageLength).Count == 0)
            {
                isEmptyListLbl.Visible = true;
            }

            else
            {
                isEmptyListLbl.Visible = false;
            }

        }

        prevPageButton.Visible = (currentPage != 1);
        nextPageButton.Visible = (currentPage != int.Parse(this.allPagesLbl.Text.ToString()));

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
