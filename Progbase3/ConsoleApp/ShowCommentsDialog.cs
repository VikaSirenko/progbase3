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
            allCommentsListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),

            };

            allCommentsListView.OpenSelectedItem += OnOpenComment;
            prevPageButton = new Button(28, 14, "<");
            nextPageButton = new Button(44, 14, ">");
            this.currentPageLbl = new Label(36, 14, "?");
            Label slash = new Label(38, 14, "/");
            this.allPagesLbl = new Label(40, 14, "?");

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


        }
        public void SetRepository(CommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
            ShowCurrentPage();
        }

        private void OnOpenComment(ListViewItemEventArgs args)
        {

            //TODO

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

    }
}
