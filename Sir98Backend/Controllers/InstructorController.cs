using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models;
using Sir98Backend.Repository;

namespace Sir98Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorRepo _instructorRepo;

        public InstructorController(InstructorRepo instructorRepo)
        {
            _instructorRepo = instructorRepo;
        }

        // GET: api/Instructor
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Instructor>>> GetAll()
        {
            var instructors = await _instructorRepo.GetAllAsync();
            return Ok(instructors);
        }

        // GET: api/Instructor/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Instructor>> GetById(int id)
        {
            var instructor = await _instructorRepo.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            return Ok(instructor);
        }

        // POST: api/Instructor
        [HttpPost]
        public async Task<ActionResult<Instructor>> Post([FromBody] Instructor instructor)
        {
            if (instructor == null)
                return BadRequest("Body is required.");

            var created = await _instructorRepo.AddAsync(instructor);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        // PUT: api/Instructor/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Instructor>> Put(int id, [FromBody] Instructor instructor)
        {
            if (instructor == null)
                return BadRequest("Body is required.");

            var updated = await _instructorRepo.UpdateAsync(id, instructor);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: api/Instructor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _instructorRepo.DeleteAsync(id);
            if (deleted == null)
                return NotFound();

            return NoContent();
        }
    }
}
