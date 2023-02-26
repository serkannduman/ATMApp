﻿using ATMApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.Application.Services.MailService
{


    public class MailService : IMailService
    {

        private readonly IConfiguration _configuration;
        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmail(MailModel model)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Port = Convert.ToInt32(_configuration["EmailConfiguration:Port"]);

            smtpClient.Host = _configuration["EmailConfiguration:Host"];

            smtpClient.EnableSsl = true;

            smtpClient.Credentials = new NetworkCredential(_configuration["EmailConfiguration:Mail"], _configuration["EmailConfiguration:Password"]);

            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_configuration["EmailConfiguration:Mail"], _configuration["EmailConfiguration:DisplayName"]);

            mailMessage.To.Add(model.ToEmail);

            mailMessage.Subject = model.Subject;

            mailMessage.Body = model.Body;

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
