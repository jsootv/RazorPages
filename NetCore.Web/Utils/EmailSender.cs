using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NetCore.Web.Utils
{
    /// <summary>
    /// SendGrid 옵션들
    /// </summary>
    public class SmtpOptions
    {
        /// <summary>
        /// 발신자 정보
        /// </summary>
        public string SendGridUser { get; set; } = "Administrator";
        
        /// <summary>
        /// 발신자 이메일
        /// </summary>
        public string SendGridEmail { get; set; } = "admin@netcore.com";

        /// <summary>
        /// SendGrid API Key
        /// </summary>
        public string SendGridApiKey { get; set; }
    }

    public class EmailSender : IEmailSender
    {
        private SmtpOptions _Options { get; set; }

        public EmailSender(IOptions<SmtpOptions> optionsAccessor)
        {
            _Options = optionsAccessor.Value;
        }

        /// <summary>
        /// 이메일 보내기
        /// </summary>
        /// <param name="email">수신자 이메일</param>
        /// <param name="subject">이메일 제목</param>
        /// <param name="htmlMessage">이메일 내용</param>
        /// <returns></returns>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SendGridClient client = new SendGridClient(_Options.SendGridApiKey);

            SendGridMessage message = new SendGridMessage()
            {
                // 발신자 주소
                From = new EmailAddress(_Options.SendGridEmail, _Options.SendGridUser)
                , Subject = subject
                , PlainTextContent = htmlMessage
                , HtmlContent = htmlMessage
            };

            // 수신자
            message.AddTo(new EmailAddress(email));

            return client.SendEmailAsync(message);
        }
    }
}
