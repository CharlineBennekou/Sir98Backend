using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models;
using Sir98Backend.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sir98Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorRepo _instructorRepo;

        public InstructorController(InstructorRepo instructorRepo)
        {
            _instructorRepo = instructorRepo;
        }

        // GET: api/Instructor
        [HttpGet]
        public ActionResult<IEnumerable<Instructor>> GetAll()
        {
            return Ok(_instructorRepo.GetAll());
        }

        // GET: api/Instructor/5
        [HttpGet("{id}")]
        public ActionResult<Instructor> GetById(int id)
        {
            var instructor = _instructorRepo.GetById(id);
            if (instructor == null)
                return NotFound();
            return Ok(instructor);
        }

        // POST: api/Instructor
        [HttpPost]
        public ActionResult<Instructor> Post([FromBody] Instructor instructor)
        {
            var created = _instructorRepo.Add(instructor);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Instructor/5
        [HttpPut("{id}")]
        public ActionResult<Instructor> Put(int id, [FromBody] Instructor instructor)
        {
            var updated = _instructorRepo.Update(id, instructor);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        // DELETE: api/Instructor/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var deleted = _instructorRepo.Delete(id);
            if (deleted == null)
                return NotFound();
            return NoContent();
        }
    }
}
