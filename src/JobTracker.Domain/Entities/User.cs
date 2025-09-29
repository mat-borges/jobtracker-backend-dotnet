using JobTracker.Domain.Common;

namespace JobTracker.Domain.Entities
{
    public class User : AuditableEntity
    {
        protected User() { }
        public User(Guid id, string email, string passwordHash)
        {
            Id = id;
            Email = email;
            PasswordHash = passwordHash;
            IsActive = true;
            SetCreatedNow();
            SetUpdatedNow();
        }

        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Role { get; private set; } = "User";
        public bool IsActive { get; private set; }

        // navigation
        public ICollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();

        public void Deactivate() => IsActive = false;
    }
}
