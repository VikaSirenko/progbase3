using Terminal.Gui;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class ShowUsersDialog : Dialog
    {
        private ListView allUsersListView;

        private int pageLength = 10;
        private int currentPage = 1;
        private Label currentPageLbl;
        private Label allPagesLbl;
        private Button prevPageButton;
        private Button nextPageButton;
        private UserRepository userRepository;
        private Label isEmptyListLbl;
        public ShowUsersDialog()
        {
            this.Title = "Show users";
            allUsersListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),

            };

            allUsersListView.OpenSelectedItem += OnOpenUser;
            prevPageButton = new Button(28, 14, "<");
            nextPageButton = new Button(44, 14, ">");
            this.currentPageLbl = new Label(36, 14, "?");
            Label slash = new Label(38, 14, "/");
            this.allPagesLbl = new Label(40, 14, "?");

            nextPageButton.Clicked += OnNextButtonClicked;
            prevPageButton.Clicked += OnPrevButtonClicked;

            FrameView frameView = new FrameView("Users")
            {
                X = 4,
                Y = 2,
                Width = Dim.Fill() - 4,
                Height = pageLength + 2,
            };

            frameView.Add(allUsersListView);
            this.Add(frameView);

            isEmptyListLbl = new Label("There is no users.");
            frameView.Add(isEmptyListLbl);
            isEmptyListLbl.Visible = false;


        }
        public void SetRepository(UserRepository userRepository)
        {
            this.userRepository = userRepository;
            ShowCurrentPage();
        }

        private void OnOpenUser(ListViewItemEventArgs args)
        {

            //TODO

        }

        private void OnNextButtonClicked()
        {
            int totalPages = userRepository.GetTotalPages(pageLength);
            if (currentPage >= totalPages)
            {
                return;
            }

            this.currentPage++;
            ShowCurrentPage();

        }

        private void OnPrevButtonClicked()
        {

            int totalPages = userRepository.GetTotalPages(pageLength);
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
            int totalPages = userRepository.GetTotalPages(pageLength);

            if (totalPages == 0)
            {
                totalPages = 1;
            }

            this.allPagesLbl.Text = totalPages.ToString();

            this.allUsersListView.SetSource(userRepository.GetPageOfUsers(currentPage, pageLength));

            prevPageButton.Visible = (currentPage != 1);
            nextPageButton.Visible = (currentPage != int.Parse(this.allPagesLbl.Text.ToString()));

            if (userRepository.GetPageOfUsers(currentPage, pageLength).Count == 0)
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