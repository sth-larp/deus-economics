﻿using Microsoft.AspNet.Identity;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DeusCloud.Helpers;

namespace WispCloud.Identity.Services
{
    public class WispEmailService : IIdentityMessageService
    {
        public static WispEmailService Instance { get; }

        static WispEmailService()
        {
            Instance = new WispEmailService();
        }

        SmtpClient _client;
        MailAddress _from;

        public bool IsEnabled { get; private set; }

        private WispEmailService()
        {
            IsEnabled = AppSettings.Is("mailEnabled");
            if (!IsEnabled)
                return;

            this._from = new MailAddress(AppSettings.Raw("mailFrom"));
            this._client = new SmtpClient();
            this._client.Host = AppSettings.Raw("mailHost");
            this._client.Port = AppSettings.Int("mailPort").Value;
            this._client.EnableSsl = true;
            this._client.UseDefaultCredentials = false;
            this._client.Credentials = new NetworkCredential(
                AppSettings.Raw("mailUser"),
                AppSettings.Raw("mailPassword"));
        }

        public async Task Send(string to, string subject, string body, bool isBodyHtml)
        {
            if (!IsEnabled)
                return;

            var message = new MailMessage(_from.Address, to, subject, body) { IsBodyHtml = isBodyHtml };
            try
            {
                await Task.Run(() => _client.SendMailAsync(message));
            }
            catch (Exception)
            {

            }
        }

        async Task IIdentityMessageService.SendAsync(IdentityMessage message)
        {
            await Send(message.Destination, message.Subject, message.Body, true);
        }

    }

}