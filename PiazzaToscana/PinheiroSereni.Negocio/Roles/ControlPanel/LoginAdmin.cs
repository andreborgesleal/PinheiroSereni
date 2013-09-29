using System;
using System.Collections.Generic;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories;
using System.Net.Mail;
using System.Data;
using System.Data.Entity.Validation;
using PinheiroSereni.Negocio.Roles.Chat;

namespace PinheiroSereni.Negocio.Roles.ControlPanel
{
    public class LoginAdmin : Context, ISecurity
    {
        public Validate autenticar(string usuario, string senha)
        {
            using (db = new PinheiroSereniContext())
            {   
                Validate validate = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
                try
                {
                    #region Recupera o usuário
                    string _usuario = db.Parametros.Find((int)Parametros.EMAIL_ADMINISTRACAO).valor;
                    #endregion

                    #region recupera a senha
                    string _senha = db.Parametros.Find((int)Parametros.AUTKEY).valor;
                    #endregion

                    #region autenticar
                    if (!usuario.Equals(_usuario) || !senha.Equals(_senha))
                    {
                        validate.Code = 35;
                        validate.Message = MensagemPadrao.Message(35).ToString();
                        validate.MessageBase = MensagemPadrao.Message(999).ToString();
                    }
                    #endregion

                    #region insere a sessao
                    if (validate.Code == 0)
                    {
                        System.Web.HttpContext web = System.Web.HttpContext.Current;
                        Sessao s1 = db.Sessaos.Find(web.Session.SessionID);

                        if (s1 == null)
                        {
                            Sessao sessao = new Sessao()
                            {
                                sessaoId = web.Session.SessionID,
                                dt_ativacao = DateTime.Now,
                                dt_atualizacao = DateTime.Now,
                                statusOperador = "O"
                            };

                            db.Sessaos.Add(sessao);
                        }
                        else
                        {
                            Sessao sessao = db.Sessaos.Find(web.Session.SessionID);
                            sessao.dt_desativacao = null;
                            sessao.dt_atualizacao = DateTime.Now;

                            db.Entry(sessao).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        validate.Field = web.Session.SessionID;
                    }
                    #endregion
                }
                catch (DbEntityValidationException ex)
                {
                    throw new PinheiroSereniException(ex.Message, GetType().FullName);
                }
                catch (Exception ex)
                {
                    throw new PinheiroSereniException(ex.Message, GetType().FullName);
                }
                return validate;
            }

        }

        public bool validarSessao(string sessionId)
        {
            try
            {
                using (db = base.Create())
                {
                    #region Validar Sessão do usuário
                    Sessao s = db.Sessaos.Find(sessionId);
                    if (s == null)
                        return false;
                    #endregion

                    #region Verifica se a sessão já expirou
                    if (s.dt_desativacao != null)
                        return false;
                    #endregion

                    #region Atualiza a sessão
                    s.dt_atualizacao = DateTime.Now;
                    db.Entry(s).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                }
            }
            catch(Exception ex)
            {
                PinheiroSereniException.saveError(ex, GetType().FullName);
                return false;
            }

            return true;
        }

        public void EncerrarSessao(string sessionId)
        {
            try
            {
                using (db = base.Create())
                {
                    #region Desativa a sessão
                    Sessao s = db.Sessaos.Find(sessionId);
                    if (s != null)
                    {
                        s.dt_atualizacao = DateTime.Now;
                        s.dt_desativacao = DateTime.Now;
                        db.Entry(s).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    #endregion

                    #region Desativa as sessões que estão sem atualização há mais de 15 minutos
                    ChatModel chat = new ChatModel();
                    chat.CleanInactiveSessions();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                PinheiroSereniException.saveError(ex, GetType().FullName);
            }
        }
    }
}
