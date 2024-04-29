using EntityEquity.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace EntityEquity.Services
{
    public class EmailService
    {
        private IConfiguration _configuration { get; set; }
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private DbAppSettings _appSettings;
        public EmailService(IConfiguration configuration, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _configuration = configuration;
            _dbContextFactory = dbContextFactory;
            _appSettings = new(_dbContextFactory);
        }
        public async Task SendEmail(Email email, string password)
        {
            SmtpClient client = new SmtpClient(await _appSettings.Get("SmtpAddress"));
            client.UseDefaultCredentials = false;
            NetworkCredential credential = new(email.From, password);
            MailAddress from = new MailAddress(email.From);
            MailAddress to = new MailAddress(email.To);
            MailMessage mail = new MailMessage(from, to);

            mail.Subject = email.Subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;

            mail.Body = email.HtmlContent;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            mail.IsBodyHtml = true;
            client.EnableSsl = true;
            client.Port = 465;
            client.Send(mail);

            await SaveEmail(email);
        }
        public async Task SendNews(string toAddress, string subject, string message)
        {
            string? from = await _appSettings.Get("NewsEmailAddress");
            string? password = await _appSettings.Get("NewsMailPassword");
            EmailService service = new EmailService(_configuration, _dbContextFactory);
            Email email = new Email()
            {
                From = from,
                To = toAddress,
                Subject = subject,
                HtmlContent = message
            };
            await service.SendEmail(email, password);
        }
        private async Task SaveEmail(Email email)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.Emails.AddAsync(email);
            await dbContext.SaveChangesAsync();
        }
    }
}
