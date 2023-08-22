using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using CBD.Models;

namespace CBD.Services
{
    public class EmailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //handles local (secrets) and production email settings
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");
            var emailDisplayName = _mailSettings.DisplayName ?? Environment.GetEnvironmentVariable("DisplayName");

            MimeMessage newEmail = new();
            newEmail.From.Add(new MailboxAddress(emailDisplayName, emailSender));
            //newEmail.Sender = MailboxAddress.Parse(emailSender);

            //can send email alert to group of users
            foreach (var emailAddress in email.Split(":"))
            {
                newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            }

            newEmail.Subject = subject;

            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;

            newEmail.Body = emailBody.ToMessageBody();

            //log into client
            using SmtpClient smtpClient = new();

            try
            {
                //handles local (secrets) and production email settings
                var host = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
                var port = _mailSettings.EmailPort != 0 ? _mailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
                var password = _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");

                //turned None for now, gmail needs StartTls
                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);


                if (password != "")
                {
                    await smtpClient.AuthenticateAsync(emailSender, password);
                }

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);


            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }

        }
    }



}

