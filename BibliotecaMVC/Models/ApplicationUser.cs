using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        // Relación con Préstamos
        public virtual ICollection<Prestamo> Prestamos { get; set; }
    }
}