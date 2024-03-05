using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppLoginA.Models
{
    [Table("Logs")]
    public class RegistroLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idLog { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Ingrese una dirección de correo válida")]
        public string? Correo { get; set; }

        public bool? IsRegisteredToken { get; set; }
    }
}
