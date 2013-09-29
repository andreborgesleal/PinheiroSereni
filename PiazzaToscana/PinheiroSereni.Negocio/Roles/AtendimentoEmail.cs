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
using System.Data.Common;

namespace PinheiroSereni.Negocio.Roles
{
    public class AtendimentoEmail : IPinheiroSereniCrud
    {
        #region Métodos da Interface IPinheiroSereniCrud
        #region getObject
        public Repository getObject(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Validate
        public Validate Validate(Repository value, Crud operation)
        {
            AtendimentoEmailRepository r = (AtendimentoEmailRepository)value;
            if (r.prospect.telefone != null)
            {
                if (r.prospect.telefone.Length < 10 && r.prospect.telefone.Length > 0)
                {
                    value.mensagem = new Validate()
                    {
                        Code = 4,
                        Field = "prospect.telefone",
                        Message = "Número do telefone inválido"
                    };
                    return value.mensagem;
                }
                else
                    return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString().Replace("[br]", "<br />") };
            }
            else if (r.msg.empreendimentoId == null)
            {
                value.mensagem = new Validate()
                {
                    Code = 4,
                    Message = "Empreendimento deve ser informado"
                };
                return value.mensagem;
            }
            else
                return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString().Replace("[br]", "<br />") };
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
                try
                {
                    value.mensagem = Validate(value, Crud.INCLUIR);
                    if (value.mensagem.Code > 0)
                        throw new ArgumentException(value.mensagem.Message);

                    #region identifica o corretor da vez
                    AtendimentoEmailRepository r = (AtendimentoEmailRepository)value;
                    CorretorOnline corretor = r.corretor.obterCorretor(db.Mensagems, db.CorretorOnlines);
                    #endregion

                    #region insere a inscricao do cliente
                    r.prospect.dt_cadastro = DateTime.Now;
                    r.prospect.isFolderDigital = "N";
                    if (db.Prospects.Find(r.prospect.email, r.prospect.empreendimentoId) == null)
                        db.Prospects.Add(r.prospect);
                    #endregion

                    #region insere a mensagem
                    r.msg.dt_cadastro = DateTime.Now;
                    r.msg.email = r.prospect.email;
                    r.msg.empreendimentoId = r.prospect.empreendimentoId;
                    r.msg.corretorId = corretor.corretorId;
                    r.msg.emailDirecao1 = db.Parametros.Find((int)Parametros.EMAIL_ADMINISTRACAO).valor;
                    r.msg.assunto = "Confirmação de recebimento de e-mail";

                    db.Mensagems.Add(r.msg);
                    #endregion

                    db.SaveChanges();

                    #region envia e-mail para o corretor e para os diretores (com cópia para o próprio cliente)
                    string fone_emp = "3131-4450";

                    Empreendimento emp = db.Empreendimentos.Find(r.prospect.empreendimentoId);
                    if (emp.empreendimentoId == 2)
                        fone_emp = "3222-8111";

                    EMail message = new EMail();
                    //message.From = new MailAddress(db.Parametros.Find((int)Parametros.EMAIL_SISTEMA).valor, db.Parametros.Find((int)Parametros.NOME_EMAIL_SISTEMA).valor);
                    message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["emailRemetente"], db.Parametros.Find((int)Parametros.NOME_EMAIL_SISTEMA).valor);
                    message.To.Add(new MailAddress(corretor.email, corretor.nome));
                    message.Bcc.Add(new MailAddress(r.prospect.email, r.prospect.nome));
                    message.Bcc.Add(new MailAddress(db.Parametros.Find((int)Parametros.EMAIL_ADMINISTRACAO).valor, db.Parametros.Find((int)Parametros.NOME_ADMINISTRACAO).valor));
                    message.Subject = r.msg.assunto;
                    message.Body = "<div style=\"font-family: verdana; font-size: 12px; width: 700px\">" +
                                   "<div><p>Olá, <b>" + r.prospect.nome + "</b></p></div>" +
                                   "<div><p>Você cadastrou seus dados no hotsite " + emp.nomeEmpreendimento + ", um empreendimento da " + db.Parametros.Find((int)Parametros.NOME_ADMINISTRACAO).valor + ", e está recebendo a confirmação de envio de sua mensagem para um de nossos corretores.</p></div>" +
                                   "<div><p>E-Mail:</p></div>" +
                                   "<div><p><a href=\"mailto:" + r.prospect.email + "\">" + r.prospect.email + "</a></p></div>" +
                                   "<div><p>Mensagem:</p></div>" +
                                   "<div><p><i>\"" + r.msg.mensagem + "\"</i></p></div>" +
                                   "<div><p>Caso deseje obter mais informações sobre o " + emp.nomeEmpreendimento + ", teremos o maior prazer em atendê-lo através de nossa Central de Atendimento, pelo telefone: (91)" + fone_emp + ".</p></div>" +
                                   "<div><p>Um abraço,</p></div>" +
                                   "<div><b>Salomão Benmuyal</b></div>" +
                                   "<div><b>Gerente Comercial</b></div>" +
                                   "<div><b>" + db.Parametros.Find((int)Parametros.NOME_ADMINISTRACAO).valor + "</b></div>" +
                                   "<div>&nbsp;</div>" +
                                   "<div><img src=\"http://www.vendaspiazzatoscana.com.br/content/themes/base/images/PiazzaToscana/LogoPinheiroSereni.png\" alt=\"\" /></div>" +
                                   "</div>";
                    #if (release)
                    message.SendMail();
                    #endif
                    message.Dispose();
                    #endregion

                    //trans.Commit();
                }
                catch (PinheiroSereniException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = value.mensagem.Code, Message = MensagemPadrao.Message(value.mensagem.Code.Value,"telefone",value.mensagem.Message.ToString()).ToString(), MessageBase = ex.Message };
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    //trans.Rollback();
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem = new Validate() { Code = 15, Message = MensagemPadrao.Message(15).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    //trans.Rollback();
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
