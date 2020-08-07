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

namespace Application.Activities
{
    public class Attend
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
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
                var activity = await _context.Activities.FindAsync(request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound,
                        new { Activity = "Could not find activity" });

                var user = await _context.Users
                    .SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetCurrentUsername(),
                        cancellationToken);

                var attendance = await _context.UserActivities
                    .SingleOrDefaultAsync(ua => ua.ActivityId == activity.Id && ua.AppUserId == user.Id,
                        cancellationToken);

                if (attendance != null)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new { Attendance = "Already attending this activity" });

                attendance = new UserActivity
                {
                    Activity = activity,
                    AppUser = user,
                    IsHost = false,
                    DateJoined = DateTime.Now
                };

                _context.UserActivities.Add(attendance);

                bool isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (isSuccess) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}