using Riok.Mapperly.Abstractions;
using Users.API.DTOs;
using Users.Domain.Entities;

namespace Users.API.Profiles
{
    [Mapper]
    public partial class Mapper
    {
        [MapperIgnoreSource(nameof(User.RoleId))]
        [MapperIgnoreSource(nameof(User.Status))]
        [MapperIgnoreSource(nameof(User.Password))]
        public partial UserResponse Map(User source);

        public partial User Map(CreateUserRequest source);

        public partial IEnumerable<UserResponse> Map(IEnumerable<User> source);
    }
}
