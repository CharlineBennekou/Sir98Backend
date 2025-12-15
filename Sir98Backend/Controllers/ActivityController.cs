using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models;
using Sir98Backend.Repository;
using Sir98Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Sir98Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityRepo _activityRepo;
        private readonly AppDbContext _context;

        public ActivityController(ActivityRepo activityRepo, AppDbContext context)
        {
            _activityRepo = activityRepo;
            _context = context; 
        }

        // GET: api/Activity/Context
        [HttpGet("Context")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetAllContext()
        {
            var activities = await _context.Activities
       .AsNoTracking()
       .Include(a => a.Instructors)
       .Include(a => a.ActivitySubscriptions)
       .Include(a => a.ChangedActivity)
       .ToListAsync();
            return Ok(activities);
        }

        // GET: api/Activity
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetAll()
        {
            var activities = await _activityRepo.GetAllInclAllAsync();
            return Ok(activities);
        }

        // GET: api/Activity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetById(int id)
        {
            var activity = await _activityRepo.GetByIdAsync(id);
            if (activity == null)
                return NotFound();

            return Ok(activity);
        }

        // POST: api/Activity
        [HttpPost]
        public async Task<ActionResult<Activity>> Post([FromBody] Activity activity)
        {
            if (activity == null)
                return BadRequest("Body is required.");

            var created = await _activityRepo.AddAsync(activity);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        // PUT: api/Activity/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Activity>> Put(int id, [FromBody] Activity activity)
        {
            if (activity == null)
                return BadRequest("Body is required.");

            var updated = await _activityRepo.UpdateAsync(id, activity);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _activityRepo.DeleteAsync(id);
            if (deleted == null)
                return NotFound();

            return NoContent();
        }
    }
}
