using Business.Models;
using System.Collections.Generic;

namespace Business.Services
{
    public interface IAccountService
    {
        List<Account> Get(int userId);
    }
}