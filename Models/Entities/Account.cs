namespace Models.Entities
{
    public class Account
    {
        public ulong Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public Guid EmailConfirmedToken { get; set; }
        public Role Role { get; set; }
    }
}
