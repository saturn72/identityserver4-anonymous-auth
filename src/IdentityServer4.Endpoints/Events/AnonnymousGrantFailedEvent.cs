using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Events;

namespace IdentityServer4.Anonnymous.Events
{
    public class AnonnymousGrantFailedEvent : Event
    {
        public AnonnymousCodeInfo Code { get; set; }

        public AnonnymousGrantFailedEvent(AnonnymousCodeInfo code, string message = null) :
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
