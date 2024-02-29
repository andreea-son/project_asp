using Project.Backend.Common.Models;

namespace Project.Backend.Core
{
    public interface IUsersService
    {
        void AddUser(RegisterDto data);
        AuthTokenDto Login(LoginDto data);
        string[] GetUsers();
    }
}