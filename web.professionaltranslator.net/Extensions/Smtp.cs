using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace web.professionaltranslator.net.Extensions
{
    internal class Smtp
    {
        internal enum BodyType
        {
            Html,
            Text
        }

        internal enum SslSetting
        {
            Off,
            On
        }

        internal static void SendMail(SiteSettings configuration, List<MailAddress> toList, string subject, string body, BodyType bodyType, SslSetting sslSetting)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(configuration.Postmaster, configuration.PostmasterDisplayName)
            };

            foreach (MailAddress mailAddress in toList)
            {
                mail.To.Add(mailAddress);
            }

            mail.Subject = subject;
            mail.Body = body;
            var smtp = new SmtpClient(configuration.SmtpServer);

            var credentials = new NetworkCredential(configuration.Postmaster, configuration.SmtpPassword);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = credentials;
            smtp.Port = sslSetting == SslSetting.On ? configuration.SmtpPortSsl : configuration.SmtpPort;
            smtp.EnableSsl = sslSetting == SslSetting.On;
            mail.IsBodyHtml = bodyType == BodyType.Html;
            smtp.SendAsync(mail, Guid.NewGuid().ToString());
        }
    }
}
