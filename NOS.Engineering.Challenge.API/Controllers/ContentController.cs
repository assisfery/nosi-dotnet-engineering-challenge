using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    private readonly ILogger<ContentController> _logger;
    private readonly IMemoryCache _cache;
    public ContentController(IContentsManager manager, ILogger<ContentController> logger, IMemoryCache cache)
    {
        _manager = manager;
        _logger = logger;
        _cache = cache;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetManyContents(bool testMode = false)
    {
        _logger.LogInformation("Listing Contents");

        if(!_cache.TryGetValue("GetManyContents", out IEnumerable<Content?>? contents))
        {
            contents = await _manager.GetManyContents().ConfigureAwait(false);

            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
            
            if(!testMode)
            {
                _cache.Set("GetManyContents", contents, cacheOptions);
            }
        }        

        if (contents == null || !contents.Any())
            return NotFound();
        
        return Ok(contents);
    }

    [HttpGet]
    [Route("~/api/v2/[controller]")]    
    public async Task<IActionResult> SearchContents([FromQuery] String? title, [FromQuery] List<string>? genres)
    {
        _logger.LogInformation("Listing Contents");

        //string cacheKey = "SearchContents:" + title + ":" + genres;
        //if (!_cache.TryGetValue(cacheKey, out IEnumerable<Content?>? contents))
        //{
        //    contents = await _manager.SearchContents(title, genres).ConfigureAwait(false);

        //    var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
        //    _cache.Set(cacheKey, contents, cacheOptions);
        //}

        var contents = await _manager.SearchContents(title, genres).ConfigureAwait(false);

        if (contents == null || !contents.Any())
            return NotFound();

        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        _logger.LogInformation("Get Content for " + id.ToString());

        string cacheKey = "GetContent:" + id.ToString();
        if (!_cache.TryGetValue(cacheKey, out Content? content))
        {
            content = await _manager.GetContent(id).ConfigureAwait(false);

            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
            _cache.Set(cacheKey, content, cacheOptions);
        }

        if (content == null)
            return NotFound();
        
        return Ok(content);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation("Creating Content");

        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        return createdContent == null ? Problem() : Ok(createdContent);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation("Updating Content");

        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        _logger.LogInformation("Deleting Content");

        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        return Ok(deletedId);
    }
    
    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        _logger.LogInformation("Add Genres to Content");

        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
            return NotFound();
        
        List<string> listGenre = content.GenreList.ToList();   
        foreach (string g in  genre)
        {
            if(!listGenre.Contains(g))
            {
                listGenre.Add(g);
            }
        }

        ContentInput input = new ContentInput();
        input.GenreList = listGenre;
        var updatedContent = await _manager.UpdateContent(id, input.ToDto()).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
    
    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        _logger.LogInformation("Delete Genres from Content");

        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
            return NotFound();

        List<string> listGenre = content.GenreList.ToList();
        foreach (string g in genre)
        {
            if (listGenre.Contains(g))
            {
                listGenre.Remove(g);
            }
        }

        ContentInput input = new ContentInput();
        input.GenreList = listGenre;
        var updatedContent = await _manager.UpdateContent(id, input.ToDto()).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
}