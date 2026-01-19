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
            var isSeries = false; // Since we are in ChangedActivity, it will always be a change to a single occurrence
            var payload = _payloadBuilder.BuildUpdatePayload(before, after, isSeries);

            await _notificationService.NotifyUsersAboutSeriesChangeAsync(dto.ActivityId, payload);

            return Ok();
        }


    }
}
