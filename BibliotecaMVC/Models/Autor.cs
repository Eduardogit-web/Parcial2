using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Autor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del autor es obligatorio")]
        [StringLength(100, MinimumLength = 2)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Nacionalidad { get; set; }

        // Relación con Libros
        public virtual ICollection<Libro> Libros { get; set; }
    }
}
