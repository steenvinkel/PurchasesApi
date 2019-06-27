using Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Repositories
{
    public interface IAccountRepository
    {
        List<Account> Get(int userId);
    }
}
