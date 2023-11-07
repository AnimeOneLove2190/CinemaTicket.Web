using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Genres;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IGenreService
    {
        Task CreateAsync(GenreCreate genreCreate);
        Task UpdateAsync(GenreUpdate genreUpdate);
    }
}
