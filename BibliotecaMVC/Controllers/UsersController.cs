using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Utils;
using BibliotecaMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = RolesApp.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nombre = user.Nombre,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Nombre = user.Nombre,
                UserRoles = [.. userRoles],
                AllRoles = allRoles
            };

            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Nombre = model.Nombre;
                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Actualizar roles
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // Quitar roles que ya no están seleccionados
                    foreach (var role in userRoles)
                    {
                        if (!model.SelectedRoles.Contains(role))
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }
                    }

                    // Agregar nuevos roles seleccionados
                    foreach (var role in model.SelectedRoles)
                    {
                        if (!userRoles.Contains(role))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            model.AllRoles = allRoles;
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Nombre = user.Nombre,
                Roles = roles.ToList()
            };

            return View(viewModel);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Verificar si tiene préstamos
            var tienePrestamos = await _userManager.Users
                .Include(u => ((ApplicationUser)u).Prestamos)
                .AnyAsync(u => u.Id == id && ((ApplicationUser)u).Prestamos.Any());

            if (tienePrestamos)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var viewModel = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nombre = user.Nombre,
                    Roles = roles.ToList()
                };

                ViewBag.ErrorMessage = "No se puede eliminar el usuario porque tiene préstamos activos.";
                return View(viewModel);
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
