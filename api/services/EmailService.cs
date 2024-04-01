using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using utilities;
using Microsoft.Graph;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace permaAPI.services
{


    public class EmailService
    {
        public IConfiguration Configuration { get; }
        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task<Response> SendInviteEmail(ContactDetails contact, string password)
        {

            var emailTemplate = this.GetTemplate(EmailTemplateType.USERINVITE);
            if (emailTemplate != null)
            {
                string subject = emailTemplate.EmailSubject.Replace("{FIRSTNAME}", contact.FirstName).Replace("{DISPLAYNAME}", contact.DisplayName).Replace("{JOBTITLE}", contact.JobTitle);
                string body = emailTemplate.EmailTemplateBody.Replace("{FIRSTNAME}", contact.FirstName).Replace("{DISPLAYNAME}", contact.DisplayName).Replace("{JOBTITLE}", contact.JobTitle).Replace("{PASSWORD}", password).Replace("{EMAIL}", contact.Email);
                var client = new SendGridClient(this.Configuration["sendgridAPI"]);
                var msg = new SendGridMessage()
                {
                    From = new SendGrid.Helpers.Mail.EmailAddress(this.Configuration["sendgridFROM"], "PERMA Portal"),
                    Subject = subject,
                    HtmlContent = body
                };
                msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(contact.Email, contact.DisplayName));
                var response = await client.SendEmailAsync(msg);
                return response;
            }
            else
            {
                return null;
            }
        }
        public async Task SendSubmittedNotification(permaAPI.models.applications.Application app)
        {

            var emailTemplate = this.GetTemplate(EmailTemplateType.APPLICATIONSUBMITTED);
            if (emailTemplate != null)
            {
                string subject = emailTemplate.EmailSubject.Replace("{MEMBERNAME}", app.MemberName);
                string domain = this.Configuration["portalDomain"];
                string applicationLink = $"{domain}/applications/details/{app.ApplicationId}/{app.CurrentSectionId}";
                string body = emailTemplate.EmailTemplateBody.Replace("{MEMBERNAME}", app.MemberName).Replace("{APPLICATIONLINK}", $"<a href='{applicationLink}'>click here</a>");
                var client = new SendGridClient(this.Configuration["sendgridAPI"]);
                var msg = new SendGridMessage()
                {
                    From = new SendGrid.Helpers.Mail.EmailAddress(this.Configuration["sendgridFROM"], "PERMA Portal"),
                    Subject = subject,
                    HtmlContent = body
                };
                var recipients = emailTemplate.Recipients.Split(";");
                if (recipients.Length > 0)
                {
                    foreach (var item in recipients)
                    {
                        msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(item.Trim()));
                    }

                    var response = await client.SendEmailAsync(msg);

                }
                else
                {

                }
            }

        }



        public EmailTemplate GetTemplate(string templateType)
        {
            EmailTemplate template = null;
            string connectionString = this.Configuration.GetConnectionString("permaportal");

            string query = @"
select emailsubject, emailtemplatebody, recipients from emailtemplate where templatetype= @templatetype
";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@templateType", templateType);


                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        template = new EmailTemplate();
                        template.EmailSubject = reader["EmailSubject"].asString();
                        template.EmailTemplateBody = reader["emailTemplateBody"].asString();
                        template.Recipients = reader["recipients"].EmptyIfDBNullString();
                    }
                }
                return template;
            }
        }


    }
    public static class EmailTemplateType
    {
        public const string USERINVITE = "USERINVITE";
        public const string APPLICATIONSUBMITTED = "APPLICATIONSUBMITTED";
    }
    public class EmailTemplate
    {

        public string EmailTemplateBody { get; set; }
        public string EmailSubject { get; set; }
        public string Recipients { get; set; }
    }
}

