using Assignment_1.Models;
using Assignment_1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ToDoService _toDoService;
        private readonly CategoryService _categoryService;

        public ToDoController(ToDoService toDoService, CategoryService categoryService)
        {
            _toDoService = toDoService ?? throw new ArgumentNullException(nameof(toDoService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        public async Task<IActionResult> Index()
        {
            var todos = await _toDoService.GetAllToDos();
            return View(todos);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ToDo toDo)
        {
            await _toDoService.InsertToDo(toDo);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var toDo = await _toDoService.GetToDoById(id);
            if (toDo == null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;
            return View(toDo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ToDo toDo)
        {
            await _toDoService.UpdateToDo(toDo);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var toDo = await _toDoService.GetToDoById(id);
            if (toDo == null)
            {
                return NotFound();
            }
            return View(toDo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _toDoService.DeleteToDo(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
