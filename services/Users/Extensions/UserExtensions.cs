using System;
using Shared.Grpc.Users;
using Users.Models;

namespace Users.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToTransfer(this User user) =>
            new UserDto
            {
                Id = $"{user.Id}",
                Name = user.Name,
                Email = user.Email,
                Identity = user.Identity
            };

        public static User ToModel(this UserDto user) =>
            new User
            {
                Id = Guid.Parse(user.Id),
                Name = user.Name,
                Email = user.Email,
                Identity = user.Identity
            };
    }
}