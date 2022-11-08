using MimeKit;
using MailKit.Net.Smtp;

namespace BusinessLogic.Implemantations
{
    public class MailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailService> _loger;
        private readonly string _from = "pizzalog711@gmail.com";
        private readonly string _token;

        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            _configuration = configuration;
            _loger = logger;
            _token = _configuration.GetValue<string>("TokenMailService");
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Admin of website", _from));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, true);
                await client.AuthenticateAsync(_from, _token);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }


    }
}
