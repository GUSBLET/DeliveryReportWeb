namespace UI
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository<Account>, AccountRepository>();
            services.AddScoped<IBaseRepository<ReportOfDelivary>, ReportPanelRepository>();
        }
        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IReportPanelService, ReportPanelService>();
            services.AddScoped<IAdminPanelService, AdminPanelService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddTransient<IMailService, MailService>();
        }

    }
}
