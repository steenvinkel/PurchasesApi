using Business.Repositories;
using Legacy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyPostingController(IPostingRepository postingRepository, ISubCategoryRepository subCategoryRepository) : ControllerBase
    {
        private readonly IPostingRepository _postingRepository = postingRepository;
        private readonly ISubCategoryRepository _subCategoryRepository = subCategoryRepository;

        [HttpGet]
        public ActionResult<List<LegacyPosting>> Get()
        {
            var userId = HttpContext.GetUserId();
            var existing = _postingRepository.Get200Descending(userId);

            var subcategoryIdMap = _subCategoryRepository.GetCategories(userId).SelectMany(c => c.Subcategories).ToDictionary(s => s.SubCategoryId, s => s.Name);

            return Ok(existing.Select(posting => LegacyPosting.Map(posting, subcategoryIdMap[posting.SubcategoryId])));
        }

        [HttpPut]
        public ActionResult<LegacyPosting> Put(LegacyPosting posting)
        {
            ValidatePosting(posting);

            var userId = HttpContext.GetUserId();

            var subCategoryId = _subCategoryRepository.GetSubCategoryId(userId, posting.Description);

            var newPosting = _postingRepository.Update(userId, LegacyPosting.Map(posting, userId, subCategoryId));

            return Ok(LegacyPosting.Map(newPosting, posting.Description));
        }

        [HttpPost]
        public ActionResult<LegacyPosting> Post(LegacyPosting posting)
        {
            if (posting.Posting_id != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(posting.Posting_id));
            }
            ValidatePosting(posting);

            var userId = HttpContext.GetUserId();

            var subCategoryId = _subCategoryRepository.GetSubCategoryId(userId, posting.Description);

            var newPosting = _postingRepository.Add(userId, LegacyPosting.Map(posting, userId, subCategoryId));

            return Ok(LegacyPosting.Map(newPosting, posting.Description));
        }

        private static void ValidatePosting(LegacyPosting posting)
        {
            if (posting.Amount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(posting.Amount));
            }
            if (string.IsNullOrEmpty(posting.Description))
            {
                throw new ArgumentOutOfRangeException(nameof(posting.Description));
            }
            if (posting.Date == new DateTime())
            {
                throw new ArgumentOutOfRangeException(nameof(posting.Date));
            }
            if ((posting.Latitude.HasValue
                || posting.Longitude.HasValue
                || posting.Accuracy.HasValue)
                && (!posting.Latitude.HasValue
                || !posting.Longitude.HasValue
                || !posting.Accuracy.HasValue
                    ))
            {
                throw new ArgumentException("Coordinates has to be all set or none set");
            }
        }
    }
}