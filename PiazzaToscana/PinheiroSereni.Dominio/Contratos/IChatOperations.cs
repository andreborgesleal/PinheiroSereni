using PinheiroSereni.Dominio.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinheiroSereni.Dominio.Contratos
{
    public interface IChatOperations
    {
        Repository getSessao(string sessionId, int? chatId = null);
        Repository ping(string sessionId);
        Repository changeStatusOperador(string sessionId, string newStatus);
        IEnumerable<Repository> listening(string sessionId, int? chatId);
        Repository ActivateEdition(string sessionId, int chatId);
        Repository Start(Repository chatRepository);
        Repository Finish(string sessionId, int chatId);
        Repository Send(string sessionId, int chatId, string text);
        Repository Send(int chatId, string text);
        void Exit(string sessionId);
        Repository ChatOver(int chatId);
        void CleanInactiveSessions();
    }
}
