using Business.Models;

namespace Business.Repositories
{
    public interface IUserRepository
    {
        User Get(string authToken);
    }
}
