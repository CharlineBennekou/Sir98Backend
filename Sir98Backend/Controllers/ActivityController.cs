using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models;
using Sir98Backend.Repository;
using Sir98Backend.Data;
using Microsoft.EntityFrameworkCore;
using Sir98Backend.Services;
using Sir98Backend.Models.DataTransferObjects;

namespace Sir98Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityRepo _activityRepo;
        private readonly AppDbContext _context;
        private readonly ActivityService _activityService;
        private readonly InstructorRepo _instructorRepo;

        public ActivityController(ActivityRepo activityRepo, AppDbContext context, ActivityService activityService, InstructorRepo instructorRepo)
        {
            _activityRepo = activityRepo ?? throw new ArgumentNullException(nameof(activityRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            _instructorRepo = instructorRepo ?? throw new ArgumentNullException(nameof(instructorRepo));
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
        public async Task<ActionResult<Activity>> Post([FromBody] ActivityDto activity)
        {
            if (activity == null)
                return BadRequest("Body is required.");
            
            

            var created = await _activityRepo.AddAsync(await MapActivityDTO(activity));

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        // PUT: api/Activity/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Activity>> Put(int id, [FromBody] ActivityDto activity)
        {
            if (activity == null)
                return BadRequest("Body is required.");

            var updated = await _activityRepo.UpdateAsync(id, await MapActivityDTO(activity));
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        private async Task<Activity> MapActivityDTO(ActivityDto activity)
        {
            List<Instructor> instructors = new();
            List<Instructor> availableInstructors = await _instructorRepo.GetAllAsyncTracking();
            foreach (int instructorID in activity.InstructorIds)
            {
                instructors.Add(availableInstructors.FirstOrDefault((instructor) => instructor.Id == instructorID));
            }
            Activity newActivity = new()
            {
                Title = activity.Title,
                StartUtc = activity.StartUtc,
                EndUtc = activity.EndUtc,
                Address = activity.Address,
                Image = activity.Image,
                Link = activity.Link,
                Cancelled = activity.Cancelled,
                Description = activity.Description,
                Instructors = instructors,
                Tag = activity.Tag,
                IsRecurring = activity.IsRecurring,
                Rrule = activity.Rrule,
            };
            return newActivity;
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

        // PUT: api/Activity/5/PutAndNotify
        [HttpPut("{id}/PutAndNotify")]
        public async Task<ActionResult<Activity>> PutAndNotify(int id, [FromBody] Activity activity)
        {
            if (activity == null)
                return BadRequest("Body is required.");

            var updated = await _activityService.PutAndNotifyAsync(id, activity);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: api/Activity/5/DeleteAndNotify
        [HttpDelete("{id}/DeleteAndNotify")]
        public async Task<IActionResult> DeleteAndNotify(int id)
        {
            var deleted = await _activityService.DeleteAndNotifyAsync(id);
            if (deleted == null)
                return NotFound();

            return NoContent();
        }

    }
}
