namespace Users.Domain.Entities
{
    public class User : BaseEntity
    {

        public User(string name, string email, string password, int roleId)
        {
            Name = name;
            Email = email;
            Password = password;
            RoleId = roleId;
        }

        public User(int id, string name, string email, string password, int roleId)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            RoleId = roleId;
        }

        protected User() : base() { }

        public string Email { get; private set; } = null!;
        public string Password { get; private set; } = null!;
        public int RoleId { get; private set; }
        public Role? Role { get; private set; }

        public void SetPassword(string passwordHash)
        {
            Password = passwordHash;
        }
    }
}
