using Business.Models;
using System.Collections.Generic;

namespace Business.Services
{
    public interface IAccountService
    {
        List<AccountDAO> Get(int userId);
    }
}