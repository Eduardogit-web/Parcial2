using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Genero
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del género es obligatorio")]
        [StringLength(50, MinimumLength = 2)]
        public string Nombre { get; set; }

        // Relación con Libros
        public virtual ICollection<Libro> Libros { get; set; }
    }
}
