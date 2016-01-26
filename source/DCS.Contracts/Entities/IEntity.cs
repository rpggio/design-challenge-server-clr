namespace DCS.Contracts.Entities
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}