using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Models
{
    public class Prestamo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de préstamo")]
        [DataType(DataType.Date)]
        public DateTime FechaPrestamo { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Fecha de devolución")]
        [DataType(DataType.Date)]
        public DateTime FechaDevolucion { get; set; }

        [Required]
        public int LibroId { get; set; }

        [ForeignKey("LibroId")]
        public virtual Libro Libro { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; }

        // Validación para FechaDevolucion > FechaPrestamo
        [NotMapped]
        public bool EsFechaDevolucionValida => FechaDevolucion > FechaPrestamo;
    }
}

