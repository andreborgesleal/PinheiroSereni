using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("Chat")]
    public class Chat
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Informe o ID do Chat")]
        public int chatId { get; set; }

        [DisplayName("ID Sessao")]
        [Required(ErrorMessage = "Informe o ID da Sessão")]
        public string sessaoId { get; set; }

        [DisplayName("ID Corretor")]
        [Required(ErrorMessage = "Informe o ID do Corretor")]
        public int corretorId { get; set; }

        [DisplayName("Dt.Início")]
        [Required(ErrorMessage = "Informe a data de início do chat")]
        public DateTime dt_inicio { get; set; }

        [DisplayName("Dt.Fim")]
        public DateTime? dt_fim { get; set; }

        [DisplayName("Empreendimento")]
        public int empreendimentoId { get; set; }

        [DisplayName("E-Mail *")]
        [StringLength(200, ErrorMessage = "O e-mail deve ter no máximo 200 caracteres")]
        [Required(ErrorMessage = "Por favor, informe o e-mail do cliente")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "E-mail inválido.")]
        public string email { get; set; }

        public string typingClient { get; set; }
        public string typingOperator { get; set; }

        public virtual Sessao Sessao { get; set; }
        public virtual Prospect Prospect { get; set; }
        public virtual CorretorOnline CorretorOnline { get; set; }
        public virtual Empreendimento Empreendimento { get; set; }

    }
}
