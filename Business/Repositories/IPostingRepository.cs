using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IPostingRepository
    {
        IEnumerable<Posting> Get200Descending(int userId);
        Posting Add(int userId, Posting posting);
        Posting Update(int userId, Posting posting);
        IEnumerable<Posting> GetAllForSubcategory(int userId, int subcategoryId);
    }
}
