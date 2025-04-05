using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMVC.Utils;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class PrestamosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrestamosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Prestamos
        [Authorize(Roles = RolesApp.Admin + "," + RolesApp.Bibliotecario)]
        public async Task<IActionResult> Index()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .ToListAsync();
            return View(prestamos);
        }

        // GET: Mis Prestamos (para lectores)
        [Authorize(Roles = RolesApp.Lector)]
        public async Task<IActionResult> MisPrestamos()
        {
            var userId = _userManager.GetUserId(User);
            var misPrestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Where(p => p.UsuarioId == userId)
                .ToListAsync();
            return View(misPrestamos);
        }

        // GET: Prestamos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prestamo == null)
            {
                return NotFound();
            }

            // Solo admin, bibliotecario o el usuario dueño del préstamo pueden ver detalles
            if (!User.IsInRole(RolesApp.Admin) &&
                !User.IsInRole(RolesApp.Bibliotecario) &&
                prestamo.UsuarioId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(prestamo);
        }

        // GET: Prestamos/Create
        public IActionResult Create()
        {
            // Solo mostrar libros disponibles
            ViewData["LibroId"] = new SelectList(_context.Libros.Where(l => l.Disponible), "Id", "Titulo");

            // Si es admin o bibliotecario, puede seleccionar cualquier usuario
            if (User.IsInRole(RolesApp.Admin) || User.IsInRole(RolesApp.Bibliotecario))
            {
                ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Email");
            }

            // Por defecto la fecha de devolución es 15 días después
            ViewData["FechaDevolucionPredeterminada"] = DateTime.Now.AddDays(15).ToString("yyyy-MM-dd");

            return View();
        }

        // POST: Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaPrestamo,FechaDevolucion,LibroId,UsuarioId")] Prestamo prestamo)
        {
            // Validación adicional - Fecha devolución > Fecha préstamo
            if (prestamo.FechaDevolucion <= prestamo.FechaPrestamo)
            {
                ModelState.AddModelError("FechaDevolucion", "La fecha de devolución debe ser posterior a la fecha de préstamo");
            }

            // Si es lector, asignar el ID del usuario actual
            if (User.IsInRole(RolesApp.Lector))
            {
                prestamo.UsuarioId = _userManager.GetUserId(User);
            }

            if (ModelState.IsValid)
            {
                // Verificar que el libro esté disponible
                var libro = await _context.Libros.FindAsync(prestamo.LibroId);
                if (libro == null || !libro.Disponible)
                {
                    ModelState.AddModelError("LibroId", "El libro no está disponible para préstamo");
                    ViewData["LibroId"] = new SelectList(_context.Libros.Where(l => l.Disponible), "Id", "Titulo");

                    if (User.IsInRole(RolesApp.Admin) || User.IsInRole(RolesApp.Bibliotecario))
                    {
                        ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Email");
                    }

                    return View(prestamo);
                }

                // Actualizar estado del libro
                libro.Disponible = false;

                // Guardar préstamo
                _context.Add(prestamo);
                await _context.SaveChangesAsync();

                if (User.IsInRole(RolesApp.Lector))
                {
                    return RedirectToAction(nameof(MisPrestamos));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["LibroId"] = new SelectList(_context.Libros.Where(l => l.Disponible), "Id", "Titulo", prestamo.LibroId);

            if (User.IsInRole(RolesApp.Admin) || User.IsInRole(RolesApp.Bibliotecario))
            {
                ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Email", prestamo.UsuarioId);
            }

            return View(prestamo);
        }

        // Devolver libro
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesApp.Admin + "," + RolesApp.Bibliotecario)]
        public async Task<IActionResult> DevolverLibro(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo == null)
            {
                return NotFound();
            }

            // Actualizar disponibilidad del libro
            prestamo.Libro.Disponible = true;

            // Guardar cambios
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}