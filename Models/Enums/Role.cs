namespace Models.Enums
{
    public enum Role
    {
        [Display(Name = "User")]
        User = 0,
        [Display(Name = "Deliveryman")]
        Deliveryman = 1,
        [Display(Name = "Manager")]
        Manager = 2,
        [Display(Name = "Admin")]
        Admin = 3,
    }
}
