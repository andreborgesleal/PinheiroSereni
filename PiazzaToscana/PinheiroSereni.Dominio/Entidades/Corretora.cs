using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinheiroSereni.Dominio.Entidades
{
    [Table("Corretora")]
    public class Corretora
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage="Informe o Código da Corretora")]
        public int corretoraId { get; set; }

        [DisplayName("Corretora *")]
        [Required(ErrorMessage = "Por favor, informe o nome da corretora")]
        [StringLength(50, ErrorMessage = "O nome da corretora deve ter no máximo 50 caracteres")]
        public string nome { get; set; }

    }
}
