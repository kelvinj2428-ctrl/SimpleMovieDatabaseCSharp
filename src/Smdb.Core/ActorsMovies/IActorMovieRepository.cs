namespace Smdb.Core.ActorsMovies;

using Abcs.Http;
using Smdb.Core.Actors;
using Smdb.Core.Movies;

public interface IActorMovieRepository
{
	public Task<ActorMovie?> Create(ActorMovie newData);
	public Task<ActorMovie?> Read(int id);
	public Task<ActorMovie?> Update(int id, ActorMovie updatedData);
	public Task<ActorMovie?> Delete(int id);
	public Task<PagedResult<ActorMovie>> List(int page, int size);
	public Task<PagedResult<(ActorMovie, Movie)>> ListMoviesByActor(int actorId, int page, int size);
	public Task<PagedResult<(ActorMovie, Actor)>> ListActorsByMovie(int movieId, int page, int size);
	public Task<List<Actor>> ListActors();
	public Task<List<Movie>> ListMovies();
}
