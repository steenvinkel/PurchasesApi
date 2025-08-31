using Business.Models;
using Business.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;
using System.Collections.Generic;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostingController(IPostingRepository postingRepository) : ControllerBase
    {
        private readonly IPostingRepository _postingRepository = postingRepository;

        [HttpGet]
        public ActionResult<IEnumerable<Posting>> Get()
        {
            var userId = HttpContext.GetUserId();

            var postings = _postingRepository.Get(userId);

            return Ok(postings);
        }

        [HttpPost]
        public ActionResult<Posting> Post(Posting posting)
        {
            var userId = HttpContext.GetUserId();

            var createdPosting = _postingRepository.Add(userId, posting);

            return Ok(createdPosting);
        }

        [HttpPut]
        public ActionResult<Posting> Put(Posting posting)
        {
            var userId = HttpContext.GetUserId();

            var updatedPosting = _postingRepository.Update(userId, posting);

            return Ok(updatedPosting);
        }

        [HttpDelete("{postingId}")]
        public ActionResult Delete(int postingId)
        {
            var userId = HttpContext.GetUserId();

            _postingRepository.Delete(userId, postingId);

            return NoContent();
        }
    }
}
