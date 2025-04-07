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

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = RolesApp.Admin + "," + RolesApp.Bibliotecario)]
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Libros
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Genero)
                .ToListAsync();
            return View(libros);
        }

        // GET: Libros disponibles (para préstamos)
        [Authorize(Roles = RolesApp.Lector + "," + RolesApp.Admin + "," + RolesApp.Bibliotecario)]
        public async Task<IActionResult> Disponibles()
        {
            var librosDisponibles = await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Genero)
                .Where(l => l.Disponible)
                .ToListAsync();
            return View(librosDisponibles);
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Genero)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libros/Create
        public IActionResult Create()
        {
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre");
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre");
            return View();
        }

        // POST: Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,AnioPublicacion,AutorId,GeneroId,Disponible")] Libro libro)
        {
            /* if (ModelState.IsValid)
             {
                 _context.Add(libro);
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));
             }*/

            try
            {
                _context.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                return View(libro);
            }
           /* ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre", libro.AutorId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(libro);*/
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound();
            }

            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre", libro.AutorId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,AnioPublicacion,AutorId,GeneroId,Disponible")] Libro libro)
        {
            if (id != libro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre", libro.AutorId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(libro);
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Genero)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro != null)
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.Id == id);
        }
    }
}