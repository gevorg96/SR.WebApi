using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public interface IBillDataService
    {
        Task<int> Insert(BillParent bill);
    }
}
