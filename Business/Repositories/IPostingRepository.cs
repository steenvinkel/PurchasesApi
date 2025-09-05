using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IPostingRepository
    {
        IEnumerable<Posting> Get(int userId);
        IEnumerable<Posting> GetAllForSubcategory(int userId, int subcategoryId);
        Posting Add(int userId, Posting posting);
        Posting Update(int userId, Posting posting);
        void Delete(int userId, int postingId);
    }
}
