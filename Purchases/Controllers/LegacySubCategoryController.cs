using System.Collections.Generic;
using System.Linq;
using Business.Repositories;
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
        private readonly ISubCategoryRepository _subCategoryRepository;

        public LegacySubCategoryController(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        [HttpGet]
        public ActionResult<List<string>> Get()
        {
            var userId = HttpContext.GetUserId();

            var subcategories = _subCategoryRepository.GetList(userId);

            var subcategoryNames = subcategories.Select(s => s.Name).ToList();

            return Ok(subcategoryNames);
        }
    }
}