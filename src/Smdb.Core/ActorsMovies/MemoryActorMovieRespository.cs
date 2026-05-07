namespace Smdb.Core.ActorsMovies;

using Abcs.Http;
using Smdb.Core.Actors;
using Smdb.Core.Movies;
using Smdb.Core.Shared;

public class MemoryActorMovieRepository : IActorMovieRepository
{
	private MemoryDatabase db;

	public MemoryActorMovieRepository(MemoryDatabase db)
	{
		this.db = db;
	}

	public async Task<ActorMovie?> Create(ActorMovie newData)
	{
		newData.Id = db.NextActorMovieId();

		db.ActorsMovies.Add(newData);

		return await Task.FromResult(newData);
	}

	public async Task<ActorMovie?> Read(int id)
	{
		ActorMovie? actorMovie = db.ActorsMovies.FirstOrDefault((am) => am.Id == id);

		return await Task.FromResult(actorMovie);
	}

	public async Task<ActorMovie?> Update(int id, ActorMovie updatedData)
	{
		ActorMovie? actorMovie = db.ActorsMovies.FirstOrDefault((am) => am.Id == id);

		if (actorMovie != null)
		{
			actorMovie.ActorId = updatedData.ActorId;
			actorMovie.MovieId = updatedData.MovieId;
			actorMovie.RoleName = updatedData.RoleName;
		}

		return await Task.FromResult(actorMovie);
	}

	public async Task<ActorMovie?> Delete(int id)
	{
		ActorMovie? actorMovie = db.ActorsMovies.FirstOrDefault((am) => am.Id == id);

		if (actorMovie != null)
		{
			db.ActorsMovies.Remove(actorMovie);
		}

		return await Task.FromResult(actorMovie);
	}

	public async Task<PagedResult<ActorMovie>> List(int page, int size)
	{
		int totalCount = db.ActorsMovies.Count;
		int start = Math.Clamp((page - 1) * size, 0, totalCount);
		int length = Math.Clamp(size, 0, totalCount - start);
		List<ActorMovie> values = db.ActorsMovies.Slice(start, length);
		var pagedResult = new PagedResult<ActorMovie>(totalCount, values);

		return await Task.FromResult(pagedResult);
	}

	public async Task<PagedResult<(ActorMovie, Movie)>> ListMoviesByActor(int actorId, int page, int size)
	{
		List<ActorMovie> ams = db.ActorsMovies.FindAll((am) => am.ActorId == actorId);
		List<(ActorMovie, Movie)> movies = [];

		foreach (var am in ams)
		{
			var movie = db.Movies.FirstOrDefault(m => m.Id == am.MovieId)!;
			movies.Add((am, movie));
		}

		int totalCount = movies.Count;
		int start = Math.Clamp((page - 1) * size, 0, totalCount);
		int length = Math.Clamp(size, 0, totalCount - start);
		List<(ActorMovie, Movie)> values = movies.Slice(start, length);
		var pagedResult = new PagedResult<(ActorMovie, Movie)>(totalCount, values);

		return await Task.FromResult(pagedResult);
	}

	public async Task<PagedResult<(ActorMovie, Actor)>> ListActorsByMovie(int movieId, int page, int size)
	{
		List<ActorMovie> ams = db.ActorsMovies.FindAll((am) => am.MovieId == movieId);
		List<(ActorMovie, Actor)> actors = [];

		foreach (var am in ams)
		{
			var actor = db.Actors.FirstOrDefault(a => a.Id == am.ActorId)!;
			actors.Add((am, actor));
		}

		int totalCount = actors.Count;
		int start = Math.Clamp((page - 1) * size, 0, totalCount);
		int length = Math.Clamp(size, 0, totalCount - start);
		List<(ActorMovie, Actor)> values = actors.Slice(start, length);
		var pagedResult = new PagedResult<(ActorMovie, Actor)>(totalCount, values);

		return await Task.FromResult(pagedResult);
	}

	public async Task<List<Actor>> ListActors()
	{
		return await Task.FromResult(db.Actors);
	}

	public async Task<List<Movie>> ListMovies()
	{
		return await Task.FromResult(db.Movies);
	}
}
