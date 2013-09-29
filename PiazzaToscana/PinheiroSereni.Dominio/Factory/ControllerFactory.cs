using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Control;

namespace PinheiroSereni.Dominio.Factory
{
    public class ControllerFactory<T> : IController<T>
    {
        private T Instance
        {
            get;
            set;
        }

        private IListRepository query
        {
            get { return (IListRepository)Instance; }
        }

        private IPinheiroSereniCrud crud
        {
            get { return (IPinheiroSereniCrud)Instance; }
        }

        private IDeleteInfo deleteInfo
        {
            get { return (IDeleteInfo)Instance; }
        }

        private Type typeInstance { get; set; }

        public ControllerFactory()
        {
            this.typeInstance = typeof(T);
            Instance = (T)Activator.CreateInstance(typeInstance); 
        }

        #region Métodos da Interface IController
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            return query.getPagedList(index, pageSize, param);
        }

        public IEnumerable<Repository> ListRepository(params object[] param)
        {
            return query.ListRepository(param);
        }

        public Repository getRepository(Object id)
        {
            using (PinheiroSereniContext db = new PinheiroSereniContext())
                return query.getRepository(id);
        }

        public Repository getObject(Object id)
        {
            return crud.getObject(id);
        }

        public Repository Insert(Repository repository)
        {
            return crud.Insert(repository); 
        }

        public Repository Edit(Repository repository)
        {
            return crud.Update(repository);
        }

        public Repository Delete(Repository repository)
        {
            return crud.Delete(repository);
        }

        #endregion
    }
}