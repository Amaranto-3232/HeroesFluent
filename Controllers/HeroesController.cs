using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeroesFluent.Data;
using HeroesFluent.Models;

namespace HeroesFluent.Controllers
{
    public class HeroesController : Controller
    {
        private readonly HeroesDbContext _context;

        public HeroesController(HeroesDbContext context)
        {
            _context = context;
        }

        private void CargarEquipos(int? equipoSeleccionado = null)
        {
            ViewData["EquipoId"] = new SelectList(
                _context.Equipos.AsNoTracking().OrderBy(e => e.Nombre),
                "Id",
                "Nombre",
                equipoSeleccionado);
        }

        // GET: Heroes
        public async Task<IActionResult> Index()
        {
            var heroes = await _context.Heroes
                .Include(h => h.Equipo)
                .ToListAsync();
            return View(heroes);
        }

        // GET: Heroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heroe = await _context.Heroes
                .Include(h => h.Equipo)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (heroe == null)
            {
                return NotFound();
            }

            return View(heroe);
        }

        // GET: Heroes/Create
        public IActionResult Create()
        {
            CargarEquipos();
            return View();
        }

        // POST: Heroes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Nombre,Alias,NivelPoder,FechaRegistro,EquipoId")] Heroe heroe)
        {
            if (heroe.EquipoId.HasValue &&
                !await _context.Equipos.AnyAsync(e => e.Id == heroe.EquipoId.Value))
            {
                ModelState.AddModelError(nameof(Heroe.EquipoId),
                    "El equipo seleccionado no existe.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(heroe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarEquipos(heroe.EquipoId);
            return View(heroe);
        }

        // GET: Heroes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heroe = await _context.Heroes.FindAsync(id.Value);
            if (heroe == null)
            {
                return NotFound();
            }

            CargarEquipos(heroe.EquipoId);
            return View(heroe);
        }

        // POST: Heroes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Nombre,Alias,NivelPoder,FechaRegistro,EquipoId")] Heroe heroe)
        {
            if (id != heroe.Id)
            {
                return NotFound();
            }

            if (heroe.EquipoId.HasValue &&
                !await _context.Equipos.AnyAsync(e => e.Id == heroe.EquipoId.Value))
            {
                ModelState.AddModelError(nameof(Heroe.EquipoId),
                    "El equipo seleccionado no existe.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(heroe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Heroes.AnyAsync(h => h.Id == heroe.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            CargarEquipos(heroe.EquipoId);
            return View(heroe);
        }

        // GET: Heroes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heroe = await _context.Heroes
                .Include(h => h.Equipo)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (heroe == null)
            {
                return NotFound();
            }

            return View(heroe);
        }

        // POST: Heroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var heroe = await _context.Heroes.FindAsync(id);
            if (heroe != null)
            {
                _context.Heroes.Remove(heroe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HeroeExists(int id)
        {
            return _context.Heroes.Any(e => e.Id == id);
        }
    }
}
