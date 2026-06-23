using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Leave__Management_System.Services.Email
{
    /// <summary>
    /// Email configuration helper - provides Email Settings documentation and examples
    /// </summary>
    public static class EmailConfigurationHelper
    {
        /// <summary>
        /// Validates email configuration from settings
        /// </summary>
        /// <returns>Validation result with messages</returns>
        public static (bool IsValid, List<string> Messages) ValidateEmailSettings(IConfiguration configuration)
        {
            var messages = new List<string>();
            var isValid = true;

            var fromEmail = configuration["EmailSettings:FromEmail"];
            var smtpServer = configuration["EmailSettings:SmtpServer"];
            var smtpPort = configuration["EmailSettings:SmtpPort"];
            var smtpUsername = configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = configuration["EmailSettings:SmtpPassword"];

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                messages.Add("❌ EmailSettings:FromEmail is not configured");
                isValid = false;
            }
            else
            {
                messages.Add($"✅ From Email: {fromEmail}");
            }

            if (string.IsNullOrWhiteSpace(smtpServer))
            {
                messages.Add("❌ EmailSettings:SmtpServer is not configured");
                isValid = false;
            }
            else
            {
                messages.Add($"✅ SMTP Server: {smtpServer}");
            }

            if (string.IsNullOrWhiteSpace(smtpPort))
            {
                messages.Add("❌ EmailSettings:SmtpPort is not configured");
                isValid = false;
            }
            else
            {
                messages.Add($"✅ SMTP Port: {smtpPort}");
            }

            if (string.IsNullOrWhiteSpace(smtpUsername))
            {
                messages.Add("❌ EmailSettings:SmtpUsername is not configured");
                isValid = false;
            }
            else
            {
                messages.Add($"✅ SMTP Username: {smtpUsername}");
            }

            if (string.IsNullOrWhiteSpace(smtpPassword))
            {
                messages.Add("❌ EmailSettings:SmtpPassword is not configured");
                isValid = false;
            }
            else
            {
                messages.Add("✅ SMTP Password: [CONFIGURED]");
            }

            return (isValid, messages);
        }

        /// <summary>
        /// Gets example configurations for common email providers
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> GetExampleConfigurations()
        {
            return new Dictionary<string, Dictionary<string, string>>
            {
                {
                    "Gmail",
                    new Dictionary<string, string>
                    {
                        { "SmtpServer", "smtp.gmail.com" },
                        { "SmtpPort", "587" },
                        { "SmtpUsername", "your-email@gmail.com" },
                        { "SmtpPassword", "your-16-char-app-password" },
                        { "FromEmail", "your-email@gmail.com" },
                        { "Note", "Requires App Password (not main password)" }
                    }
                },
                {
                    "Outlook/Hotmail",
                    new Dictionary<string, string>
                    {
                        { "SmtpServer", "outlook.office365.com" },
                        { "SmtpPort", "587" },
                        { "SmtpUsername", "your-email@outlook.com" },
                        { "SmtpPassword", "your-app-password" },
                        { "FromEmail", "your-email@outlook.com" },
                        { "Note", "Requires App Password" }
                    }
                },
                {
                    "SendGrid",
                    new Dictionary<string, string>
                    {
                        { "SmtpServer", "smtp.sendgrid.net" },
                        { "SmtpPort", "587" },
                        { "SmtpUsername", "apikey" },
                        { "SmtpPassword", "SG.your-sendgrid-api-key" },
                        { "FromEmail", "noreply@yourdomain.com" },
                        { "Note", "Recommended for production" }
                    }
                },
                {
                    "Mailgun",
                    new Dictionary<string, string>
                    {
                        { "SmtpServer", "smtp.mailgun.org" },
                        { "SmtpPort", "587" },
                        { "SmtpUsername", "postmaster@yourdomain.com" },
                        { "SmtpPassword", "your-mailgun-smtp-password" },
                        { "FromEmail", "noreply@yourdomain.com" },
                        { "Note", "Recommended for production" }
                    }
                }
            };
        }
    }
}
