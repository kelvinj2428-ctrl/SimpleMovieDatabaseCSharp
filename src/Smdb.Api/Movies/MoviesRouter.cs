namespace Smdb.Api.Movies;

using Abcs.Http;

public class MoviesRouter : HttpRouter
{
	public MoviesRouter(MoviesController moviesController)
	{
		UseParametrizedRouteMatching();
		MapGet("/", moviesController.Read);
		MapPost("/", HttpUtils.ReadRequestBodyAsText, moviesController.Create);
		MapGet("/:id", moviesController.Read);
		MapPut("/:id", HttpUtils.ReadRequestBodyAsText, moviesController.Update);
		MapDelete("/:id", moviesController.Delete);
	}
}
