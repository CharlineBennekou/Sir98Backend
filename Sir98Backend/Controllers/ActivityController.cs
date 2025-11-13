using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models;
using Sir98Backend.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sir98Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityRepo _activityRepo;

        public ActivityController(ActivityRepo activityRepo)
        {
            _activityRepo = activityRepo;
        }

        // GET: api/Activity
        [HttpGet]
        public ActionResult<IEnumerable<Activity>> GetAll()
        {
            return Ok(_activityRepo.GetAll());
        }

        // GET: api/Activity/5
        [HttpGet("{id}")]
        public ActionResult<Activity> GetById(int id)
        {
            var activity = _activityRepo.GetById(id);
            if (activity == null)
                return NotFound();
            return Ok(activity);
        }

        // POST: api/Activity
        [HttpPost]
        public ActionResult<Activity> Post([FromBody] Activity activity)
        {
            var created = _activityRepo.Add(activity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Activity/5
        [HttpPut("{id}")]
        public ActionResult<Activity> Put(int id, [FromBody] Activity activity)
        {
            var updated = _activityRepo.Update(id, activity);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var deleted = _activityRepo.Delete(id);
            if (deleted == null)
                return NotFound();
            return NoContent();
        }
    }
}
