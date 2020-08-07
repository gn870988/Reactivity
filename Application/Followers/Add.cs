using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Followers
{
    public class Add
    {
        public class Command : IRequest
        {
            public string Username { get; set; }

        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {

                var observer = await _context.Users
                    .SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(), cancellationToken);

                var target = await _context.Users
                    .SingleOrDefaultAsync(u => u.UserName == request.Username, cancellationToken);

                if (target == null)
                    throw new RestException(HttpStatusCode.NotFound, new { User = "not found" });

                var following = await _context.Followings
                    .SingleOrDefaultAsync(u =>
                        u.ObserverId == observer.Id && u.TargetId == target.Id, cancellationToken);

                if (following != null)
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "You are already following this user" });

                following = new UserFollowing()
                {
                    Observer = observer,
                    Target = target
                };

                _context.Followings.Add(following);

                bool isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (isSuccess) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}