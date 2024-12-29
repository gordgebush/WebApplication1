using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class PostController : ControllerBase
    {
     

        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetPost")]
        public async Task<IActionResult> GetPost()
        {
            var result = await _context.Post.Select(x => new Post
            {
                Id = x.Id,
                Image = x.Image,
                Text = x.Text,
            }).ToListAsync();
            return Ok(result);
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromBody] Post post)
        {
            _context.Post.Add(post);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("EditPost")]
        public async Task<IActionResult> EditPost([FromBody] Post post)
        {
            var rows = await _context.Post.Where(x => x.Id == post.Id).ExecuteUpdateAsync(x => x.SetProperty(x => x.Text, post.Text));

            return Ok(post);
        }

        [HttpDelete("DeletePost")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var rows = await _context.Post.Where(x => x.Id == postId).ExecuteDeleteAsync();

            return Ok(true);
        }

    }
}
