using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using movies.Entities;
using movies.Mappers;
using movies.Models;
using movies.Services;

namespace movies.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _ms;
        private readonly IActorService _as;
        private readonly IGenreService _gs;

        public MovieController(
            IMovieService movieService,
            IGenreService genreService,
            IActorService actorService)
        {
            _ms = movieService;
            _as = actorService;
            _gs = genreService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(NewMovie movie)
        {
            if(movie.ActorIds.Count() < 1 || movie.GenreIds.Count() < 1)
            {
                return BadRequest("Actors and Genres are required");
            }

            if(!movie.GenreIds.All(id => _gs.ExistsAsync(id).Result))
            {
                return BadRequest("Genre doesnt exist");
            }

            if(!movie.ActorIds.All(id => _as.ExistsAsync(id).Result))
            {
                return BadRequest("Actor doesnt exist");
            }

            var genres = movie.GenreIds.Select(id => _gs.GetAsync(id).Result);
            var actors = movie.ActorIds.Select(id => _as.GetAsync(id).Result);
            
            var result = await _ms.CreateAsync(movie.ToEntity(actors, genres));

            if(result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.Exception.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(await _ms.GetAllAsync(), options);
            return Ok(json);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            if(await _ms.ExistsAsync(id))
            {
                return Ok(await _ms.GetAsync(id));
            }

            return NotFound();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateMovie([FromRoute]Guid id,[FromBody] Movie movie)
        {
            movie.Id = id;
            var updatedResult = await _ms.UpdatedMovieAsync(id, movie);

            if (updatedResult.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(updatedResult.exception.Message);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid id)
        {
            var deleteResult = await _ms.DeleteMovieAsync(id);

            if(deleteResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}