namespace Smdb.Core.ActorsMovies;

using Abcs.Http;
using Smdb.Core.Actors;
using Smdb.Core.Movies;

public interface IActorMovieService
{
	public Task<Result<ActorMovie>> Create(ActorMovie newData);
	public Task<Result<ActorMovie>> Read(int id);
	public Task<Result<ActorMovie>> Update(int id, ActorMovie updatedData);
	public Task<Result<ActorMovie>> Delete(int id);
	public Task<Result<PagedResult<ActorMovie>>> List(int page, int size);
	public Task<Result<PagedResult<(ActorMovie, Movie)>>> ListMoviesByActor(int actorId, int page, int size);
	public Task<Result<PagedResult<(ActorMovie, Actor)>>> ListActorsByMovie(int movieId, int page, int size);
	public Task<Result<List<Actor>>> ListActors();
	public Task<Result<List<Movie>>> ListMovies();
}
