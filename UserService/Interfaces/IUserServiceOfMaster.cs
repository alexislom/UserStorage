using System;
using UserService.Entities;
using UserService.Events;

namespace UserService.Interfaces
{
    public interface IUserServiceOfMaster : IUserService
    {
        void Add(User user);
        void Delete(Func<User, bool> deletePredicate);

        event EventHandler<UserEventArgs> AddUser;
        event EventHandler<UserEventArgs> DeleteUser;
        event EventHandler<UserEventArgs> AddUserOnSlaveCreating;
    }
}
