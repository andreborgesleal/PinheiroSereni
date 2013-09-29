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
    public class CorretoraCrud : Context, IPinheiroSereniCrud, IListRepository
    {
        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<CorretoraRepository> list = (IEnumerable<CorretoraRepository>)ListRepository(param);

                PagedListObject First = new PagedListObject() { index = pageIndex };
                PagedListObject Last = new PagedListObject() { index = pageIndex };
                PagedListObject Prior = new PagedListObject() { index = pageIndex };
                PagedListObject Next = new PagedListObject() { index = pageIndex };

                PagedListObject[] routeValues = { First, Prior, Next, Last };

                return new PagedList<CorretoraRepository>(list.ToList(), pageIndex, pageSize, routeValues);
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
                CorretoraRepository r = new CorretoraRepository()
                {
                    mensagem = new Validate(),
                    corretora = db.Corretoras.Find((int)id)
                };

                return r;
            }
        }
        #endregion

        #region Validate
        public Validate Validate(Repository value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
            if (operation != Crud.INCLUIR && ((CorretoraRepository)value).corretora.corretoraId == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Field = "corretora.nome";
                value.mensagem.Message = MensagemPadrao.Message(5, "Código da Corretora").ToString();
            }

            if (((CorretoraRepository)value).corretora.nome.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Field = "corretora.nome";
                value.mensagem.Message = MensagemPadrao.Message(5, "Nome da Corretora").ToString();
            }

            if (operation == Crud.INCLUIR)
            {
                int nomeCorretora = (from c in db.Corretoras
                                     where c.nome.Equals(((CorretoraRepository)value).corretora.nome)
                                     select c.nome).Count();
                if (nomeCorretora > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Field = "corretora.nome";
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                }
            }
            return value.mensagem;
        }
        #endregion

        #region List
        public IEnumerable<Repository> List()
        {
            using (db = new PinheiroSereniContext())
            {
                return (from c in db.Corretoras
                        select new CorretoraRepository
                        {
                            mensagem = new Validate() { Code = 0 },
                            corretora = c
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

                    #region insere a corretora
                    if (value.mensagem.Code == 0)
                    {
                        CorretoraRepository r = (CorretoraRepository)value;
                        r.corretora.corretoraId = getId();
                        if (db.Corretoras.Find(r.corretora.corretoraId) == null)
                        {
                            db.Corretoras.Add(r.corretora);
                        }
                        db.SaveChanges();
                    }
                    else
                        value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();

                    #endregion
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
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
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

        private int getId()
        {
            int id = 1;
            if (db.Corretoras.Count() > 0)
                id = db.Corretoras.ToList().Last().corretoraId + 1;
            return id;
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

                    #region altera a corretora
                    if (value.mensagem.Code == 0)
                    {
                        CorretoraRepository r = (CorretoraRepository)value;
                        db.Entry(r.corretora).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                        value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                    #endregion
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
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
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
            using (db = new PinheiroSereniContext())
            {
                try
                {
                    #region validar exclusão
                    value.mensagem = this.Validate(value, Crud.EXCLUIR);
                    #endregion

                    #region altera a corretora
                    if (value.mensagem.Code == 0)
                    {
                        CorretoraRepository r = (CorretoraRepository)value;
                        Corretora corr = db.Corretoras.Find(r.corretora.corretoraId);
                        db.Corretoras.Remove(corr);
                        db.SaveChanges();
                    }
                    else
                        value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();

                    #endregion
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
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
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
        #endregion
    }
}
