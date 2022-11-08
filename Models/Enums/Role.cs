namespace Models.Enums
{
    public enum Role
    {
        [Display(Name = "User")]
        User = 0,
        [Display(Name = "Deliveryman")]
        Moderator = 1,
        [Display(Name = "Admin")]
        Admin = 2,
    }
}
