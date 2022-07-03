using Domain.DomainModels;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Implementation
{
    public class GenreService : IGenreService
    {

        private readonly IRepository<Genre> _repository;

        public GenreService(IRepository<Genre> repository)
        {
            _repository = repository;
        }

        public void createNewGenre(Genre g)
        {
            this._repository.Insert(g);
        }

        public List<Genre> getAllGenres()
        {
            return this._repository.GetAll().OrderBy(z => z.Name).ToList();
        }

        public Genre getGenreDetails(Guid? genreId)
        {
            return this._repository.Get(genreId);
        }
    }
}
