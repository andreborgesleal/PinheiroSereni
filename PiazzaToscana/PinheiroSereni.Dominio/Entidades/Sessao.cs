using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("Sessao")]
    public class Sessao
    {
        [Key]
        [DisplayName("ID Sessão")]
        [Required(ErrorMessage="ID da sessão deve ser informado")]
        public string sessaoId { get; set; }

        public int? corretorId { get; set; }

        [DisplayName("Dt.Ativação")]
        [Required(ErrorMessage = "Data de ativação da sessão deve ser informada")]
        public DateTime dt_ativacao { get; set; }

        [DisplayName("Dt.Atualização")]
        [Required(ErrorMessage = "Data da atualização da sessão deve ser informada")]
        public DateTime dt_atualizacao { get; set; }

        [DisplayName("Dt.Desativação")]
        public DateTime? dt_desativacao { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage="Status deve sr informado")]
        public string statusOperador { get; set; }

        public virtual CorretorOnline CorretorOnline { get; set; }

    }
}
