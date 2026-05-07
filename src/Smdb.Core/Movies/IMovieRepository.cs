namespace Smdb.Core.Movies;

using Abcs.Http;

public interface IMovieRepository
{
	public Task<Movie?> Create(Movie newData);
	public Task<Movie?> Read(int id);
	public Task<Movie?> Update(int id, Movie updatedData);
	public Task<Movie?> Delete(int id);
	public Task<PagedResult<Movie>> List(int page, int size);
}
