using Legacy.LegacyFormatting;
using Legacy.Models;
using Legacy.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;
using System;
using System.Collections.Generic;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyPostingController : ControllerBase
    {
        private readonly ILegacyPostingRepository _postingRepository;

        public LegacyPostingController(ILegacyPostingRepository postingRepository)
        {
            _postingRepository = postingRepository;
        }

        [HttpGet]
        public ActionResult<List<LegacyPosting>> Get()
        {
            var postings = _postingRepository.Get(HttpContext.GetUserId());
            return Ok(postings);
        }

        [HttpPut]
        public ActionResult<LegacyPosting> Put(LegacyPosting posting)
        {
            ValidatePosting(posting);

            var userId = HttpContext.GetUserId();
            var newPosting = _postingRepository.Put(posting, userId);

            return Ok(newPosting);
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
            var newPosting = _postingRepository.Post(posting, userId);

            return Ok(newPosting);
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