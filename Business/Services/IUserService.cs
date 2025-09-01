using Business.Models;

namespace Business.Services
{
    public interface IUserService
    {
        User Get(int userId);
    }
}
