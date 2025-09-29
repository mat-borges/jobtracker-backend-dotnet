using JobTracker.Domain.Common;
using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities
{
    public class ApplicationEvent : AuditableEntity
    {
        protected ApplicationEvent() { }

        public ApplicationEvent(Guid applicationId,
                                ApplicationStage? previousStage,
                                ApplicationStage? newStage,
                                ApplicationStatus? previousStatus,
                                ApplicationStatus? newStatus,
                                string? note)
        {
            Id = Guid.NewGuid();
            ApplicationId = applicationId;
            PreviousStage = previousStage;
            NewStage = newStage;
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            Note = note;
            SetCreatedNow();
            SetUpdatedNow();
        }

        public Guid Id { get; private set; }
        public Guid ApplicationId { get; private set; }
        public ApplicationStage? PreviousStage { get; private set; }
        public ApplicationStage? NewStage { get; private set; }
        public ApplicationStatus? PreviousStatus { get; private set; }
        public ApplicationStatus? NewStatus { get; private set; }
        public string? Note { get; private set; }
        public DateTime OccurredAt => CreatedAt;
    }
}
