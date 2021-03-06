﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User;
using MediatR;

namespace BingeBuddyNg.Core.Activity.Commands
{
    public class AddImageActivityCommand : IRequest<string>
    {
        public AddImageActivityCommand(string userId, Stream stream, string fileName, double? lat, double? lng)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));

            if (lat.GetValueOrDefault() + lng.GetValueOrDefault() > 0)
            {
                Location = new Location(lat.Value, lng.Value);
            }
        }

        public string UserId { get; }
        public Stream Stream { get; }
        public string FileName { get; }
        public Location Location { get; }

        public override string ToString()
        {
            return $"{{{nameof(Stream)}={Stream}, {nameof(FileName)}={FileName}, {nameof(Location)}={Location}}}";
        }
    }

    public class AddImageActivityCommandHandler :
           IRequestHandler<AddImageActivityCommand, string>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IStorageAccessService storageAccessService;

        public AddImageActivityCommandHandler(
            IUserRepository userRepository,
            IActivityRepository activityRepository,
            IStorageAccessService storageAccessService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<string> Handle(AddImageActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetUserAsync(request.UserId);

            // store file in blob storage
            string imageUrlOriginal = await storageAccessService.SaveFileInBlobStorage("img", "activities", request.FileName, request.Stream);

            var activity = Domain.Activity.CreateImageActivity(request.Location, request.UserId, user.Name, imageUrlOriginal);

            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());

            await activityRepository.AddToActivityAddedTopicAsync(activity.Id);

            return savedActivity.Id;
        }
    }
}