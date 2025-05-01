using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfigurationWebApp.Data;
using ConfigurationWebApp.Models;

namespace ConfigurationWebApp.Controllers
{
    public class ConfigurationItemsController : Controller
    {
        private readonly AppDbContext _context;

        public ConfigurationItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ConfigurationItems
        public async Task<IActionResult> Index()
        {
            var list = await _context.ConfigurationSettings.ToListAsync();
            return View(list);
        }


        // GET: ConfigurationItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ConfigurationItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfigurationItem configurationItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(configurationItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(configurationItem);
        }

        // GET: ConfigurationItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configurationItem = await _context.ConfigurationSettings.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (configurationItem == null)
            {
                return NotFound();
            }
            return View(configurationItem);
        }

        // POST: ConfigurationItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConfigurationItem configurationItem)
        {
            if (id != configurationItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(configurationItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConfigurationItemExists(configurationItem.Id))
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
            return View(configurationItem);
        }

        // GET: ConfigurationItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configurationItem = await _context.ConfigurationSettings.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (configurationItem == null)
            {
                return NotFound();
            }

            return View(configurationItem);
        }

        // POST: ConfigurationItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var configurationItem = await _context.ConfigurationSettings.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (configurationItem != null)
            {
                _context.ConfigurationSettings.Remove(configurationItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Güncelleme (Edit) sırasında olası eşzamanlı güncelleme hatalarını (concurrency exceptions) yönetmek için yazılmıştır.
        private bool ConfigurationItemExists(int id)
        {
            return _context.ConfigurationSettings.Any(l => l.Id == id);
        }
    }
}
