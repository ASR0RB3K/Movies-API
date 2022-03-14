using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using movies.Data;
using movies.Entities;
using static System.Net.Mime.MediaTypeNames;

namespace movies.Services
{
    public class MovieService : IMovieService
    {
        private readonly MoviesContext _ctx;
        private readonly ILogger<GenreService> _logger;

        public MovieService(MoviesContext context, ILogger<GenreService> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, Exception Exception, Movie Movie)> CreateAsync(Movie movie)
        {
            try
            {
                await _ctx.Movies.AddAsync(movie);
                await _ctx.SaveChangesAsync();

                return (true, null, movie);
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
                var movie = await GetAsync(id);

                if(movie == default(Movie))
                {
                    return (false, new Exception("Not found"));
                }

                _ctx.Movies.Remove(movie);
                await _ctx.SaveChangesAsync();

                return (true,  null);
            }
            catch(Exception e)
            {
                return (false, e);
            }
        }

        public async Task<(bool IsSuccess, Exception exception, Movie movie)> UpdatedMovieAsync(Guid id, Movie movie)
        {
            try
            {
                if (await _ctx.Movies.AnyAsync(p => p.Id == id))
                {
                    _ctx.Entry(movie).State = EntityState.Modified;
            _ctx.SaveChanges();
            _ctx.Movies.Update(movie);
            await _ctx.SaveChangesAsync();

                    _logger.LogInformation($"Movie updated in database: {id}.");
                    return (true, null, movie);
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

        public async Task<(bool IsSuccess, Exception exception)> DeleteMovieAsync(Guid id)
        {
            try
            {
                if (await _ctx.Movies.AnyAsync(p => p.Id == id))
                {
                    _ctx.Movies.Remove(_ctx.Movies.FirstOrDefault(p => p.Id == id));
                    await _ctx.SaveChangesAsync();

                    _logger.LogInformation($"Genre removed from database: {id}");

                    return (true, null);
                }

                 else
                {
                    return (false, null);
                }
            }

            catch (Exception e)
            {
                _logger.LogInformation($"Deleting genre from database: {id} failed\n{e.Message}", e);
                return (false, e);
            }
        }

        public Task<bool> ExistsAsync(Guid id)
            => _ctx.Movies.AnyAsync(a => a.Id == id);

        public Task<List<Movie>> GetAllAsync()
            => _ctx.Movies
                .AsNoTracking()
                .Include(m => m.Actors)
                .Include(m => m.Genres)
                .ToListAsync();

        public Task<List<Movie>> GetAllAsync(string title)
            => _ctx.Movies
                .AsNoTracking()
                .Where(a => a.Title == title)
                .Include(m => m.Actors)
                .Include(m => m.Genres)
                .ToListAsync();

        public Task<Movie> GetAsync(Guid id)
            => _ctx.Movies.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<(bool IsSuccess, Exception Exception)> CreateImagesAsync(List<Entities.Image> images)
        {
            try
            {
                await _ctx.Images.AddRangeAsync(images);
                await _ctx.SaveChangesAsync();

                return (true, null);
            }
            catch(Exception e)
            {
                return (false, e);
            }
        }

        public Task<Entities.Image> GetImageAsync(Guid id)
        => _ctx.Images.FirstOrDefaultAsync(i => i.Id == id);

        public Task<List<Entities.Image>> GetImagesAsync(Guid id)
        => _ctx.Images
            .AsNoTracking()
            .Where(i => i.MovieId == id)
            .ToListAsync();

        public Task<bool> ImageExistsAsync(Guid id)
        => _ctx.Images.AnyAsync(i => i.Id == id);
   
    }
}