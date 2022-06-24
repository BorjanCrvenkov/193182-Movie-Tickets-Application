using Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interface
{
    public interface IGenreService
    {
        public List<Genre> getAllGenres();
        public Genre getGenreDetails(Guid? genreId);
    }
}
