using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories;
using PinheiroSereni.Negocio.Repositories.Chat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PinheiroSereni.Negocio.Roles.Chat
{
    public class ChatModel : Context, IListRepository, IChatOperations
    {
        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 40, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<ChatRepository> list = (IEnumerable<ChatRepository>)ListRepository(param);

                PagedListObject First = new PagedListObject() { index = pageIndex };
                PagedListObject Last = new PagedListObject() { index = pageIndex };
                PagedListObject Prior = new PagedListObject() { index = pageIndex };
                PagedListObject Next = new PagedListObject() { index = pageIndex };

                PagedListObject[] routeValues = { First, Prior, Next, Last };

                return new PagedList<ChatRepository>(list.ToList(), pageIndex, pageSize, routeValues);
            }
            catch (Exception ex)
            {
                throw new Exception(PinheiroSereniException.Exception(ex, GetType().FullName, PinheiroSereniException.ErrorType.PaginationError));
            }
        }
        public IEnumerable<Repository> ListRepository(params object[] param)
        {
            string _path_fotoCorretor = db.Parametros.Find((int)Parametros.PATH_FOTOCORRETOR).valor;
            string sessionId = param[0].ToString();

            using (db = new PinheiroSereniContext())
            {
                IEnumerable<ChatRepository> q = (
                                                    from ch in db.Chats
                                                    join s in db.Sessaos on ch.Sessao equals s
                                                    join c in db.CorretorOnlines on s.CorretorOnline equals c
                                                    join p in db.Prospects on ch.Prospect equals p
                                                    where ch.sessaoId == sessionId && ch.dt_fim == null
                                                    select new ChatRepository()
                                                    {
                                                        sessionId = sessionId,
                                                        sessao = s,
                                                        corretor = c,
                                                        nome_cliente = p.nome,
                                                        path_fotoCorretor = _path_fotoCorretor,
                                                        chat = ch,
                                                        chatMessages = (from m in db.ChatMessages
                                                                        where m.chatId == ch.chatId
                                                                        orderby m.dt_message
                                                                        select m),
                                                    }
                                                ).ToList();
                return q;
            }
        }
        public Repository getRepository(Object id)
        {
            using (db = Create())
            {
                int chatId = int.Parse(id.ToString());
                ChatRepository q = (from ch in db.Chats
                                    join s in db.Sessaos on ch.Sessao equals s
                                    join c in db.CorretorOnlines on s.CorretorOnline equals c
                                    join p in db.Prospects on ch.Prospect equals p
                                    where ch.chatId == chatId
                                    select new ChatRepository()
                                    {
                                        isOnline = ch.dt_fim == null ? "S" : "N",
                                        sessionId = s.sessaoId,
                                        sessao = s,
                                        corretor = c,
                                        prospect = p,
                                        nome_cliente = p.nome,
                                        chat = ch,
                                        typingClient = ch.typingClient,
                                        typingOperator = ch.typingOperator,
                                        chatMessages = (from m in db.ChatMessages
                                                        where m.chatId == chatId
                                                        orderby m.dt_message
                                                        select m),
                                    }).First();
                
                q.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };
                q.path_fotoCorretor = db.Parametros.Find((int)Parametros.PATH_FOTOCORRETOR).valor;

                return q;
            }      
            
        }
        #endregion

        #region Métodos da Interface IChatOperations
        #region métodos do lado do corretor
        public Repository getSessao(string sessionId, int? chatId = null)
        {
            using (db = Create())
            {
                string _path_fotoCorretor = db.Parametros.Find((int)Parametros.PATH_FOTOCORRETOR).valor;
                SessaoRepository q = (from s in db.Sessaos
                                      join c in db.CorretorOnlines on s.CorretorOnline equals c
                                      where s.sessaoId == sessionId
                                      select new SessaoRepository()
                                      {
                                          sessionId = sessionId,
                                          sessao = s,
                                          corretorOnline = c,
                                          chatRepositories = (
                                                                from ch in db.Chats
                                                                join s1 in db.Sessaos on ch.Sessao equals s1
                                                                join c1 in db.CorretorOnlines on s1.CorretorOnline equals c1
                                                                join p in db.Prospects on ch.Prospect equals p
                                                                join e in db.Empreendimentos on ch.empreendimentoId equals e.empreendimentoId
                                                                where ch.sessaoId == sessionId && ch.dt_fim == null
                                                                select new ChatRepository()
                                                                {
                                                                    isAtivo = chatId.HasValue ? ch.chatId == chatId ? "S" : "N" : "N",
                                                                    sessionId = sessionId,
                                                                    sessao = s1,
                                                                    corretor = c1,
                                                                    nome_cliente = p.nome,
                                                                    path_fotoCorretor = _path_fotoCorretor,
                                                                    chat = ch,
                                                                    typingClient = ch.typingClient,
                                                                    typingOperator = ch.typingOperator,
                                                                    nome_empreendimento = e.nomeEmpreendimento,
                                                                    chatMessages = (from m in db.ChatMessages
                                                                                    where m.chatId == ch.chatId
                                                                                    orderby m.dt_message
                                                                                    select m),
                                                                }
                                                            )
                                      }).First();
                return q;
            }
        }

        public Repository ping(string sessionId)
        {
            SessaoRepository sessaoRepository;
            using (db = Create())
            {
                sessaoRepository = (SessaoRepository)Create(sessionId);
                try
                {
                    #region atualiza sessao
                    updateSession(sessionId);
                    #endregion

                    return getSessao(sessionId);
                }
                catch (ArgumentException ex)
                {
                    sessaoRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    sessaoRepository.mensagem.Code = 17;
                    sessaoRepository.mensagem.MessageBase = ex.Message;
                    sessaoRepository.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return sessaoRepository;
        }

        /// <summary>
        /// Muda o status do operador (Online ou Invisível).
        /// Se o operador estiver no status Invisível, o sistema não irá lhe atribuir novos clientes para chat.
        /// Mas o mesmo continuará atendendo os clientes que já estão na lista.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public Repository changeStatusOperador(string sessionId, string newStatus)
        {
            SessaoRepository sessaoRepository;
            using (db = Create())
            {
                sessaoRepository = (SessaoRepository)Create(sessionId);
                try
                {
                    #region atualiza sessao
                    updateSession(sessionId, newStatus);
                    #endregion

                    return getSessao(sessionId);
                }
                catch (ArgumentException ex)
                {
                    sessaoRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    sessaoRepository.mensagem.Code = 17;
                    sessaoRepository.mensagem.MessageBase = ex.Message;
                    sessaoRepository.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return sessaoRepository;
        }

        public IEnumerable<Repository> listening(string sessionId, int? chatId)
        {
            throw new NotImplementedException();
        }

        public Repository ActivateEdition(string sessionId, int chatId)
        {
            SessaoRepository sessaoRepository;
            using (db = Create())
            {
                sessaoRepository = (SessaoRepository)Create(sessionId);
                try
                {
                    #region atualiza sessao
                    updateSession(sessionId);
                    #endregion

                    return getSessao(sessionId, chatId);
                }
                catch (ArgumentException ex)
                {
                    sessaoRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    sessaoRepository.mensagem.Code = 17;
                    sessaoRepository.mensagem.MessageBase = ex.Message;
                    sessaoRepository.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return sessaoRepository;
        }

        public Repository Finish(string sessionId, int chatId)
        {
            SessaoRepository sessaoRepository;
            using (db = Create())
            {
                sessaoRepository = (SessaoRepository)Create(sessionId);
                try
                {
                    #region Finalizar Chat
                    PinheiroSereni.Dominio.Entidades.Chat chatEncerrar = db.Chats.Find(chatId);
                    chatEncerrar.dt_fim = DateTime.Now;
                    db.Entry(chatEncerrar).State = EntityState.Modified;
                    #endregion

                    #region Insere mensagem de despedida ao cliente
                    return Send(sessionId, chatId, "Prezado cliente, a Pinheiro Sereni Engenharia agradece o seu contato e qualquer dúvida estamos à disposição para maiores esclarecimentos.");
                    #endregion
                }
                catch (ArgumentException ex)
                {
                    sessaoRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    sessaoRepository.mensagem.Code = 17;
                    sessaoRepository.mensagem.MessageBase = ex.Message;
                    sessaoRepository.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return sessaoRepository;
        }

        public Repository Send(string sessionId, int chatId, string text)
        {
            SessaoRepository sessaoRepository;
            using (db = Create())
            {
                sessaoRepository = (SessaoRepository)Create(sessionId);
                try
                {
                    #region grava mensagem
                    PinheiroSereni.Dominio.Entidades.Chat ch = db.Chats.Find(chatId);
                    ChatMessage msg = new ChatMessage()
                    {
                        messageId = Guid.NewGuid(),
                        chatId = chatId,
                        corretorId = ch.corretorId,
                        empreendimentoId = ch.empreendimentoId,
                        dt_message = DateTime.Now,
                        message = text.Replace("'","")
                    };
                    db.ChatMessages.Add(msg);

                    _TypingOperator(chatId, "N");

                    #endregion

                    #region atualiza sessao
                    updateSession(sessionId);
                    #endregion

                    return getSessao(sessionId, chatId);
                }
                catch (ArgumentException ex)
                {
                    sessaoRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    sessaoRepository.mensagem.Code = 17;
                    sessaoRepository.mensagem.MessageBase = ex.Message;
                    sessaoRepository.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return sessaoRepository;
        }

        public void Exit(string sessionId)
        {
            using (db = new PinheiroSereniContext())
            {
                SessaoRepository r = (from s in db.Sessaos
                                      where s.sessaoId == sessionId
                                      select new SessaoRepository()
                                      {
                                        sessionId = s.sessaoId,
                                        sessao = s,
                                        chatRepositories = (from c in db.Chats
                                                            where c.sessaoId == sessionId && c.dt_fim == null
                                                            select new ChatRepository
                                                            {
                                                                chat = c
                                                            })
                                      }).First();

                Sessao _s = db.Sessaos.Find(sessionId);
                _s.dt_desativacao = DateTime.Now;
                db.Entry(_s).State = EntityState.Modified;

                foreach (ChatRepository cr in r.chatRepositories)
                {
                    PinheiroSereni.Dominio.Entidades.Chat _chat = db.Chats.Find(cr.chat.chatId);
                    _chat.dt_fim = DateTime.Now;
                    db.Entry(_chat).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
        }

        public void _TypingOperator(int chatId, string value)
        {
            PinheiroSereni.Dominio.Entidades.Chat chat = db.Chats.Find(chatId);
            chat.typingOperator = value;
            db.Entry(chat).State = EntityState.Modified;
        }

        public void TypingOperator(int chatId, string value)
        {
            using (db = new PinheiroSereniContext())
            {
                _TypingOperator(chatId, value);
                db.SaveChanges();
            }
        }
        #endregion

        #region Métodos do lado do cliente
        public Repository ContabilizaClick(Repository chatRepository)
        {
            using (db = new PinheiroSereniContext())
            {
                try
                {
                    ChatRepository r = (ChatRepository)chatRepository;

                    #region verifica se tem sessão ativa para o corretor da vez
                    var _s = db.Sessaos.Take(1);
                    #endregion

                    r.corretor = db.CorretorOnlines.Find(25); // Supervisor Online
                    r.sessionId = _s.First().sessaoId;
                    r.sessao = _s.First();

                    #region Incluir cliente
                    Prospect p = new Prospect()
                    {
                        email = ((ChatRepository)chatRepository).prospect.email.ToLower(),
                        empreendimentoId = ((ChatRepository)chatRepository).prospect.empreendimentoId,
                        nome = ((ChatRepository)chatRepository).prospect.nome,
                        isFolderDigital = "N",
                        dt_cadastro = DateTime.Now
                    };

                    if (db.Prospects.Find(r.prospect.email, r.prospect.empreendimentoId) == null)
                        db.Prospects.Add(p);
                    #endregion

                    #region Incluir o Chat
                    PinheiroSereni.Dominio.Entidades.Chat chat = new Dominio.Entidades.Chat()
                    {
                        sessaoId = r.sessionId,
                        corretorId = r.corretor.corretorId,
                        email = r.prospect.email,
                        empreendimentoId = r.prospect.empreendimentoId,
                        dt_inicio = DateTime.Now,
                        dt_fim = DateTime.Now.AddSeconds(1),
                        typingClient = "N",
                        typingOperator = "N"
                    };
                    db.Chats.Add(chat);

                    #endregion

                    #region Incluir a mensagem de boas vindas
                    ChatMessage msg = new ChatMessage()
                    {
                        messageId = Guid.NewGuid(),
                        chatId = chat.chatId,
                        email = r.prospect.email,
                        dt_message = DateTime.Now,
                        empreendimentoId = r.prospect.empreendimentoId,
                        message = "[Contabilização de CHAT - Data do click: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "h. - IP do cliente: " + System.Web.HttpContext.Current.Request.UserHostAddress + " ]"
                    };
                    db.ChatMessages.Add(msg);
                    #endregion

                    db.SaveChanges();

                    r.chat.chatId = chat.chatId;

                    return r;
                }
                catch (ArgumentNullException ex)
                {
                    chatRepository.mensagem = new Validate() { Code = 35, Message = MensagemPadrao.Message(35).ToString(), MessageBase = ex.Message };
                }
                catch (ArgumentException ex)
                {
                    chatRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    chatRepository.mensagem.Code = 17;
                    chatRepository.mensagem.MessageBase = ex.Message;
                    chatRepository.mensagem.Message = new PinheiroSereniException(ex.Message + " => " + ex.InnerException.Message, GetType().FullName).Message;
                }

            }
            return chatRepository;

        }

        /// <summary>
        /// Inicia um novo chat entre o cliente e o corretor
        /// </summary>
        /// <param name="chatRepository"></param>
        /// <returns></returns>
        public Repository Start(Repository chatRepository)
        {
            using (db = new PinheiroSereniContext())
            {
                try
                {
                    ChatRepository r = (ChatRepository)chatRepository;

                    #region identifica o corretor da vez
                    r.corretor = r.corretorDaVez.obterCorretor(db.Chats, db.CorretorOnlines);
                    #endregion

                    #region verifica se tem sessão ativa para o corretor da vez
                    var _s = from s in db.Sessaos
                             where s.corretorId == r.corretor.corretorId &&
                                   s.dt_desativacao == null &&
                                   s.statusOperador.Equals("O")
                             select s;
                    #endregion

                    #region Se o corretor da vez não tiver sessão ativa, procurar o primeiro corretor que esteja online
                    if (_s.Count() == 0)
                    {
                        _s = from s in db.Sessaos
                             where s.dt_desativacao == null &&
                                   s.statusOperador.Equals("O") &&
                                   s.corretorId != null
                             orderby s.CorretorOnline.indexEscala
                             select s;

                        if (_s.Count() == 0)
                            throw new ArgumentNullException(); // não tem nenhum corretor online
                        else if (_s.Where(m => m.CorretorOnline.indexEscala >= r.corretor.indexEscala).Count() > 0)
                            _s = _s.Where(m => m.CorretorOnline.indexEscala >= r.corretor.indexEscala);
                    }
                    #endregion

                    r.corretor = db.CorretorOnlines.Find(_s.First().corretorId);
                    r.sessionId = _s.First().sessaoId;
                    r.sessao = _s.First();

                    #region Incluir cliente
                    Prospect p = new Prospect()
                    {
                        email = ((ChatRepository)chatRepository).prospect.email.ToLower(),
                        empreendimentoId = ((ChatRepository)chatRepository).prospect.empreendimentoId,
                        nome = ((ChatRepository)chatRepository).prospect.nome,
                        telefone = ((ChatRepository)chatRepository).prospect.telefone,
                        isFolderDigital = "N",
                        dt_cadastro = DateTime.Now
                    };

                    if (db.Prospects.Find(r.prospect.email, r.prospect.empreendimentoId) == null)
                        db.Prospects.Add(p);
                    #endregion

                    #region Incluir o Chat
                    PinheiroSereni.Dominio.Entidades.Chat chat = new Dominio.Entidades.Chat()
                    {
                        sessaoId = r.sessionId,
                        corretorId = r.corretor.corretorId,
                        email = r.prospect.email,
                        empreendimentoId = r.prospect.empreendimentoId,
                        dt_inicio = DateTime.Now
                    };
                    db.Chats.Add(chat);

                    #endregion

                    #region Incluir a mensagem de boas vindas
                    ChatMessage msg = new ChatMessage()
                    {
                        messageId = Guid.NewGuid(),
                        chatId = chat.chatId,
                        corretorId = r.corretor.corretorId,
                        dt_message = DateTime.Now,
                        empreendimentoId = r.prospect.empreendimentoId,
                        message = "Olá <b>" + r.prospect.nome + "</b>. Sou da equipe de vendas da Pinheiro Sereni Engenharia. Estou à disposição para ajudá-lo(a). Você já foi atendido(a) por um de nossos corretores?"
                    };
                    db.ChatMessages.Add(msg);
                    #endregion

                    db.SaveChanges();

                    r.chat.chatId = chat.chatId;
                    r.path_fotoCorretor = db.Parametros.Find((int)Parametros.PATH_FOTOCORRETOR).valor;

                    return r;
                }
                catch (ArgumentNullException ex)
                {
                    chatRepository.mensagem = new Validate() { Code = 35, Message = MensagemPadrao.Message(35).ToString(), MessageBase = ex.Message };
                }
                catch (ArgumentException ex)
                {
                    chatRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    chatRepository.mensagem.Code = 17;
                    chatRepository.mensagem.MessageBase = ex.Message;
                    chatRepository.mensagem.Message = new PinheiroSereniException(ex.Message + " => " + ex.InnerException.Message, GetType().FullName).Message;
                }

            }
            return chatRepository;

        }

        public Repository Send(int chatId, string text)
        {
            ChatRepository chatRepository;
            using (db = Create())
            {
                chatRepository = new ChatRepository() { mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() } };
                try
                {
                    #region grava mensagem
                    PinheiroSereni.Dominio.Entidades.Chat ch = db.Chats.Find(chatId);
                    ChatMessage msg = new ChatMessage()
                    {
                        messageId = Guid.NewGuid(),
                        chatId = chatId,
                        email = ch.email,
                        empreendimentoId = ch.empreendimentoId,
                        dt_message = DateTime.Now,
                        message = text.Replace("'","")
                    };
                    db.ChatMessages.Add(msg);

                    _TypingClient(chatId, "N");

                    db.SaveChanges();


                    #endregion

                    return getRepository(chatId);
                }
                catch (ArgumentException ex)
                {
                    chatRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    chatRepository.mensagem.Code = 17;
                    chatRepository.mensagem.MessageBase = ex.Message;
                    chatRepository.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }

            return chatRepository;
        }

        public Repository ChatOver(int chatId)
        {
            ChatRepository chatRepository;
            using (db = Create())
            {
                chatRepository = new ChatRepository() { mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() } };
                try
                {
                    #region Finalizar Chat
                    PinheiroSereni.Dominio.Entidades.Chat chatEncerrar = db.Chats.Find(chatId);
                    if (chatEncerrar.dt_fim == null)  // o chat não foi encerrado pelo corretor
                    {
                        chatEncerrar.dt_fim = DateTime.Now;
                        db.Entry(chatEncerrar).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    #endregion
                }
                catch (ArgumentException ex)
                {
                    chatRepository.mensagem = new Validate() { Code = 17, Message = MensagemPadrao.Message(17).ToString(), MessageBase = ex.Message };
                }
                catch (Exception ex)
                {
                    chatRepository.mensagem.Code = 17;
                    chatRepository.mensagem.MessageBase = ex.Message;
                    chatRepository.mensagem.Message = new PinheiroSereniException(ex.Message, GetType().FullName).Message;
                }
            }
            return chatRepository;

        }

        private void _TypingClient(int chatId, string value)
        {
            PinheiroSereni.Dominio.Entidades.Chat chat = db.Chats.Find(chatId);
            chat.typingClient = value;
            db.Entry(chat).State = EntityState.Modified;
        }

        public void TypingClient(int chatId, string value)
        {
            using (db = new PinheiroSereniContext())
            {
                _TypingClient(chatId, value);
                db.SaveChanges();
            }
        }
        #endregion

        /// <summary>
        /// Este método verifica as sessões com mais de 15 minutos de inatividade.
        /// Para cada uma dessas sessões o sistema desativa as mesmas e os chats relacionados a elas.
        /// </summary>
        public void CleanInactiveSessions()
        {
            using (db = new PinheiroSereniContext())
            {
                DateTime dt_ref = DateTime.Now.AddMinutes(-15);
                IEnumerable<SessaoRepository> r = from s in db.Sessaos
                                                  where s.dt_atualizacao < dt_ref && s.dt_desativacao == null
                                                  select new SessaoRepository()
                                                  {
                                                      sessionId = s.sessaoId,
                                                      sessao = s,
                                                      chatRepositories = (from c in db.Chats
                                                                          where c.Sessao == s
                                                                          select new ChatRepository
                                                                          {
                                                                              chat = c
                                                                          })
                                                  };
                foreach (SessaoRepository sr in r)
                {
                    Sessao _s = db.Sessaos.Find(sr.sessionId);
                    _s.dt_desativacao = DateTime.Now;
                    db.Entry(_s).State = EntityState.Modified;

                    foreach (ChatRepository cr in sr.chatRepositories)
                    {
                        PinheiroSereni.Dominio.Entidades.Chat _chat = db.Chats.Find(cr.chat.chatId);
                        _chat.dt_fim = DateTime.Now;
                        db.Entry(_chat).State = EntityState.Modified;
                    }

                }

                if (r.Count() > 0)
                    db.SaveChanges();

            }


        }

        #region Métodos privados
        private void updateSession(string sessionId, string statusOperador)
        {
            Sessao sessao = db.Sessaos.Find(sessionId);
            sessao.statusOperador = statusOperador;
            sessao.dt_atualizacao = DateTime.Now;
            db.Entry(sessao).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void updateSession(string sessionId)
        {
            Sessao sessao = db.Sessaos.Find(sessionId);
            sessao.dt_atualizacao = DateTime.Now;
            db.Entry(sessao).State = EntityState.Modified;
            db.SaveChanges();
        }

        private Repository Create(string sessionId)
        {
            Sessao s1 = db.Sessaos.Find(sessionId);
            CorretorOnline c1 = db.CorretorOnlines.Find(s1.corretorId);
            SessaoRepository sessaoRepository = new SessaoRepository()
            {
                sessionId = sessionId,
                sessao = s1,
                corretorOnline = c1,
                mensagem = new Validate() 
            };

            return sessaoRepository;
        }
        #endregion

        #endregion



    }
}
