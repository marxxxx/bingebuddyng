using System;

namespace BingeBuddyNg.Functions.Services.Notifications
{
    public class NotificationMessage
    {
        public NotificationMessage(string title, string body)
        {
            this.Title = title ?? throw new ArgumentNullException(nameof(title));
            this.Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public string Title { get; }

        public string Body { get; }

        public override string ToString()
        {
            return $"{{{nameof(Title)}={Title}, {nameof(Body)}={Body}}}";
        }
    }
}
