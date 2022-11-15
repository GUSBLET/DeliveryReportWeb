namespace DataAccessLayer.EF
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Account> TableUsers { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>(buildAction =>
            {
                buildAction.ToTable("TableAccountUsers")
                    .HasKey(x => x.Id);
                buildAction.Property(x => x.Id)
                    .ValueGeneratedOnAdd();
                buildAction.Property(x => x.Email)
                    .IsRequired();
                buildAction.Property(x => x.Password)
                    .IsRequired();
                buildAction.Property(x => x.Name)
                    .IsRequired(false);
                buildAction.Property(x => x.LastName)
                    .IsRequired(false);
                buildAction.Property(x => x.PhoneNumber)
                    .IsRequired(false);
                buildAction.Property(x => x.EmailConfirmed);
                buildAction.Property(x => x.EmailConfirmedToken)
                    .IsRequired();
                buildAction.Property(x => x.Role)
                    .IsRequired();

                buildAction.HasData(new Account
                {
                    Id = 1,
                    Email = "pizzalog711@gmail.com",
                    Password = HashPasswordHelper.HashPassowrd("12345678"),
                    Name = "Denis",
                    LastName = "Chykalov",
                    PhoneNumber = "+380 50 268 9846",
                    EmailConfirmed = true,
                    EmailConfirmedToken = Guid.NewGuid(),
                    Role = Models.Enums.Role.Admin
                });
            });
        }
    }
}
