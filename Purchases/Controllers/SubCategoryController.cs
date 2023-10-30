using System.Collections.Generic;
using Business.Models;
using Business.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public SubCategoryController(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        [HttpGet]
        public ActionResult<List<SubCategory>> Get()
        {
            var userId = HttpContext.GetUserId();

            var subcategories = _subCategoryRepository.GetList(userId);

            return Ok(subcategories);
        }
    }
}