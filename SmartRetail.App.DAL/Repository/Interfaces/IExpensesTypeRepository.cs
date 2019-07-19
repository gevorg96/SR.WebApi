﻿using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IExpensesTypeRepository
    {
        IEnumerable<ExpensesType> GetAll();
    }
}
