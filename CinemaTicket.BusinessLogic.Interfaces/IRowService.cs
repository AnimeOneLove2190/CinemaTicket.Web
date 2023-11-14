﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Rows;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IRowService
    {
        Task CreateAsync(RowCreate rowCreate);
        Task UpdateAsync(RowUpdate rowUpdate);
        Task<RowDetails> GetAsync(int id);
        Task<List<RowListElement>> GetListAsync();
        Task DeleteAsync(int id);
    }
}
