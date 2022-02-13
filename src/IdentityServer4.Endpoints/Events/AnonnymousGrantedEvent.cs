using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Events;

namespace IdentityServer4.Anonnymous.Events
{
    public class AnonnymousGrantedEvent : Event
    {
        public AnonnymousCodeInfo Code { get; set; }

        public AnonnymousGrantedEvent(AnonnymousCodeInfo code, string message = null) :
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
