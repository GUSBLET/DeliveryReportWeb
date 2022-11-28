namespace Models.ViewModels
{
    public class ControlReportViewModal
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public ulong ReportId { get; set; }
        public string County { get; set; }
        public string FullName { get; set; }
        public int DistancePassed { get; set; }
        public DateOnly ReportDate { get; set; }
        public TimeOnly BeginTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public TimeOnly WorkingTime { get; set; }
    }

    public class ReportViewDataViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "CountyLabel")]
        public string County { get; set; }

        [Display(Name = "WorkingTimeLabel")]
        [DataType(DataType.Time)]
        public TimeOnly WorkingTime { get; set; }

        [Display(Name = "DistancePassedLabel")]
        [DataType(DataType.Text)]
        public int DistancePassed { get; set; }

        [Display(Name = "BeginTimeLabel")]
        [DataType(DataType.Time)]
        public TimeOnly BeginTime { get; set; }

        [Display(Name = "EndTimeLabel")]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        [Display(Name = "FullNameLabel")]
        [DataType(DataType.Text)]
        public string FullName { get; set; }

        [Display(Name = "UserIdLabel")]
        public ulong UserId { get; set; }

        [Display(Name = "ReportIdLabel")]
        public ulong ReportId { get; set; }
    }

    public class CreateReportViewModel
    {
        public ulong UserId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "CountyLabel")]
        [Required(ErrorMessage = "FieldErrorRequired")]
        public string County { get; set; }

        [Display(Name = "DistancePassedLabel")]
        [Required(ErrorMessage = "FieldErrorRequired")]
        [DataType(DataType.Duration)]
        public int DistancePassed { get; set; }

        [DataType(DataType.Date)]
        public DateOnly ReportDate { get; set; }

        [Display(Name = "BeginTimeLabel")]
        [Required(ErrorMessage = "FieldErrorRequired")]
        [DataType(DataType.Time)]
        public TimeOnly BeginTime { get; set; }

        [Display(Name = "EndTimeLabel")]
        [Required(ErrorMessage = "FieldErrorRequired")]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        [Display(Name = "WorkingTimeLabel")]
        [Required(ErrorMessage = "FieldErrorRequired")]
        [DataType(DataType.Time)]
        public TimeOnly WorkingTime { get; set; }
    }

    public class ViewReportsViewModel
    {
        public IEnumerable<ReportOfDelivary> Reports { get; }
        public PageViewModel PageViewModel { get; }
        public ReportFilterUserMangeViewModel DelivarymanReportFilterViewModel { get; }
        public ViewReportsViewModel(
            IEnumerable<ReportOfDelivary> reports,
            PageViewModel viewModel,
            ReportFilterUserMangeViewModel mangeViewModels)
        {
            Reports = reports;
            PageViewModel = viewModel;
            DelivarymanReportFilterViewModel = mangeViewModels;
        }
    }

    public class ControlViewReportsViewModel
    {
        public IEnumerable<ControlReportViewModal> Reports { get; }
        public PageViewModel PageViewModel { get; }
        public ReportFilterUserMangeViewModel ReportFilterViewModel { get; }
        public ControlViewReportsViewModel(
            IEnumerable<ControlReportViewModal> reports,
            PageViewModel viewModel,
            ReportFilterUserMangeViewModel mangeViewModels)
        {
            Reports = reports;
            PageViewModel = viewModel;
            ReportFilterViewModel = mangeViewModels;
        }
    }
}
