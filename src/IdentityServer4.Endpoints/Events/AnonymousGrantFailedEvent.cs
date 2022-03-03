using IdentityServer4.Anonymous.Services;
using IdentityServer4.Events;

namespace IdentityServer4.Anonymous.Events
{
    public class AnonymousGrantFailedEvent : Event
    {
        public AnonymousCodeInfo Code { get; set; }

        public AnonymousGrantFailedEvent(AnonymousCodeInfo code, string message = null) :
            base(EventCategories.Grants,
                Constants.Events.GrantFailedEventName,
                EventTypes.Failure,
                Constants.Events.GrantFailedEventId,
                message)
        {
            Code = code;
        }
    }
}
