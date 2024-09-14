using Assignment_2.Models;
using Assignment_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoService _toDoService;

        public ToDoController(ToDoService toDoService)
        {
            _toDoService = toDoService ?? throw new ArgumentNullException(nameof(toDoService));
        }

        // GET: api/todo
        [HttpGet]
        public async Task<ActionResult<List<ToDo>>> GetAllToDos()
        {
            var toDos = await _toDoService.GetAllToDos();
            return Ok(toDos);
        }

        // GET: api/todo/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> GetToDoById(int id)
        {
            var toDo = await _toDoService.GetToDoById(id);
            if (toDo == null)
            {
                return NotFound();
            }
            return Ok(toDo);
        }

        // POST: api/todo
        [HttpPost]
        public async Task<ActionResult<int>> CreateToDo(ToDo toDo)
        {
            var newToDoId = await _toDoService.InsertToDo(toDo);
            return CreatedAtAction(nameof(GetToDoById), new { id = newToDoId }, newToDoId);
        }

        // PUT: api/todo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateToDo(int id, ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return BadRequest();
            }

            var existingToDo = await _toDoService.GetToDoById(id);
            if (existingToDo == null)
            {
                return NotFound();
            }

            await _toDoService.UpdateToDo(toDo);
            return NoContent();
        }

        // DELETE: api/todo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDo(int id)
        {
            var existingToDo = await _toDoService.GetToDoById(id);
            if (existingToDo == null)
            {
                return NotFound();
            }

            await _toDoService.DeleteToDo(id);
            return NoContent();
        }
    }
}
