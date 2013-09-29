using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;

namespace PinheiroSereni.Library
{
    public class BindHtmlPiazzaToscana : IBindHtml
    {
        public object repository { get; set; }
        public string Subject { get; set; }
        public MailAddressCollection EmailTo { get; set; }
        public MailAddressCollection EmailCc { get; set; }
        public MailAddressCollection EmailBcc { get; set; }

        public BindHtmlPiazzaToscana(Object _repository)
        {
            this.repository = _repository;
            this.Subject = "FINAM: Abertura de Projeto";
            EmailTo = new MailAddressCollection();
            EmailCc = new MailAddressCollection();
            EmailBcc = new MailAddressCollection();
        }

        public string BindHtmlText()
        {
            string html = "";
            System.Web.HttpContext web = System.Web.HttpContext.Current; 
            string path = web.Request.PhysicalApplicationPath + "\\Views\\Html\\EmailAberturaProjeto.htm";
            StreamReader s = new StreamReader(path);
            using (s)
            {
                html = s.ReadToEnd();
                html = html.Replace("#idBeneficiario#", "Teste 1");
                html = html.Replace("#oficio#", "0001");
                html = html.Replace("#cnpj#", "09.050.082/0001-42");
                html = html.Replace("#Data#", "15/10/2012");
                html = html.Replace("#descricao#", "Teste 2");
            }

            return html;
        }
    }

}
