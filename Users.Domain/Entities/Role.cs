namespace Users.Domain.Entities
{
    public class Role : BaseEntity
    {
        protected Role() : base() { }
        public Role(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
