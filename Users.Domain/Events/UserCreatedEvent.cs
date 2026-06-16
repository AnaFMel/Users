namespace Users.Domain.Events
{
    internal class UserCreatedEvent
    {
        public UserCreatedEvent(int userId, string userName, string userEmail, int userRole)
        {
            UserId = userId;
            UserName = userName;
            UserEmail = userEmail;
            UserRole = userRole;
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int UserRole { get; set; }
    }
}
