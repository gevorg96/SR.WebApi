using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IUnitRepository
    {
        IEnumerable<Units> GetAllUnits();
    }
}