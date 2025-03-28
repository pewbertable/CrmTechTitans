using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CrmTechTitans.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;
        private readonly IWebHostEnvironment _environment;

        public EmailSender(
            IOptions<EmailSettings> emailSettings, 
            ILogger<EmailSender> logger,
            IWebHostEnvironment environment)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _environment = environment;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                // Always create the mail message (we'll need it if we're sending for real)
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                
                mailMessage.To.Add(email);

                // Determine if we're in development mode
                bool isDevelopmentMode = _environment.IsDevelopment() || _emailSettings.DevelopmentMode;

                // Log the email information clearly in the console (useful in both dev and prod)
                _logger.LogInformation("---------------------------------------------------------");
                _logger.LogInformation($"Email To: {email}");
                _logger.LogInformation($"Subject: {subject}");
                
                if (isDevelopmentMode)
                {
                    _logger.LogInformation("---------------------------------------------------------");
                    _logger.LogInformation("📧 DEVELOPMENT MODE EMAIL BODY:");
                    _logger.LogInformation("---------------------------------------------------------");
                    _logger.LogInformation($"{htmlMessage}");
                    _logger.LogInformation("---------------------------------------------------------");
                }

                // Configure the SMTP client
                string smtpServer = isDevelopmentMode && _emailSettings.Development?.SmtpServer != null
                    ? _emailSettings.Development.SmtpServer
                    : _emailSettings.SmtpServer;
                
                int port = isDevelopmentMode && _emailSettings.Development?.Port != null
                    ? _emailSettings.Development.Port.Value
                    : _emailSettings.Port;
                
                bool enableSsl = isDevelopmentMode && _emailSettings.Development?.EnableSsl != null
                    ? _emailSettings.Development.EnableSsl.Value
                    : _emailSettings.EnableSsl;

                using var client = new SmtpClient(smtpServer, port);
                
                // Only use credentials in production or if specifically configured for development
                if (!isDevelopmentMode || 
                    (isDevelopmentMode && !string.IsNullOrEmpty(_emailSettings.Development?.Username)))
                {
                    client.Credentials = new NetworkCredential(
                        isDevelopmentMode && _emailSettings.Development?.Username != null 
                            ? _emailSettings.Development.Username 
                            : _emailSettings.Username,
                        isDevelopmentMode && _emailSettings.Development?.Password != null 
                            ? _emailSettings.Development.Password 
                            : _emailSettings.Password
                    );
                }
                
                client.EnableSsl = enableSsl;

                // Skip actual sending if using log-only mode
                bool skipSending = isDevelopmentMode && 
                                  (_emailSettings.Development == null || 
                                   !_emailSettings.Development.SendEmailsInDevelopment);
                
                if (skipSending)
                {
                    _logger.LogInformation("📧 DEV MODE: Email Not Sent (Logged Only)");
                    return;
                }

                // If we get here, we're actually going to send the email
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {email} via {smtpServer}:{port}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {email}: {ex.Message}");
                throw;
            }
        }
    }

    public class EmailSettings
    {
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public bool DevelopmentMode { get; set; } = true; // Default to development mode
        public DevelopmentEmailSettings Development { get; set; }
    }

    public class DevelopmentEmailSettings
    {
        public string SmtpServer { get; set; }
        public int? Port { get; set; }
        public bool? EnableSsl { get; set; }
        public bool GenerateLinks { get; set; } = true; // Always display confirmation links in development by default
        public bool SendEmailsInDevelopment { get; set; } = true; // Actually send emails in development by default
        public string Username { get; set; }
        public string Password { get; set; }
    }
} 