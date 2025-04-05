using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, MinimumLength = 2)]
        public string Titulo { get; set; }

        [Display(Name = "Año de publicación")]
        [Range(1000, 2100, ErrorMessage = "El año debe estar entre 1000 y 2100")]
        public int AnioPublicacion { get; set; }

        [Display(Name = "Disponible")]
        public bool Disponible { get; set; } = true;

        // Relaciones
        [Required]
        [Display(Name = "Autor")]
        public int AutorId { get; set; }

        [ForeignKey("AutorId")]
        public virtual Autor Autor { get; set; }

        [Required]
        [Display(Name = "Género")]
        public int GeneroId { get; set; }

        [ForeignKey("GeneroId")]
        public virtual Genero Genero { get; set; }

        public virtual ICollection<Prestamo> Prestamos { get; set; }
    }
}
