using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Movies;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IMovieService
    {
        Task CreateAsync(MovieCreate movieCreate);
    }
}
