﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CinemaTicket.Entities;
using CinemaTicket.DataTransferObjects.Movies;
using CinemaTicket.Infrastructure.Helpers;

namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IMovieService
    {
        Task CreateAsync(MovieCreate movieCreate);
        Task UpdateAsync(MovieUpdate movieUpdate);
        Task<MovieDetails> GetAsync(int id);
        Task<List<MovieListElement>> GetListAsync();
        Task<Page<MoviePageView>> GetPageAsync(MovieSearchRequest movieSearch);
    }
}
