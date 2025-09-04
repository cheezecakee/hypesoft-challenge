namespace Hypesoft.Domain.Common
{

    public abstract class BaseEntity
    {
        public string Id { get; protected set; } = string.Empty;
        public DateTime CreateAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
            CreateAt = DateTime.UtcNow;
        }

        protected BaseEntity(string id)
        {
            Id = id;
            CreateAt = DateTime.UtcNow;
        }

        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

