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
            var result = await _context.Post.Select(x => new 
            {
                Id = x.Id,
                // = x.Image,
                Text = x.Text,
                Image = x.Image != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(x.Image)}" : null
            }).ToListAsync();
            return Ok(result);
        }

        [HttpGet("GetPost/{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _context.Post
                .Where(x => x.Id == id)
                .Select(x => new Post
                {
                    Id = x.Id,
                    Image = x.Image,
                    Text = x.Text,
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound($"Post with Id {id} not found.");
            }

            return Ok(post);
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] string text, [FromForm] IFormFile? image)
        {
            // Create the post object
            var post = new Post
            {
                Text = text
            };

            if (image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copy the image to a memory stream
                    await image.CopyToAsync(memoryStream);
                    post.Image = memoryStream.ToArray(); // Store the byte array in the post object
                }
            }

            // Save the post to the database
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
