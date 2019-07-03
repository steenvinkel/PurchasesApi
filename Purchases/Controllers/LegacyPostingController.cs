using Legacy.LegacyFormatting;
using Legacy.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

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
    }
}