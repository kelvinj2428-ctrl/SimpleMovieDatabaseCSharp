namespace Smdb.Ssr;

using System.Collections;
using System.Net;
using Abcs.Http;
using Smdb.Core.Actors;
using Smdb.Core.ActorsMovies;
using Smdb.Core.Movies;
using Smdb.Core.Users;
using Smdb.Core.Shared;
using Smdb.Ssr.Actors;
using Smdb.Ssr.ActorsMovies;
using Smdb.Ssr.Auth;
using Smdb.Ssr.Movies;
using Smdb.Ssr.Users;

public class App : HttpServer
{
	public App()
	{
	}

	public override void Init()
	{
		//var userRepository = new MockUserRepository();
		var userRepository = new MySqlUserRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=54321;");
		var userService = new DefaultUserService(userRepository);
		var userController = new UserController(userService);
		var authController = new AuthController(userService);

		//var actorRepository = new MockActorRepository();
		var actorRepository = new MySqlActorRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=54321;");
		var actorService = new DefaultActorService(actorRepository);
		var actorController = new ActorController(actorService);

		//var movieRepository = new MockMovieRepository();
		var movieRepository = new MySqlMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=54321;");
		var movieService = new DefaultMovieService(movieRepository);
		var movieController = new MovieController(movieService);

		//var actorMovieRepository = new MockActorMovieRepository(actorRepository, movieRepository);
		var actorMovieRepository = new MySqlActorMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=54321;");
		var actorMovieService = new DefaultActorMovieService(actorMovieRepository);
		var actorMovieController = new ActorMovieController(actorMovieService, actorService, movieService);

		router.Use(HttpUtils.StructuredLogging);
		router.Use(HttpUtils.CentralizedErrorHandling);
		router.Use(HttpUtils.AddResponseCorsHeaders);
		router.Use(HttpUtils.DefaultResponse);
		router.Use(HttpUtils.ParseRequestUrl);
		router.Use(HttpUtils.ParseRequestQueryString);
		router.Use(HttpUtils.ReadRequestBodyAsForm);
		router.Use(HttpUtils.ServeStaticFiles);
		router.UseSimpleRouteMatching();

		router.MapGet("/", authController.LandingPageGet);
		router.MapGet("/register", authController.RegisterGet);
		router.MapPost("/register", authController.RegisterPost);
		router.MapGet("/login", authController.LoginGet);
		router.MapPost("/login", authController.LoginPost);
		router.MapPost("/logout", authController.LogoutPost);

		router.MapGet("/users", authController.CheckAdmin, userController.ViewAllUsersGet);
		router.MapGet("/users/add", authController.CheckAdmin, userController.AddUserGet);
		router.MapPost("/users/add", authController.CheckAdmin, userController.AddUserPost);
		router.MapGet("/users/view", authController.CheckAdmin, userController.ViewUserGet);
		router.MapGet("/users/edit", authController.CheckAdmin, userController.EditUserGet);
		router.MapPost("/users/edit", authController.CheckAdmin, userController.EditUserPost);
		router.MapPost("/users/remove", authController.CheckAdmin, userController.RemoveUserPost);

		router.MapGet("/actors", actorController.ViewAllActorsGet);
		router.MapGet("/actors/add", authController.CheckAuth, actorController.AddActorGet);
		router.MapPost("/actors/add", authController.CheckAuth, actorController.AddActorPost);
		router.MapGet("/actors/view", authController.CheckAuth, actorController.ViewActorGet);
		router.MapGet("/actors/edit", authController.CheckAuth, actorController.EditActorGet);
		router.MapPost("/actors/edit", authController.CheckAuth, actorController.EditActorPost);
		router.MapPost("/actors/remove", authController.CheckAuth, actorController.RemoveActorPost);

		router.MapGet("/movies", movieController.ViewAllMoviesGet);
		router.MapGet("/movies/add", authController.CheckAuth, movieController.AddMovieGet);
		router.MapPost("/movies/add", authController.CheckAuth, movieController.AddMoviePost);
		router.MapGet("/movies/view", authController.CheckAuth, movieController.ViewMovieGet);
		router.MapGet("/movies/edit", authController.CheckAuth, movieController.EditMovieGet);
		router.MapPost("/movies/edit", authController.CheckAuth, movieController.EditMoviePost);
		router.MapPost("/movies/remove", authController.CheckAuth, movieController.RemoveMoviePost);

		router.MapGet("/actors/movies", authController.CheckAuth, actorMovieController.ViewAllMoviesByActor);
		router.MapGet("/actors/movies/add", authController.CheckAuth, actorMovieController.AddMoviesByActorGet);
		router.MapPost("/actors/movies/add", authController.CheckAuth, actorMovieController.AddMoviesByActorPost);
		router.MapPost("/actors/movies/remove", authController.CheckAuth, actorMovieController.RemoveMoviesByActorPost);

		router.MapGet("/movies/actors", authController.CheckAuth, actorMovieController.ViewAllActorsByMovie);
		router.MapGet("/movies/actors/add", authController.CheckAuth, actorMovieController.AddActorsByMovieGet);
		router.MapPost("/movies/actors/add", authController.CheckAuth, actorMovieController.AddActorsByMoviePost);
		router.MapPost("/movies/actors/remove", authController.CheckAuth, actorMovieController.RemoveActorsByMoviePost);
	}
}


