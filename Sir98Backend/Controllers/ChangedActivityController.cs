using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sir98Backend.Interfaces;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Repository;
using Sir98Backend.Services;
namespace Sir98Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChangedActivityController : ControllerBase
    {
        private readonly ActivityRepo _activityRepo;
        private readonly ChangedActivityRepo _changedActivityRepo;
        private readonly IOccurrenceSnapshotResolver _occurrenceResolver;
        private readonly ActivityNotificationPayloadBuilder _payloadBuilder;
        private readonly NotificationService _notificationService;

        public ChangedActivityController( ActivityRepo activityRepo, ChangedActivityRepo changedActivityRepo, IOccurrenceSnapshotResolver occurrenceResolver, NotificationService notificationService, ActivityNotificationPayloadBuilder payloadBuilder)
        {
           
            _activityRepo = activityRepo;
            _changedActivityRepo = changedActivityRepo;
            _occurrenceResolver = occurrenceResolver;
            _payloadBuilder = payloadBuilder;
            _notificationService = notificationService;
        }


        //[HttpGet()]
        //public async Task<ActionResult<EditOccurrenceDto>> GetForEdit(
        //[FromQuery] int activityId,
        //[FromQuery] DateTimeOffset originalStartUtc)
        //{
        //    var activity = await _activityRepo.GetByIdAsync(activityId);
        //    if (activity == null)
        //        return NotFound();

        //    var change = await _changedActivityRepo
        //        .GetByActivityAndOriginalStartAsync(activityId, originalStartUtc);

        //    // Brug eksisterende logik
        //    var start = change?.NewStartUtc ?? originalStartUtc;
        //    var end = change?.NewEndUtc ??
        //              originalStartUtc + (activity.EndUtc - activity.StartUtc);

        //    return Ok(new EditOccurrenceDto
        //    {
        //        ActivityId = activityId,
        //        OriginalStartUtc = originalStartUtc,
        //        StartUtc = start,
        //        EndUtc = end,
        //        Title = change?.NewTitle ?? activity.Title,
        //        Description = change?.NewDescription ?? activity.Description,
        //        Address = change?.NewAddress ?? activity.Address,
        //        Tag = change?.NewTag ?? activity.Tag,
        //        IsCancelled = change?.IsCancelled ?? false,
        //        InstructorIds = (change?.NewInstructors ?? activity.Instructors)
        //            .Select(i => i.Id)
        //            .ToList()
        //    });
        //}

        [HttpPost]
        public async Task<IActionResult> UpsertOccurrence([FromBody] EditOccurrenceDto dto)
        {
            // BEFORE: resolve what the user currently sees
            var before = await _occurrenceResolver.ResolveAsync(dto.ActivityId, dto.OriginalStartUtc);

            var existing = await _changedActivityRepo
                .GetByActivityAndOriginalStartAsync(dto.ActivityId, dto.OriginalStartUtc);

            if (existing == null)
            {
                existing = new ChangedActivity
                {
                    ActivityId = dto.ActivityId,
                    OriginalStartUtc = dto.OriginalStartUtc,
                    NewStartUtc = dto.StartUtc,
                    NewEndUtc = dto.EndUtc,
                    NewTitle = dto.Title,
                    NewDescription = dto.Description,
                    NewAddress = dto.Address,
                    NewTag = dto.Tag,
                    IsCancelled = dto.IsCancelled
                };

                _changedActivityRepo.Add(existing);
            }
            else
            {
                existing.NewStartUtc = dto.StartUtc;
                existing.NewEndUtc = dto.EndUtc;
                existing.NewTitle = dto.Title;
                existing.NewDescription = dto.Description;
                existing.NewAddress = dto.Address;
                existing.NewTag = dto.Tag;
                existing.IsCancelled = dto.IsCancelled;
            }

            await _changedActivityRepo.SaveAsync();

            // AFTER: resolve what the user will see now (base + updated override)
            var after = await _occurrenceResolver.ResolveAsync(dto.ActivityId, dto.OriginalStartUtc);

            // Build + notify
            var isSeries = true; // Since we are in ChangedActivity, it will always be a series change
            var payload = _payloadBuilder.BuildUpdatePayload(before, after, isSeries);

            await _notificationService.NotifyUsersAboutSeriesChangeAsync(dto.ActivityId, payload);

            return Ok();
        }


    }
}
