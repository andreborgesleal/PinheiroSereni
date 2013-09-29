using PinheiroSereni.Library;
using System.Net.Mail;

namespace PinheiroSereni.Library
{
    public interface IBindHtml
    {
        object repository { get; set; }
        string Subject { get; set; }
        MailAddressCollection EmailTo { get; set; }
        MailAddressCollection EmailCc { get; set; }
        MailAddressCollection EmailBcc { get; set; }
        string BindHtmlText();
    }
}

namespace System.Net.Mail
{
    public class EMail : System.Net.Mail.MailMessage
    {
        private System.Net.Mail.SmtpClient client { get; set; }
        private System.Net.NetworkCredential credential { get; set; } //= new System.Net.NetworkCredential();

        private void Initialize()
        {
            this.IsBodyHtml = true;
            this.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            this.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            this.Priority = MailPriority.Normal;

            client = new System.Net.Mail.SmtpClient();
            credential = new System.Net.NetworkCredential()
            {
                Domain = System.Configuration.ConfigurationManager.AppSettings["Domain"],
                UserName = System.Configuration.ConfigurationManager.AppSettings["UserName"],
                Password = System.Configuration.ConfigurationManager.AppSettings["Password"]
            };

            if (!string.IsNullOrEmpty(credential.Domain))
                client.Credentials = credential;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
            client.Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Port"]);
            client.EnableSsl = false;
            client.Timeout = 900000;
        }

        public EMail()
        {
            Initialize();
        }

        public EMail(System.Net.Mail.MailAddress from,
                        System.Net.Mail.MailAddressCollection to,
                        System.Net.Mail.MailAddressCollection cc,
                        System.Net.Mail.MailAddressCollection bcc)
        {
            Initialize();

            foreach (System.Net.Mail.MailAddress TO in to)
                this.To.Add(TO);
            foreach (System.Net.Mail.MailAddress CC in cc)
                this.CC.Add(CC);
            foreach (System.Net.Mail.MailAddress BCC in bcc)
                this.Bcc.Add(BCC);
        }

        public EMail(System.Net.Mail.MailAddress from,
                                System.Net.Mail.MailAddressCollection to,
                                System.Net.Mail.MailAddressCollection cc,
                                System.Net.Mail.MailAddressCollection bcc,
                                string subject,
                                string body)
        {
            Initialize();

            this.From = from;
            foreach (System.Net.Mail.MailAddress TO in to)
                this.To.Add(TO);
            foreach (System.Net.Mail.MailAddress CC in cc)
                this.CC.Add(CC);
            foreach (System.Net.Mail.MailAddress BCC in bcc)
                this.Bcc.Add(BCC);
            this.Subject = subject;
            this.Body = body;
        }

        public string SendMail()
        {
            string result = "";

            try
            {
                client.Send(this);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                client.Dispose();
                this.Dispose();
            }

            return result;
        }

        public string SendMail(IBindHtml bindHtml)
        {
            this.Subject = bindHtml.Subject;
            foreach (MailAddress _to in bindHtml.EmailTo)
                this.To.Add(_to);
            foreach (MailAddress _cc in bindHtml.EmailCc)
                this.CC.Add(_cc);
            foreach (MailAddress _bcc in bindHtml.EmailBcc)
                this.Bcc.Add(_bcc);
            this.Body = bindHtml.BindHtmlText();
            return SendMail();
        }

    }
}

