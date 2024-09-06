using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ola1_Christoffer_Jonathan.Controllers {
    public class ToDoController : Controller {
        private readonly ApplicationDbContext _context;

        public ToDoController(ApplicationDbContext context) {
            _context = context;
        }

        // GET: ToDo
        public async Task<IActionResult> Index() {
            var toDoItems = await _context.ToDoItems.ToListAsync();
            return View(toDoItems);
        }

        // GET: ToDo/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null) {
                return NotFound();
            }

            return View(toDoItem);
        }

        // GET: ToDo/Create
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoItem toDoItem) {
            if (ModelState.IsValid) {
                _context.Add(toDoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toDoItem);
        }

        // GET: ToDo/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null) {
                return NotFound();
            }
            return View(toDoItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ToDoItem toDoItem) {
            if (id != toDoItem.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                _context.Update(toDoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toDoItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem != null) {
                _context.ToDoItems.Remove(toDoItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoItemExists(int id) {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}
