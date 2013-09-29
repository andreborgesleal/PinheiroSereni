﻿#define release
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
    public class LigamosParaVoce : IPinheiroSereniCrud
    {
        #region getObject
        public Repository getObject(Object id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Validate
        public Validate Validate(Repository value, Crud operation)
        {
            SMSRepository r = (SMSRepository)value;
            if (r.sms.telefone.Length < 10 && r.sms.telefone.Length > 0)
            {
                value.mensagem = new Validate()
                {
                    Code = 4,
                    Field = "sms.telefone",
                    Message = "Número do telefone inválido"
                };
                return value.mensagem;
            }
            else if (r.sms.empreendimentoId == null)
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
                    SMSRepository r = (SMSRepository)value;
                    CorretorOnline corretor = r.corretor.obterCorretor(db.SMSs, db.CorretorOnlines);
                    #endregion

                    #region insere o registro SMS do cliente
                    r.sms.dt_cadastro = DateTime.Now;
                    r.sms.corretorId = corretor.corretorId;
                    db.SMSs.Add(r.sms);
                    db.SaveChanges();
                    #endregion

                    #region enviar o SMS
                    
                    #if (release)
                    Torpedo torpedo = new Torpedo();
                    value.mensagem = torpedo.send("", r.sms.nome, r.sms.telefone, corretor.telefone);
                    Validate mensagemCaco = torpedo.send("", r.sms.nome, r.sms.telefone, "9184524500"); 
                    #endif

                    #endregion
                }
                catch (PinheiroSereniException ex)
                {
                    value.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (ArgumentException ex)
                {
                    value.mensagem = new Validate() { Code = value.mensagem.Code, Message = MensagemPadrao.Message(value.mensagem.Code.Value, "telefone", value.mensagem.Message.ToString()).ToString(), MessageBase = ex.Message };
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
    }
}
