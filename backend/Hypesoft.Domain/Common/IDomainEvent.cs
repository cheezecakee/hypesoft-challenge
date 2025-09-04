namespace Hypesoft.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccuredOn { get; }
    }
}
