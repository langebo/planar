using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Users;
using Users.Data;
using Users.Extensions;
using Users.Models;

namespace Users
{
    public class UsersEndpoint : UsersService.UsersServiceBase
    {
        private readonly UsersContext db;

        public UsersEndpoint(UsersContext db)
        {
            this.db = db;
        }

        public override async Task<UserDto> GetUser(GetUserQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var user = await db.Users.AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == id, context.CancellationToken);

            return user.ToTransfer();
        }

        public override async Task GetUsers(GetUsersQueryDto request, IServerStreamWriter<UserDto> responseStream, ServerCallContext context)
        {
            var users = db.Users.AsNoTracking();
            if (request.Ids.Any())
                users = users.Where(u => request.Ids.Select(i => Guid.Parse(i)).Contains(u.Id));

            await foreach (var user in users.AsAsyncEnumerable())
                await responseStream.WriteAsync(user.ToTransfer());
        }

        public override async Task<UserDto> CreateUser(CreateUserCommandDto request, ServerCallContext context)
        {
            var result = await db.AddAsync(new User
            {
                Name = request.Name,
                Email = request.Email,
                Identity = request.Identity
            }, context.CancellationToken);

            await db.SaveChangesAsync(context.CancellationToken);
            return result.Entity.ToTransfer();
        }

        public override async Task<UserDto> UpdateUser(UpdateUserCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == id, context.CancellationToken);

            user.Name = request.Name;
            user.Email = request.Email;
            user.Identity = request.Identity;

            var result = db.Users.Attach(user);
            await db.SaveChangesAsync(context.CancellationToken);

            return result.Entity.ToTransfer();
        }

        public override async Task<Empty> DeleteUser(DeleteUserCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == id, context.CancellationToken);

            db.Users.Remove(user);
            await db.SaveChangesAsync(context.CancellationToken);

            return new Empty();
        }

        public override async Task<UserExistsResponseDto> UserExists(UserExistsQueryDto request, ServerCallContext context)
        {
            Expression<Func<User, bool>> filter = u => true;
            switch (request.IdentifierCase)
            {
                case UserExistsQueryDto.IdentifierOneofCase.Id:
                    var id = Guid.Parse(request.Id);
                    filter = u => u.Id == id;
                    break;
                case UserExistsQueryDto.IdentifierOneofCase.Identity:
                    filter = u => u.Identity == request.Identity;
                    break;
                case UserExistsQueryDto.IdentifierOneofCase.None:
                    throw new ArgumentOutOfRangeException("Unrecognized user identifier type (none)");
            }

            var exists = await db.Users.AsNoTracking()
                .AnyAsync(filter, context.CancellationToken);

            return new UserExistsResponseDto { Exists = exists };
        }
    }
}
