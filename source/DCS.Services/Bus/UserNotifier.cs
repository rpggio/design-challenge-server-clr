using System;
using System.Net;
using System.Net.Mail;
using DCS.Contracts;
using DCS.Core;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Services;
using log4net;
using Rebus;

namespace DCS.Services.Bus
{
    public class UserNotifier : IHandleMessages<NotifyUser>
    {
        private readonly ILog _log;
        private readonly AppSettings _appSettings;
        private readonly EntitiesRoot _entities;
        private readonly DcsHrefs _hrefs;

        public UserNotifier(AppSettings appSettings, ILog log, EntitiesRoot entities, DcsHrefs hrefs)
        {
            _appSettings = appSettings;
            _log = log;
            _entities = entities;
            _hrefs = hrefs;
        }

        public void Handle(NotifyUser message)
        {
            var user = _entities.Users.Get(message);
            var smtpSettings = user.IsTestUser
                ? _appSettings.Smtp.Test
                : _appSettings.Smtp.Real;
            var smtpHostParts = smtpSettings.Server.Split(':');
            var client = smtpHostParts.Length > 1
                ? new SmtpClient(smtpHostParts[0], int.Parse(smtpHostParts[1]))
                : new SmtpClient(smtpHostParts[0]);
            if (!smtpSettings.Username.IsEmpty())
            {
                client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                client.EnableSsl = true;
            }

            _log.DebugFormat("Sending message to {0} via {1}: {2}",
                user.Email,
                client.Host,
                message.Subject);
            string bodyHtml = "<h2>{0}</h2><p style='padding:10px'>{1}</p><p style='padding:10px;font-weight:bold'>See your status on <a href='{2}'>your user page</a></p>"
                .FormatFrom(
                    message.Subject,
                    message.Body,
                    _hrefs.UserPage(user.Id));
            var mailMessage = new MailMessage(
                _appSettings.Server.EmailFrom,
                user.Email,
                message.Subject,
                bodyHtml
                )
            {
                IsBodyHtml = true
            };
            try
            {
                client.Send(mailMessage);
                _log.DebugFormat("Send to {0} completed", user.Username);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error sending message to {0} via {1}: {2}",
                    user.Username,
                    smtpSettings.Server,
                    ex.Summary()
                    );
                throw;
            }
        }
    }
}