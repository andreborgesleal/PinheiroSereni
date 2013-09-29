using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PinheiroSereni.Negocio.Roles
{
    public class SessaoModel : Context, IPinheiroSereniCrud, IListRepository
    {
        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<SessaoRepository> list = (IEnumerable<SessaoRepository>)ListRepository(param);

                PagedListObject First = new PagedListObject() { index = pageIndex };
                PagedListObject Last = new PagedListObject() { index = pageIndex };
                PagedListObject Prior = new PagedListObject() { index = pageIndex };
                PagedListObject Next = new PagedListObject() { index = pageIndex };

                PagedListObject[] routeValues = { First, Prior, Next, Last };

                return new PagedList<SessaoRepository>(list.ToList(), pageIndex, pageSize, routeValues);
            }
            catch (Exception ex)
            {
                throw new Exception(PinheiroSereniException.Exception(ex, GetType().FullName, PinheiroSereniException.ErrorType.PaginationError));
            }
        }
        public IEnumerable<Repository> ListRepository(params object[] param)
        {
            return List();
        }
        public Repository getRepository(Object id)
        {
            return getObject(id);
        }
        #endregion

        #region Métodos da Interface IPinheiroSereniCrud
        #region getObject
        public Repository getObject(Object id)
        {
            //System.Web.HttpContext web = System.Web.HttpContext.Current;
            //id = web.Session.SessionID;

            using (db = base.Create())
            {
                SessaoRepository r = new SessaoRepository()
                {
                    sessao = db.Sessaos.Find(id)
                };

                return r;
            }
        }
        #endregion

        #region Validate
        public Validate Validate(Repository value, Crud operation)
        {
            return new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
        }
        #endregion

        #region List
        public IEnumerable<Repository> List()
        {
            using (db = new PinheiroSereniContext())
            {
                return (from c in db.Sessaos
                        select new SessaoRepository
                        {
                            sessao = c
                        }).ToList();
            }
        }
        #endregion

        #region Insert
        public Repository Insert(Repository value)
        {
            using (db = new PinheiroSereniContext())
            {
                try
                {
                    #region validar inclusão
                    value.mensagem = this.Validate(value, Crud.INCLUIR);
                    #endregion

                    #region insere a sessao
                    if (value.mensagem.Code == 0)
                    {
                        SessaoRepository r = (SessaoRepository)value;
                        r.sessao.sessaoId = getId();
                        if (db.Sessaos.Find(r.sessao.sessaoId) == null)
                        {
                            db.Sessaos.Add(r.sessao);
                        }
                        db.SaveChanges();
                    }
                    #endregion
                }
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    value.mensagem.Code = 17;
                    value.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                    value.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return value;
        }

        private string getId()
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;
            return web.Session.SessionID;
        }

        #endregion

        #region Update
        public Repository Update(Repository value)
        {
            using (db = new PinheiroSereniContext())
            {
                try
                {
                    #region validar alteração
                    value.mensagem = this.Validate(value, Crud.ALTERAR);
                    #endregion

                    #region altera a sessão
                    if (value.mensagem.Code == 0)
                    {
                        SessaoRepository r = (SessaoRepository)value;
                        db.Entry(r.sessao).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    #endregion
                }
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    value.mensagem.Code = 17;
                    value.mensagem.MessageBase = ex.Message;
                    value.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return value;

        }
        #endregion

        #region Delete
        public Repository Delete(Repository value)
        {
            using (db = new PinheiroSereniContext())
            {
                try
                {
                    #region validar exclusão
                    value.mensagem = this.Validate(value, Crud.EXCLUIR);
                    #endregion

                    #region exclui a sessao
                    if (value.mensagem.Code == 0)
                    {
                        SessaoRepository r = (SessaoRepository)value;
                        Sessao s = db.Sessaos.Find(r.sessao.sessaoId);
                        db.Sessaos.Remove(s);
                        db.SaveChanges();
                    }
                    #endregion
                }
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    value.mensagem.Code = 17;
                    value.mensagem.MessageBase = ex.Message;
                    value.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return value;
        }
        #endregion
        #endregion
        
    }
}
