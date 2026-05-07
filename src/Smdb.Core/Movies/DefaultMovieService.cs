namespace Smdb.Core.Movies;

using System;
using System.Net;
using Abcs.Http;

public class DefaultMovieService : IMovieService
{
	private readonly IMovieRepository movieRepository;

	public DefaultMovieService(IMovieRepository movieRepository)
	{
		this.movieRepository = movieRepository;
	}

	public async Task<Result<Movie>> Create(Movie newData)
	{
		var validationResult = ValidateMovie(newData);

		if (validationResult != null) { return validationResult; }

		var movie = await movieRepository.Create(newData);
		var result = movie == null
			? new Result<Movie>(new Exception($"Could not create movie {newData}."), (int)HttpStatusCode.NotFound)
			: new Result<Movie>(movie, (int)HttpStatusCode.Created);

		return result;
	}

	public async Task<Result<Movie>> Read(int id)
	{
		var movie = await movieRepository.Read(id);
		var result = movie == null
			? new Result<Movie>(new Exception($"Could not read movie with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<Movie>(movie, (int)HttpStatusCode.OK);

		return result;
	}

	public async Task<Result<Movie>> Update(int id, Movie updatedData)
	{
		var validationResult = ValidateMovie(updatedData);

		if (validationResult != null) { return validationResult; }

		var movie = await movieRepository.Update(id, updatedData);
		var result = movie == null
			? new Result<Movie>(new Exception($"Could not update movie {updatedData} with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<Movie>(movie, (int)HttpStatusCode.OK);

		return result;
	}

	public async Task<Result<Movie>> Delete(int id)
	{
		var movie = await movieRepository.Delete(id);
		var result = movie == null
			? new Result<Movie>(new Exception($"Could not delete movie with id {id}."), (int)HttpStatusCode.NotFound)
			: new Result<Movie>(movie, (int)HttpStatusCode.OK);

		return result;
	}

	public async Task<Result<PagedResult<Movie>>> List(int page, int size)
	{
		if (page <= 0) { new Result<PagedResult<Movie>>(new Exception("Page must be >= 1."), (int)HttpStatusCode.BadRequest); }
		if (size <= 0) { new Result<PagedResult<Movie>>(new Exception("Page size must be >= 1."), (int)HttpStatusCode.BadRequest); }

		var pagedResult = await movieRepository.List(page, size);
		var result = pagedResult == null
			? new Result<PagedResult<Movie>>(new Exception($"Could not read movies from page {page} and size {size}."), (int)HttpStatusCode.NotFound)
			: new Result<PagedResult<Movie>>(pagedResult, (int)HttpStatusCode.OK);

		return result;
	}

	private static Result<Movie>? ValidateMovie(Movie? movieData)
	{
		if (movieData is null)
		{
			return new Result<Movie>(new Exception("Movie payload is required."), (int)HttpStatusCode.BadRequest);
		}

		if (string.IsNullOrWhiteSpace(movieData.Title))
		{
			return new Result<Movie>(new Exception("Title is required and cannot be empty."), (int)HttpStatusCode.BadRequest);
		}

		if (movieData.Title.Length > 256)
		{
			return new Result<Movie>(new Exception("Title cannot be longer than 256 characters."), (int)HttpStatusCode.BadRequest);
		}

		if (movieData.Year < 1888 || movieData.Year > DateTime.UtcNow.Year)
		{
			return new Result<Movie>(new Exception($"Year must be between 1888 and {DateTime.UtcNow.Year}."), (int)HttpStatusCode.BadRequest);
		}

		return null;
	}
}

