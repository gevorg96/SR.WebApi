using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IBusinessRepository
    {
        IEnumerable<Business> GetWithFilter(string field, string value);
        Business GetById(int businessId);
    }
}
