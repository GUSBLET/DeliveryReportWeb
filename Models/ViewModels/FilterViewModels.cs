using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models.ViewModels
{
    public class FilterUserMangeViewModels
    {
        public FilterUserMangeViewModels(List<string> roles, string role, string name)
        {
            roles.Insert(0, "All");
            Roles = new SelectList(roles, role);
            SelectedEmail = name;
        }
        public SelectList Roles { get; } // List roles

        public string SelectedEmail { get; } // Name entered
    }

    public class ReportFilterUserMangeViewModel
    {
        public ReportFilterUserMangeViewModel(string fullName, DateOnly date, string county)
        {
            SelectedFullName = fullName;
            SelectedDate = date;
            SelectedCounty = county;
        }
        public DateOnly SelectedDate { get; } // Choise role
        public string SelectedFullName { get; } // Choese name
        public string SelectedCounty { get; } // County entered
    }

}
