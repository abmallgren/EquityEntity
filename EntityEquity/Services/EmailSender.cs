using EntityEquity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EntityEquity.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private IConfiguration _configuration;
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private DbAppSettings _appSettings;
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
            ILogger<EmailSender> logger,
            IConfiguration configuration, 
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _configuration = configuration;
            _dbContextFactory = dbContextFactory;
            _appSettings = new(dbContextFactory);
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            //if (string.IsNullOrEmpty(Options.EmailApiKey))
            //{
            //    throw new Exception("Null EmailApiKey");
            //}
            await Execute(subject, message, toEmail);
        }

        public async Task Execute(string subject, string message, string toEmail)
        {
            string? from = await _appSettings.Get("NewsEmailAddress");
            string? password = await _appSettings.Get("NewsMailPassword");
            EmailService service = new EmailService(_configuration, _dbContextFactory);
            Email email = new Email()
            {
                From = from,
                To = toEmail,
                Subject = subject,
                HtmlContent = message
            };
            await service.SendEmail(email, password);

            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Emails.Add(email);
            await dbContext.SaveChangesAsync();
        }
    }
}
