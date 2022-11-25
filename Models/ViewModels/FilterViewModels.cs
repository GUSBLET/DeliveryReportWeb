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
        public ReportFilterUserMangeViewModel(string lastName ,string name,DateOnly date, string county)
        {
            List<string> days = new List<string>();
            List<string> months = new List<string>();
            List<string> years = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(i.ToString());   
            }
            for (int i = 1; i <= 31; i++)
            {
                days.Add(i.ToString());
            }
            for (int i = 2000; i <= 2040; i++)
            {
                years.Add(i.ToString());   
            }
            days.Insert(0, "Day");
            years.Insert(0, "Year");
            months.Insert(0, "Month");
            Years = new SelectList(years, date.Year);
            Months = new SelectList(months, date.Month);
            Days = new SelectList(days, date.Day);
            SelectedLastName = lastName;
            SelectedName = name;
            SelectedDate = date;
            SelectedCounty = county;
        }
        public SelectList Years { get; } // List years
        public SelectList Months { get; } // List months
        public SelectList Days { get; } // List days
        public DateOnly SelectedDate { get; } // Choise role
        public string SelectedName { get;} // Choese name
        public string SelectedLastName { get; } // Choese last name
        public string SelectedCounty { get; } // County entered
    }

}
