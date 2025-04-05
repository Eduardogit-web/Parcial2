using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; }

        public List<string> AllRoles { get; set; }

        [Display(Name = "Roles seleccionados")]
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}
