using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("Empreendimento")]
    public class Empreendimento
    {
        [Key]
        public int empreendimentoId { get; set; }

        [DisplayName("Empreendimento")]
        [Required(ErrorMessage = "Por favor, informe o nome do empreendimento")]
        [StringLength(40, ErrorMessage = "O nome do empreendimento deve ter no máximo 40 caracteres")]
        public string nomeEmpreendimento { get; set; }

        [DisplayName("URL Folder Digital")]
        [StringLength(500, ErrorMessage = "A URL do Folder Digital deve ter no máximo 500 caracteres")]
        public string urlFolderDigital { get; set; }
    }
}
