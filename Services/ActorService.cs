using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using movies.Data;
using movies.Entities;

namespace movies.Services
{
public class ActorService : IActorService
{
        private readonly MoviesContext _ctx;
        private readonly ILogger<ActorService> _logger;

        public ActorService(MoviesContext context, ILogger<ActorService> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, Exception Exception, Actor Actor)> CreateAsync(Actor actor)
        {
            try
            {
                await _ctx.Actors.AddAsync(actor);
                await _ctx.SaveChangesAsync();

                return (true, null, actor);
            }
            catch(Exception e)
            {
                return (false, e, null);
            }
        }

        public async Task<(bool IsSuccess, Exception Exception)> DeleteAsync(Guid id)
        {
            try
            {
                var actor = await GetAsync(id);

                if(actor == default(Actor))
                {
                    return (false, new Exception("Not found"));
                }

                _ctx.Actors.Remove(actor);
                await _ctx.SaveChangesAsync();

                return (true,  null);
            }
            catch(Exception e)
            {
                return (false, e);
            }
        }

        public async Task<(bool IsSuccess, Exception exception, Actor actorResult)> GetActorIdAsync(Guid Id)
        {
            try
            {
                var pizzaResult = await _ctx.Actors.AsNoTracking().FirstOrDefaultAsync(p => p.Id == Id);

                if (pizzaResult is default(Actor))
                {
                    return (false, null, null);
                }

                _logger.LogInformation($"Actor recived from database: {Id}");
                return (true, null, pizzaResult);
            }

            catch (Exception e)
            {
                _logger.LogInformation($"Receiving actor from database: {Id} failed");
                return (false, e, null);
            }
        }

        public async Task<(bool IsSuccess, Exception exception, Actor actor)> UpdatedActorAsync(Guid id, Actor actor)
        {
            try
            {
                if (await _ctx.Actors.AnyAsync(p => p.Id == id))
                {
                    _ctx.Actors.Update(actor);
                    await _ctx.SaveChangesAsync();

                    _logger.LogInformation($"Actor updated in database: {id}.");
                    return (true, null, actor);
                }
                else
                {
                    return (false, new Exception(), null);
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Updating with given ID: {id} not found\nError: {e.Message}");
                return (false, e, null);
            }
        }

        public Task<bool> ExistsAsync(Guid id)
            => _ctx.Actors.AnyAsync(a => a.Id == id);

        public Task<List<Actor>> GetAllAsync()
            => _ctx.Actors.ToListAsync();

        public Task<List<Actor>> GetAllAsync(string fullname)
            => _ctx.Actors
                .AsNoTracking()
                .Where(a => a.Fullname == fullname)
                .ToListAsync();

        public Task<Actor> GetAsync(Guid id)
            => _ctx.Actors.FirstOrDefaultAsync(a => a.Id == id);
    }
}