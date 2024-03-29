﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLoginA.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idUsuario { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Ingrese una dirección de correo válida")]
        public string? Correo { get; set; }

        [StringLength(1000)]
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string? Password { get; set; }

        [Column("idRol")]
        public int idRol { get; set; }
        //Relacion muchos a uno 
        [ForeignKey("idRol")]
        public virtual Rol? Roles { get; set; }

    }
}
