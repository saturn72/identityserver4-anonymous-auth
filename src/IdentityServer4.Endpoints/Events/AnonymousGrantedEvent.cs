using IdentityServer4.Anonymous.Services;
using IdentityServer4.Events;

namespace IdentityServer4.Anonymous.Events
{
    public class AnonymousGrantedEvent : Event
    {
        public AnonymousCodeInfo Code { get; set; }

        public AnonymousGrantedEvent(AnonymousCodeInfo code, string message = null) :
            base(EventCategories.Grants,
                Constants.Events.GrantSuccessEventName,
                EventTypes.Success,
                Constants.Events.GrantSuccessEventId,
                message)
        {
            Code = code;
        }
    }
}
