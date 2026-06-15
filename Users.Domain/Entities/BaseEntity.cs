namespace Users.Domain.Entities
{
    public abstract class BaseEntity
    {
        protected BaseEntity() { }

        public int Id { get; protected set; }
        public string Name { get; protected set; } = null!;
        public char Status { get; private set; }
        public DateTime CreatedAt { get; protected set; }

        public void Deactivate() => Status = 'I';

    }
}
