using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinheiroSereni.Negocio.Repositories.Chat
{
    public class ChatRepository : Repository
    {
        public string isOnline { get; set; }
        public string isAtivo { get; set; }
        public string path_fotoCorretor { get; set; }
        public Prospect prospect { get; set; }
        public Sessao sessao { get; set; }
        public CorretorOnline corretor { get; set; }          
        public string nome_cliente { get; set; }
        public string nome_empreendimento { get; set; }
        public PinheiroSereni.Dominio.Entidades.Chat chat { get; set; }
        public IEnumerable<ChatMessage> chatMessages { get; set; }
        public ICorretorDaVez<PinheiroSereni.Dominio.Entidades.Chat> corretorDaVez { get; set; }
        public string conversa
        {
            get
            {
                string talk = "";
                foreach (ChatMessage c in chatMessages)
                {
                    if (c.email != null) // mensagem do cliente
                        talk += "[" + c.dt_message.ToString("HH:mm") + "] [<b>" + nome_cliente + "</b>] " + c.message ;
                    else
                        talk += "[" + c.dt_message.ToString("HH:mm") + "] [" + corretor.nome + "] " + c.message ;
                    talk += "<br />";
                }
                talk += "<a name=\"final\"></a>";
                return talk;
            }
        }
    }
}
