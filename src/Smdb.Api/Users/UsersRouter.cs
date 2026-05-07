namespace Smdb.Api.Users;

using Abcs.Http;

public class UsersRouter : HttpRouter
{
	public UsersRouter(UsersController usersController)
	{
		UseParametrizedRouteMatching();
		MapGet("/", usersController.ReadUsers);
		MapPost("/", HttpUtils.ReadRequestBodyAsText, usersController.CreateUser);
		MapGet("/:id", usersController.ReadUser);
		MapPut("/:id", HttpUtils.ReadRequestBodyAsText, usersController.UpdateUser);
		MapDelete("/:id", usersController.DeleteUser);
	}
}
