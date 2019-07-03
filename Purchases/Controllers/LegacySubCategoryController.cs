using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacySubCategoryController : ControllerBase
    {
        private readonly PurchasesContext _context;

        public LegacySubCategoryController(PurchasesContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<string>> Get()
        {
            var userId = HttpContext.GetUserId();
            var subcategoryNames = from subcategory in _context.Subcategory
                                   join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                                   where category.UserId == userId
                                   select subcategory.Name;

            return Ok(subcategoryNames);
        }
    }
}