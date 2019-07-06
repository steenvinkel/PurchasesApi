using Legacy.LegacyFormatting;
using Legacy.Models;
using Legacy.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;
using System;

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
        public ActionResult Get()
        {
            var postings = _postingRepository.Get(HttpContext.GetUserId());
            return postings.AddLegacyFormatting();
        }

        [HttpPut]
        public ActionResult Put(LegacyPosting posting)
        {
            ValidatePosting(posting);

            var userId = HttpContext.GetUserId();
            var newPosting = _postingRepository.Put(posting, userId);

            return Ok(newPosting).AddLegacyFormatting();
        }

        [HttpPost]
        public ActionResult Post(LegacyPosting posting)
        {
            if (posting.Posting_id != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(posting.Posting_id));
            }
            ValidatePosting(posting);

            var userId = HttpContext.GetUserId();
            var newPosting = _postingRepository.Post(posting, userId);

            return Ok(newPosting).AddLegacyFormatting();
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
            if (posting.Date == default(DateTime))
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