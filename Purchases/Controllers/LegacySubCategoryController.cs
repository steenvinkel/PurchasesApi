using System.Collections.Generic;
using System.Linq;
using Business.Repositories;
using DataAccess.Models;
using Legacy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacySubCategoryController : ControllerBase
    {
        private readonly ILegacyPostingQueryService _postingQueryService;

        public LegacySubCategoryController(ILegacyPostingQueryService postingQueryService)
        {
            _postingQueryService = postingQueryService;
        }

        [HttpGet]
        public ActionResult<List<string>> Get()
        {
            var userId = HttpContext.GetUserId();

            var names = _postingQueryService.GetAllSubCategoryNames(userId);

            return Ok(names);
        }
    }
}