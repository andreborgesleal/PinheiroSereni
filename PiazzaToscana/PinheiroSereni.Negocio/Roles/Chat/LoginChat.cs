using System;
using System.Collections.Generic;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories;
using System.Data;
using System.Linq;

namespace PinheiroSereni.Negocio.Roles.Chat
{
    public class LoginChat : Context, ISecurity
    {
        public Validate autenticar(string usuario, string senha)
        {
            using (db = new PinheiroSereniContext())
            {
                Validate validate = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
                try
                {
                    int id = 0;
                    if (!int.TryParse(usuario, out id))
                        throw new ArithmeticException("ID informado não é um número inteiro válido");

                    #region Recupera o usuário
                    CorretorOnline operador = db.CorretorOnlines.Find(id);
                    #endregion

                    #region autenticar
                    if (operador != null)
                    {
                        if (!senha.Equals(operador.senha) || operador.situacao != "A")
                        {
                            validate.Code = 35;
                            validate.Message = MensagemPadrao.Message(35).ToString();
                            validate.MessageBase = MensagemPadrao.Message(999).ToString();
                        }
                    }
                    else
                    {
                        validate.Code = 35;
                        validate.Message = MensagemPadrao.Message(35).ToString();
                        validate.MessageBase = MensagemPadrao.Message(999).ToString();
                    }
                    #endregion

                    #region Limpa as sessões inativas e os chats vinculados
                    ChatModel chatModel = new ChatModel();
                    chatModel.CleanInactiveSessions();
                    #endregion

                    #region verifica se o corretor está online em alguma outra sessão
                    if ((from s1 in db.Sessaos where s1.corretorId == id && s1.dt_desativacao == null select s1).Count() > 0)
                    {
                        validate.Code = 36;
                        validate.Message = MensagemPadrao.Message(36).ToString();
                        validate.MessageBase = MensagemPadrao.Message(998).ToString();
                    }

                    #endregion

                    #region insere a sessao
                    if (validate.Code == 0)
                    {
                        System.Web.HttpContext web = System.Web.HttpContext.Current;

                        if (db.Sessaos.Find(web.Session.SessionID) == null)
                        {
                            Sessao sessao = new Sessao()
                            {
                                sessaoId = web.Session.SessionID,
                                dt_ativacao = DateTime.Now,
                                dt_atualizacao = DateTime.Now,
                                corretorId = operador.corretorId,
                                statusOperador = "O" // online
                            };

                            db.Sessaos.Add(sessao);
                            db.SaveChanges();
                            validate.Field = web.Session.SessionID;
                        }
                        else
                        {
                            Sessao sessao = db.Sessaos.Find(web.Session.SessionID);

                            #region verifica se a sessão é do mesmo corretor
                            if (sessao.corretorId != operador.corretorId && sessao.dt_desativacao == null)
                            {
                                validate.Code = 39;
                                validate.Message = MensagemPadrao.Message(39).ToString();
                                validate.MessageBase = MensagemPadrao.Message(998).ToString();
                            }
                            #endregion
                            else
                            {
                                sessao.dt_desativacao = null;
                                sessao.dt_atualizacao = DateTime.Now;
                                sessao.corretorId = operador.corretorId;
                                sessao.statusOperador = "O";

                                db.Entry(sessao).State = EntityState.Modified;
                                db.SaveChanges();
                                validate.Field = web.Session.SessionID;
                            }
                        }

                    }
                    #endregion

                }
                catch (ArithmeticException ex)
                {
                    validate.Code = 4;
                    validate.Message = MensagemPadrao.Message(4, "ID do operador", ex.Message).ToString();
                    validate.MessageBase = MensagemPadrao.Message(999).ToString();
                }
                catch (Exception ex)
                {
                    validate.Code = 17;
                    validate.MessageBase = ex.Message;
                    validate.Message = new PinheiroSereniException(ex.Message + " => " + ex.InnerException.Message, GetType().FullName).Message;
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
            catch (Exception ex)
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
