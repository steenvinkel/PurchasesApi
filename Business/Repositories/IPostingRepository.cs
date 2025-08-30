using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IPostingRepository
    {
        IEnumerable<Posting> Get(int userId);
        Posting Add(int userId, Posting posting);
        Posting Update(int userId, Posting posting);
    }
}
