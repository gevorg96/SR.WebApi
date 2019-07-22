using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.Web.Models.Validation
{
    public interface IChecker
    {
        AvailabilityModel CheckAvailability(UserProfile user, int? shopId);
    }
}
