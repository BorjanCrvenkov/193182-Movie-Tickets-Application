using Domain.Identity;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<TicketingUser> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public TicketingUser GetDetailsForUser(String id)
        {
            return this._userRepository.Get(id);
        }

        public void Update(TicketingUser entity)
        {
            this._userRepository.Update(entity);
        }
    }
}
