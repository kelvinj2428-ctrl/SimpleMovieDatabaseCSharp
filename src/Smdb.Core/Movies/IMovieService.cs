namespace Smdb.Core.Movies;

using Abcs.Http;

public interface IMovieService
{
	public Task<Result<Movie>> Create(Movie newData);
	public Task<Result<Movie>> Read(int id);
	public Task<Result<Movie>> Update(int id, Movie updatedData);
	public Task<Result<Movie>> Delete(int id);
	public Task<Result<PagedResult<Movie>>> List(int page, int size);
}
