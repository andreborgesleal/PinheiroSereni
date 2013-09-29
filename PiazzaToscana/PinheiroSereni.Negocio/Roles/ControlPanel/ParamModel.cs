using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories.ControlPanel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace PinheiroSereni.Negocio.Roles.ControlPanel
{
    public class ParamModel : Context, IPinheiroSereniCrud, IListRepository
    {
        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<ParamRepository> list = (IEnumerable<ParamRepository>)ListRepository(param);

                PagedListObject First = new PagedListObject() { index = pageIndex };
                PagedListObject Last = new PagedListObject() { index = pageIndex };
                PagedListObject Prior = new PagedListObject() { index = pageIndex };
                PagedListObject Next = new PagedListObject() { index = pageIndex };

                PagedListObject[] routeValues = { First, Prior, Next, Last };

                return new PagedList<ParamRepository>(list.ToList(), pageIndex, pageSize, routeValues);
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
            using (db = new PinheiroSereniContext())
            {
                ParamRepository r = new ParamRepository()
                {
                    mensagem = new Validate(),
                    parametro = db.Parametros.Find((int)id)
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
            using (db = new PinheiroSereniContext())
            {
                return (from p in db.Parametros
                        where p.parametroID == 1 || p.parametroID == 7
                        select new ParamRepository
                        {
                            mensagem = new Validate() { Code = 0 },
                            sessionId = System.Web.HttpContext.Current.Session.SessionID,
                            parametro = p
                        }).ToList();
            }
        }
        #endregion

        #region Insert
        public Repository Insert(Repository value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Update
        public Repository Update(Repository value)
        {
            using (db = new PinheiroSereniContext())
            {
                try
                {
                    value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
                    ParamRepository r = (ParamRepository)value;
                    db.Entry(r.parametro).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    value.mensagem.Code = 36;
                    value.mensagem.MessageBase = MensagemPadrao.Message(36).ToString();
                    if ((ex.Message.ToUpper().Contains("FOREIGN KEY") || ex.Message.ToUpper().Contains("REFERENCE")) ||
                        (ex.InnerException.ToString().ToUpper().Contains("FOREIGN KEY") || ex.InnerException.ToString().ToUpper().Contains("REFERENCE")))
                    {
                        value.mensagem.Code = 16;
                        value.mensagem.Message = MensagemPadrao.Message(16).ToString();
                    }
                    else if (ex.Message.ToUpper().Contains("PRIMARY KEY") ||
                        ex.InnerException.ToString().ToUpper().Contains("PRIMARY KEY"))
                    {
                        value.mensagem.Code = 37;
                        value.mensagem.Message = MensagemPadrao.Message(37).ToString();
                    }
                    else
                        value.mensagem.Message = MensagemPadrao.Message(17).ToString();
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                }
                catch (Exception ex)
                {
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    value.mensagem.Code = 17;
                    value.mensagem.MessageBase = ex.InnerException.InnerException.Message ?? ex.Message;
                    value.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return value;

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
