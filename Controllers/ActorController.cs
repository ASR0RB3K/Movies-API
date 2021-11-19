using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using movies.Mappers;
using movies.Models;
using movies.Services;

namespace movies.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _as;

        public ActorController(IActorService actorService)
        {
            _as = actorService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(NewActor actor)
        {
            var result = await _as.CreateAsync(actor.ToEntity());

            if(result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.Exception.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
            => Ok(await _as.GetAllAsync());

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetActorIdAsync([FromRoute] Guid id)
        {
            var actor = await _as.GetActorIdAsync(id);

            if (actor.IsSuccess)
            {
                return Ok(actor.actorResult);
            }

            return NotFound();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] NewActor newActor)
        {
            var pizzaEntities = newActor.ToEntity(); pizzaEntities.Id = id;

            var updateResult = await _as.UpdatedActorAsync(id, pizzaEntities);

            if (updateResult.IsSuccess)
            {
                return Ok(updateResult.actor);
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid id)
        {
            var deleteResult = await _as.DeleteActorAsync(id);

            if(deleteResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}