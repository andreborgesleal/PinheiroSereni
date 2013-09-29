using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("ChatMessage")]
    public class ChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "Informe o ID da mensagem")]
        public System.Guid messageId { get; set; }

        [DisplayName("ID do Chat")]
        [Required(ErrorMessage = "Informe o ID do Chat")]
        public int chatId { get; set; }

        [DisplayName("ID do Corretor")]
        public int? corretorId { get; set; }

        [DisplayName("E-Mail Cliente")]
        public string email { get; set; }

        [DisplayName("Empreendimento")]
        public int empreendimentoId { get; set; }

        [DisplayName("Dt.Mensagem")]
        [Required(ErrorMessage = "Informe a Data de Mensagem")]
        public DateTime dt_message { get; set; }

        [DisplayName("Mensagem")]
        [Required(ErrorMessage = "Por favor, informe o texto da mensagem a ser enviado")]
        public string message { get; set; }

        public virtual Chat Chat { get; set; }
        public virtual Prospect Prospect { get; set; }
        public virtual CorretorOnline CorretorOnline { get; set; }
        public virtual Empreendimento Empreendimento { get; set; }
    }
}
