using Domain.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DomainModels
{
    public class Genre : BaseEntity
    {
        [Required]
        public string GenreName { get; set; }

        public virtual ICollection<TicketsTypeGenres> TicketsTypeGenres { get; set; }
    }
}
