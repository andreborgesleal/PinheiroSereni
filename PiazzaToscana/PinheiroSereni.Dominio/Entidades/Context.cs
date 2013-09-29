using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinheiroSereni.Dominio.Entidades
{
    public abstract class Context
    {
        public PinheiroSereniContext db { get; set; }
        public PinheiroSereniContext Create(PinheiroSereniContext value)
        {
            this.db = value;
            return db;
        }

        public PinheiroSereniContext Create()
        {
            if (db == null)
                db = new PinheiroSereniContext();

            return db;
        }

    }
}
