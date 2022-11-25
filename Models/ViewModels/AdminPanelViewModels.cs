using Models.Entities;

namespace Models.ViewModels
{
    public class UserManagementViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
    public class IndexViewModel
    {
        public IEnumerable<Account> Users { get; }
        public PageViewModel PageViewModel { get; }
        public FilterUserMangeViewModels FilterViewModel { get; }
        public IndexViewModel(IEnumerable<Account> users, PageViewModel viewModel, FilterUserMangeViewModels mangeViewModels)
        {
            Users = users;
            PageViewModel = viewModel;
            FilterViewModel = mangeViewModels;
        }
    }
}
