namespace Smdb.Core.ActorsMovies;

using Abcs.Http;
using Smdb.Core.Movies;
using Smdb.Core.Actors;
using Smdb.Core.ActorsMovies;

public class DefaultActorMovieService : IActorMovieService
{
	private IActorMovieRepository actorMovieRepository;

	public DefaultActorMovieService(IActorMovieRepository actorMovieRepository)
	{
		this.actorMovieRepository = actorMovieRepository;
	}

	public async Task<Result<ActorMovie>> Create(ActorMovie newData)
	{
		ActorMovie? actorMovie = await actorMovieRepository.Create(newData);

		var result = (actorMovie == null) ?
			new Result<ActorMovie>(new Exception("ActorMovie could not be created.")) :
			new Result<ActorMovie>(actorMovie);

		return await Task.FromResult(result);
	}

	public async Task<Result<ActorMovie>> Read(int id)
	{
		ActorMovie? actorMovie = await actorMovieRepository.Read(id);

		var result = (actorMovie == null) ?
			new Result<ActorMovie>(new Exception("ActorMovie could not be read.")) :
			new Result<ActorMovie>(actorMovie);

		return await Task.FromResult(result);
	}

	public async Task<Result<ActorMovie>> Update(int id, ActorMovie updatedData)
	{
		ActorMovie? actorMovie = await actorMovieRepository.Update(id, updatedData);

		var result = (actorMovie == null) ?
			new Result<ActorMovie>(new Exception("ActorMovie could not be updated.")) :
			new Result<ActorMovie>(actorMovie);

		return await Task.FromResult(result);
	}

	public async Task<Result<ActorMovie>> Delete(int id)
	{
		ActorMovie? actorMovie = await actorMovieRepository.Delete(id);

		var result = (actorMovie == null) ?
			new Result<ActorMovie>(new Exception("ActorMovie could not be deleted.")) :
			new Result<ActorMovie>(actorMovie);

		return await Task.FromResult(result);
	}

	public async Task<Result<PagedResult<ActorMovie>>> List(int page, int size)
	{
		var pagedResult = await actorMovieRepository.List(page, size);

		var result = (pagedResult == null) ?
			new Result<PagedResult<ActorMovie>>(new Exception("No results found.")) :
			new Result<PagedResult<ActorMovie>>(pagedResult);

		return await Task.FromResult(result);
	}

	public async Task<Result<PagedResult<(ActorMovie, Movie)>>> ListMoviesByActor(int actorId, int page, int size)
	{
		var pagedResult = await actorMovieRepository.ListMoviesByActor(actorId, page, size);

		var result = (pagedResult == null) ?
			new Result<PagedResult<(ActorMovie, Movie)>>(new Exception("No movies by actor results found.")) :
			new Result<PagedResult<(ActorMovie, Movie)>>(pagedResult);

		return await Task.FromResult(result);
	}

	public async Task<Result<PagedResult<(ActorMovie, Actor)>>> ListActorsByMovie(int movieId, int page, int size)
	{
		var pagedResult = await actorMovieRepository.ListActorsByMovie(movieId, page, size);

		var result = (pagedResult == null) ?
			new Result<PagedResult<(ActorMovie, Actor)>>(new Exception("No actors by movie results found.")) :
			new Result<PagedResult<(ActorMovie, Actor)>>(pagedResult);

		return await Task.FromResult(result);
	}

	public async Task<Result<List<Actor>>> ListActors()
	{
		var pagedResult = await actorMovieRepository.ListActors();

		var result = (pagedResult == null) ?
			new Result<List<Actor>>(new Exception("No actors results found.")) :
			new Result<List<Actor>>(pagedResult);

		return await Task.FromResult(result);
	}

	public async Task<Result<List<Movie>>> ListMovies()
	{
		var pagedResult = await actorMovieRepository.ListMovies();

		var result = (pagedResult == null) ?
			new Result<List<Movie>>(new Exception("No movies results found.")) :
			new Result<List<Movie>>(pagedResult);

		return await Task.FromResult(result);
	}
}
