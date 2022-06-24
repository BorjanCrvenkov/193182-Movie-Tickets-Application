using Domain.DomainModels;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class BackgroundEmailSender : IBackgroundEmailSender
    {

        private readonly IEmailService _emailService;
        private readonly IRepository<EmailMessage> _mailRepository;

        public BackgroundEmailSender(IEmailService emailService, IRepository<EmailMessage> mailRepository)
        {
            _emailService = emailService;
            _mailRepository = mailRepository;
        }
        public async Task DoWork()
        {
            List<EmailMessage> emailMessages = _mailRepository.GetAll().Where(z => !z.Status).ToList();
            await _emailService.SendEmailAsync(emailMessages);
            foreach(var mail in emailMessages)
            {
                mail.Status = true;
                this._mailRepository.Update(mail);
            }
        }
    }
}