#define release
using System;
using System.Collections.Generic;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories;
using PinheiroSereni.Library;
using System.Net.Mail;

namespace PinheiroSereni.Negocio.Roles
{
    public class Inscricao : IPinheiroSereniCrud
    {
        #region Métodos da Interface IPinheiroSereniCrud
        #region getObject
        public Repository getObject(Object id)
        {
            using (PinheiroSereniContext db = new PinheiroSereniContext())
            {
                Prospect value = (Prospect)id;
                ProspectRepository r = new ProspectRepository()
                {
                    prospect = db.Prospects.Find(value.email, value.empreendimentoId)
                };
                return r;
            }
        }
        #endregion

        #region Validate
        public Validate Validate(Repository value, Crud operation)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region List
        public IEnumerable<Repository> List()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Insert
        public Repository Insert(Repository value)
        {
            using (PinheiroSereniContext db = new PinheiroSereniContext())
            {
                value.mensagem = new Validate() { Code = 0, Message = "Seu folder digital já foi enviado para sua caixa de e-mail.".Replace("[br]", "<br />") };
                try
                {
                    #region insere a inscricao do cliente
                    ProspectRepository r = (ProspectRepository)value;
                    r.prospect.nome = r.nomeCompleto;
                    r.prospect.email = r.prospect.email.ToLower();
                    r.prospect.dt_cadastro = DateTime.Now;
                    r.prospect.isFolderDigital = "S";
                    if (db.Prospects.Find(r.prospect.email, r.prospect.empreendimentoId) == null)
                    {
                        db.Prospects.Add(r.prospect);
                        db.SaveChanges();
                    }
                    #endregion

                    #region envia e-mail com o anexo do prospecto do empreendimento para o cliente e para a administração
                    Empreendimento emp = db.Empreendimentos.Find(r.prospect.empreendimentoId);

                    string fone_emp = "3131-4450";
                    if (emp.empreendimentoId == 2)
                        fone_emp = "3222-8111";

                    System.Web.HttpContext web = System.Web.HttpContext.Current;
                    EMail message = new EMail();
                    //message.From = new MailAddress(db.Parametros.Find((int)Parametros.EMAIL_SISTEMA).valor, db.Parametros.Find((int)Parametros.NOME_EMAIL_SISTEMA).valor);
                    message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["emailRemetente"], db.Parametros.Find((int)Parametros.NOME_EMAIL_SISTEMA).valor);
                    message.To.Add(new MailAddress(r.prospect.email, r.prospect.nome));
                    message.Bcc.Add(new MailAddress(db.Parametros.Find((int)Parametros.EMAIL_ADMINISTRACAO).valor, db.Parametros.Find((int)Parametros.NOME_ADMINISTRACAO).valor));
                    if (r.prospect.empreendimentoId == 1)
                        message.Attachments.Add(new Attachment(web.Server.MapPath("").Replace("\\Home","") + db.Parametros.Find((int)Parametros.PATH_EMAIL_ANEXO).valor + emp.nomeEmpreendimento.Replace(" ", "") + ".pdf"));
                    message.Subject = "Apresentação " + emp.nomeEmpreendimento + " - Folder Digital";
                    message.Body =  "<!DOCTYPE html>" + 
                                    "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
                                   "<head>" +
                                   "   <title>" + emp.nomeEmpreendimento + "</title>" +
                                   "</head>" +
                                   "<body>";
                    message.Body += "<div style=\"font-family: verdana; font-size: 12px; width: 700px\">" +
                                   "<div><p>Olá, <b>" + r.prospect.nome + "</b></p></div>" +
                                   "<div><p>Você cadastrou seus dados no hotsite " + emp.nomeEmpreendimento + ", um empreendimento da " + db.Parametros.Find((int)Parametros.NOME_ADMINISTRACAO).valor + ", e está recebendo, em anexo, nosso folder digital com detalhes do projeto.</p></div>" +
                                   "<div><p>Caso deseje obter mais informações sobre o " + emp.nomeEmpreendimento + ", teremos o maior prazer em atendê-lo através de nossa Central de Atendimento, pelo telefone: (91)" + fone_emp + ".</p></div>";

                    if (r.prospect.empreendimentoId == 2) // somente se for San Gennaro
                        message.Body += "<div><p><a href=\"" + db.Empreendimentos.Find(2).urlFolderDigital + "\">Clique aqui para consultar o folder digital do empreendimento</a></p></div>";

                    message.Body += "<div><p>Um abraço,</p></div>" +
                                   "<div><b>Salomão Benmuyal</b></div>" +
                                   "<div><b>Gerente Comercial</b></div>" +
                                   "<div><b>" + db.Parametros.Find((int)Parametros.NOME_ADMINISTRACAO).valor + "</b></div>" +
                                   "<div>&nbsp;</div>" +
                                   //"<div><img src=\"http://www.vendaspiazzatoscana.com.br/content/themes/base/images/PiazzaToscana/LogoPinheiroSereni.png\" alt=\"\" /></div>" +
                                   "</div>";
                    message.Body += "</body>" +
                                    "</html>";
                    #if (release)
                    message.SendMail();
                    #endif
                    message.Dispose();
                    #endregion
                }
                catch (PinheiroSereniException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem = new Validate() { Code = 15, Message = MensagemPadrao.Message(15).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    throw new PinheiroSereniException(ex.Message, GetType().FullName);
                }
            }
            
            return value;
        }
        #endregion

        #region Update
        public Repository Update(Repository value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Delete
        public Repository Delete(Repository value)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
