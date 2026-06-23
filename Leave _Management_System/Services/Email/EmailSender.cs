using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Leave__Management_System.Services.Email
{
    /// <summary>
    /// SMTP-based email sender service for sending transactional emails (confirmations, notifications, etc.)
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email asynchronously using configured SMTP settings
        /// </summary>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                // Load configuration
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPortString = _configuration["EmailSettings:SmtpPort"];
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

                // Validate email recipient
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("SendEmailAsync called with empty recipient email");
                    return;
                }

                // Validate configuration (allow sending if credentials are in User Secrets)
                if (string.IsNullOrWhiteSpace(fromEmail))
                {
                    _logger.LogWarning("EmailSettings:FromEmail is not configured. Skipping email to {Email}", email);
                    return;
                }

                if (string.IsNullOrWhiteSpace(smtpServer))
                {
                    _logger.LogWarning("EmailSettings:SmtpServer is not configured. Skipping email to {Email}", email);
                    return;
                }

                if (!int.TryParse(smtpPortString, out var smtpPort))
                {
                    _logger.LogWarning("EmailSettings:SmtpPort is missing or invalid. Using default port 587. Skipping email to {Email}", email);
                    return;
                }

                // If credentials are empty, log and skip (they should be in User Secrets for production)
                if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
                {
                    _logger.LogInformation("SMTP credentials not configured. Email functionality disabled. To enable, set EmailSettings:SmtpUsername and EmailSettings:SmtpPassword in User Secrets (development) or environment variables (production).");
                    return;
                }

                // Create and send email
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail);
                    message.To.Add(email);
                    message.Subject = subject;
                    message.Body = htmlMessage;
                    message.IsBodyHtml = true;

                    using (var client = new SmtpClient(smtpServer, smtpPort))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                        // Set timeout for slow connections
                        client.Timeout = 10000;

                        _logger.LogDebug("Sending email to {Recipient} with subject '{Subject}'", email, subject);

                        await client.SendMailAsync(message);

                        _logger.LogInformation("Email successfully sent to {Recipient}", email);
                    }
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error sending email to {Email}: {Message}", email, ex.Message);
                // Log but don't throw - email sending should not crash the application
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid email format for {Email}: {Message}", email, ex.Message);
                // Log but don't throw
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email to {Email}: {Message}", email, ex.Message);
                // Log but don't throw - email is not critical to application functionality
            }
        }
    }
}
