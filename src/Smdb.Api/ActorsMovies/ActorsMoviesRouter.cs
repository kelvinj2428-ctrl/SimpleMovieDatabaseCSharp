namespace Smdb.Api.ActorsMovies;

using Abcs.Http;

public class ActorsMoviesRouter : HttpRouter
{
	public ActorsMoviesRouter(ActorsMoviesController actorsmoviesController)
	{
		UseParametrizedRouteMatching();
		MapGet("/", actorsmoviesController.ReadActorsMovies);
		MapPost("/", HttpUtils.ReadRequestBodyAsText, actorsmoviesController.Create);
		MapGet("/:id", actorsmoviesController.ListMovies);
		MapPut("/:id", HttpUtils.ReadRequestBodyAsText, actorsmoviesController.Update);
		MapDelete("/:id", actorsmoviesController.Delete);
	}
}
