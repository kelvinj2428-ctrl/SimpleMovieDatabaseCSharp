namespace Smdb.Api.Auths;

using Abcs.Http;

public class AuthsRouter : HttpRouter
{
	public AuthsRouter(AuthController authsController)
	{
		UseParametrizedRouteMatching();
		MapGet("/", authsController.ReadAuths);
		MapPost("/", HttpUtils.ReadRequestBodyAsText, authsController.CreateAuth);
		MapGet("/:id", authsController.ReadAuth);
		MapPut("/:id", HttpUtils.ReadRequestBodyAsText, authsController.UpdateAuth);
		MapDelete("/:id", authsController.DeleteAuth);
	}
}
