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
    public class CorretorCrud : Context, IPinheiroSereniCrud, IListRepository
    {
        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<CorretorRepository> list = (IEnumerable<CorretorRepository>)ListRepository(param);

                PagedListObject First = new PagedListObject() { index = pageIndex };
                PagedListObject Last = new PagedListObject() { index = pageIndex };
                PagedListObject Prior = new PagedListObject() { index = pageIndex };
                PagedListObject Next = new PagedListObject() { index = pageIndex };

                PagedListObject[] routeValues = { First, Prior, Next, Last };

                return new PagedList<CorretorRepository>(list.ToList(), pageIndex, pageSize, routeValues);
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
                CorretorRepository r = new CorretorRepository()
                {
                    mensagem = new Validate(),
                    corretor = db.CorretorOnlines.Find((int)id)
                };

                return r;
            }
        }
        #endregion

        #region Validate
        public Validate Validate(Repository value, Crud operation)
        {
            CorretorRepository corr = (CorretorRepository)value;
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
            if (operation != Crud.INCLUIR && ((CorretorRepository)value).corretor.corretorId == null)
            {
                value.mensagem.Code = 5;
                value.mensagem.Field = "corretor.nome";
                value.mensagem.Message = MensagemPadrao.Message(5, "Código da Corretor").ToString();
                return value.mensagem;
            }

            if (((CorretorRepository)value).corretor.nome.Trim().Length == 0)
            {
                value.mensagem.Code = 5;
                value.mensagem.Field = "corretor.nome";
                value.mensagem.Message = MensagemPadrao.Message(5, "Nome da Corretor").ToString();
                return value.mensagem;
            }

            if (operation == Crud.INCLUIR)
            {
                int nomeCorretor = (from c in db.CorretorOnlines
                                     where c.nome.Equals(((CorretorRepository)value).corretor.nome)
                                     select c.nome).Count();
                if (nomeCorretor > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Field = "Corretor.nome";
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                    return value.mensagem;
                }
            }

            if (corr.corretor.telefone == null)
            {
                value.mensagem.Code = 5;
                value.mensagem.Field = "corretor.telefone";
                value.mensagem.Message = MensagemPadrao.Message(5, "Telefone").ToString();
                return value.mensagem;
            }

            if (corr.corretor.telefone.Trim().Length != 10)
            {
                value.mensagem.Code = 4;
                value.mensagem.Field = "corretor.telefone";
                value.mensagem.Message = MensagemPadrao.Message(4, "Telefone", "").ToString();
                return value.mensagem;
            }


            if (corr.corretor.email == null)
            {
                value.mensagem.Code = 5;
                value.mensagem.Field = "corretor.email";
                value.mensagem.Message = MensagemPadrao.Message(5, "E-Mail").ToString();
                return value.mensagem;
            }

            if (corr.corretor.senha == null)
            {
                value.mensagem.Code = 5;
                value.mensagem.Field = "corretor.senha";
                value.mensagem.Message = MensagemPadrao.Message(5, "Senha").ToString();
                return value.mensagem;
            }

            if (corr.corretor.senha.Trim() == "")
            {
                value.mensagem.Code = 5;
                value.mensagem.Field = "corretor.senha";
                value.mensagem.Message = MensagemPadrao.Message(5, "Senha").ToString();
                return value.mensagem;
            }

            #region verifica se já não tem algum corretor com a escala informada
            var q = from esc in db.CorretorOnlines where esc.corretorId != ((CorretorRepository)value).corretor.corretorId && esc.indexEscala == ((CorretorRepository)value).corretor.indexEscala select esc.corretorId;
            if (q.Count() > 0)
                {
                    value.mensagem.Code = 19;
                    value.mensagem.Field = "corretor.indexEscala";
                    value.mensagem.Message = MensagemPadrao.Message(19).ToString();
                }
            #endregion

            return value.mensagem;
        }
        #endregion

        #region List
        public IEnumerable<Repository> List()
        {
            using (db = new PinheiroSereniContext())
            {
                return (from c in db.CorretorOnlines
                        select new CorretorRepository
                        {
                            mensagem = new Validate() { Code = 0 },
                            sessionId = System.Web.HttpContext.Current.Session.SessionID,
                            corretor = c
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

                    #region insere o Corretor
                    if (value.mensagem.Code == 0)
                    {
                        CorretorRepository r = (CorretorRepository)value;
                        if (db.CorretorOnlines.Find(r.corretor.corretorId) == null)
                        {
                            db.CorretorOnlines.Add(r.corretor);
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

                    #region altera a Corretor
                    if (value.mensagem.Code == 0)
                    {
                        CorretorRepository r = (CorretorRepository)value;
                        db.Entry(r.corretor).State = EntityState.Modified;
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

                    #region excui o Corretor
                    if (value.mensagem.Code == 0)
                    {
                        CorretorRepository r = (CorretorRepository)value;
                        CorretorOnline corr = db.CorretorOnlines.Find(r.corretor.corretorId);
                        db.CorretorOnlines.Remove(corr);
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
