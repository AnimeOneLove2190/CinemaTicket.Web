using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Genres;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IMovieService
    {
        Task CreateAsync(string movieName, string movieDescription, int movieDuration, List<string> genreNames);
    }
}
