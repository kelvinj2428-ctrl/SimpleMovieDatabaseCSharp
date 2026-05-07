namespace Smdb.Core.Movies;

using Abcs.Http;
using Smdb.Core.Shared;

public class MemoryMovieRepository : IMovieRepository
{
	private MemoryDatabase db;

	public MemoryMovieRepository(MemoryDatabase db)
	{
		this.db = db;
	}

	public async Task<Movie?> Create(Movie newData)
	{
		newData.Id = db.NextMovieId();
		db.Movies.Add(newData);

		return await Task.FromResult(newData);
	}

	public async Task<Movie?> Read(int id)
	{
		Movie? result = db.Movies.FirstOrDefault(m => m.Id == id);

		return await Task.FromResult(result);
	}

	public async Task<Movie?> Update(int id, Movie updatedData)
	{
		Movie? result = db.Movies.FirstOrDefault(m => m.Id == id);

		if (result != null)
		{
			result.Title = updatedData.Title;
			result.Year = updatedData.Year;
			result.Description = updatedData.Description;
		}

		return await Task.FromResult(result);
	}

	public async Task<Movie?> Delete(int id)
	{
		Movie? result = db.Movies.FirstOrDefault(m => m.Id == id);

		if (result != null)
		{
			db.Movies.Remove(result);
		}

		return await Task.FromResult(result);
	}

	public async Task<PagedResult<Movie>> List(int page, int size)
	{
		int totalCount = db.Movies.Count;
		int start = Math.Clamp((page - 1) * size, 0, totalCount);
		int length = Math.Clamp(size, 0, totalCount - start);
		var values = db.Movies.Slice(start, length);
		var result = new PagedResult<Movie>(totalCount, values);

		return await Task.FromResult(result);
	}
}
