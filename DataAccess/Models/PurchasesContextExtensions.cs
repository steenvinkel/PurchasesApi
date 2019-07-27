using System.Linq;

namespace DataAccess.Models
{
    public partial class PurchasesContext
    {
        public IQueryable<Posting> PostingForUser(int userId)
        {
            return Posting.Where(posting => posting.UserId == userId);
        }

        public IQueryable<Category> CategoryForUser(int userId)
        {
            return Category.Where(category => category.UserId == userId);
        }
    }
}
