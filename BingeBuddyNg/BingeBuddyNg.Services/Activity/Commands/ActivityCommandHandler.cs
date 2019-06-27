using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class ActivityCommandHandler :   
            IRequestHandler<AddDrinkActivityCommand>,
            IRequestHandler<AddImageActivityCommand>,
            IRequestHandler<AddMessageActivityCommand>,
            IRequestHandler<AddVenueActivityCommand>,
            IRequestHandler<AddReactionCommand>
    {
        public Task<Unit> Handle(AddImageActivityCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Unit> Handle(AddMessageActivityCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Unit> Handle(AddVenueActivityCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Unit> Handle(AddReactionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<Unit> IRequestHandler<AddDrinkActivityCommand, Unit>.Handle(AddDrinkActivityCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
