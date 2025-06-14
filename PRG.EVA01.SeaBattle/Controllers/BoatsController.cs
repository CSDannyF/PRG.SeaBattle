using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BoatsController : Controller
    {
        private readonly SeaBattleDbContext _context;

        public BoatsController(SeaBattleDbContext context)
        {
            _context = context;
        }

        // GET: Boats
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                var boatContext = _context.Boats.Include(a => a.Game).ThenInclude(a => a.Boats).ThenInclude(a => a.Location).Where(a => a.GameId == id);
                return View(await boatContext.ToListAsync());
            }
            else
            {
                var seaBattleDbContext = _context.Boats.Include(b => b.Game).Include(b => b.Location);
                return View(await seaBattleDbContext.ToListAsync());
            }
                
        }

        // GET: Boats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boats
                .Include(b => b.Game)
                .Include(b => b.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        // GET: Boats/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Id");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Id");
            return View();
        }

        // POST: Boats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LocationId,Status,GameId")] Boat boat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Id", boat.GameId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Id", boat.LocationId);
            return View(boat);
        }

        // GET: Boats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boats.FindAsync(id);
            if (boat == null)
            {
                return NotFound();
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Id", boat.GameId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Id", boat.LocationId);
            return View(boat);
        }

        // POST: Boats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LocationId,Status,GameId")] Boat boat)
        {
            if (id != boat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatExists(boat.Id))
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
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Id", boat.GameId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Id", boat.LocationId);
            return View(boat);
        }

        // GET: Boats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boat = await _context.Boats
                .Include(b => b.Game)
                .Include(b => b.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boat == null)
            {
                return NotFound();
            }

            return View(boat);
        }

        // POST: Boats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boat = await _context.Boats.FindAsync(id);
            if (boat != null)
            {
                _context.Boats.Remove(boat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoatExists(int id)
        {
            return _context.Boats.Any(e => e.Id == id);
        }
    }
}
