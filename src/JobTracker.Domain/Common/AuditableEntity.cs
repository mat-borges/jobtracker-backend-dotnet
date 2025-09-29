namespace JobTracker.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public void SetCreatedNow() => CreatedAt = DateTime.UtcNow;
        public void SetUpdatedNow() => UpdatedAt = DateTime.UtcNow;
    }
}