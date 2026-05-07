namespace Smdb.Ssr.ActorsMovies;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using Abcs.Http;
using Smdb.Core.Actors;
using Smdb.Core.ActorsMovies;
using Smdb.Core.Movies;
using Smdb.Core.Shared;

public class ActorMovieController
{
	private IActorMovieService actorMovieService;
	private IActorService actorService;
	private IMovieService movieService;

	public ActorMovieController(IActorMovieService actorMovieService, IActorService actorService, IMovieService movieService)
	{
		this.actorMovieService = actorMovieService;
		this.actorService = actorService;
		this.movieService = movieService;
	}

	// GET /actors/movies?aid=1&page=1&size=5
	public async Task ViewAllMoviesByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

		var result1 = await actorService.Read(aid);
		var result2 = await actorMovieService.ListMoviesByActor(aid, page, size);

		if (!result1.IsError && !result2.IsError)
		{
			var actor = result1.Payload!;
			var pagedResult = result2.Payload!;
			var movies = pagedResult.Values;
			int totalCount = pagedResult.TotalCount;

			string html = ActorMovieHtmlTemplates.ViewAllMoviesByActor(actor, movies, totalCount, page, size);
			string content = HtmlTemplates.Base("SimpleMDB", "View All Movies By Actor Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			string error = !result1.IsError ? "" : result1.Error!.Message;
			error += !result2.IsError ? "" : result2.Error!.Message;

			HttpUtils.AddOptions(options, "redirect", "message", error);
			await HttpUtils.Redirect(req, res, options, "/");
		}
	}

	// GET /movies/actors?mid=1&page=1&size=5
	public async Task ViewAllActorsByMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

		var result1 = await movieService.Read(mid);
		var result2 = await actorMovieService.ListActorsByMovie(mid, page, size);

		if (!result1.IsError && !result2.IsError)
		{
			var movie = result1.Payload!;
			var pagedResult = result2.Payload!;
			var actors = pagedResult.Values;
			int totalCount = pagedResult.TotalCount;

			string html = ActorMovieHtmlTemplates.ViewAllActorsByMovie(movie, actors, totalCount, page, size);
			string content = HtmlTemplates.Base("SimpleMDB", "View All Actors By Movie Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			string error = !result1.IsError ? "" : result1.Error!.Message;
			error += !result2.IsError ? "" : result2.Error!.Message;

			HttpUtils.AddOptions(options, "redirect", "message", error);
			await HttpUtils.Redirect(req, res, options, "/");
		}
	}

	// GET /actors/movies/add?aid=1
	public async Task AddMoviesByActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;

		var result1 = await actorService.Read(aid);
		var result2 = await actorMovieService.ListMovies();

		if (!result1.IsError && !result2.IsError)
		{
			var actor = result1.Payload!;
			var movies = result2.Payload!;

			string html = ActorMovieHtmlTemplates.AddMoviesByActor(actor, movies);
			string content = HtmlTemplates.Base("SimpleMDB", "Add Movies By Actor Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			string error = !result1.IsError ? "" : result1.Error!.Message;
			error += !result2.IsError ? "" : result2.Error!.Message;

			HttpUtils.AddOptions(options, "redirect", "message", error);
			await HttpUtils.Redirect(req, res, options, "/");
		}
	}

	// GET /movies/actors/add?aid=1
	public async Task AddActorsByMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;

		var result1 = await movieService.Read(mid);
		var result2 = await actorMovieService.ListActors();

		if (!result1.IsError && !result2.IsError)
		{
			var movie = result1.Payload!;
			var actors = result2.Payload!;

			string html = ActorMovieHtmlTemplates.AddActorsByMovie(movie, actors);
			string content = HtmlTemplates.Base("SimpleMDB", "Add Actors By Movie Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			string error = !result1.IsError ? "" : result1.Error!.Message;
			error += !result2.IsError ? "" : result2.Error!.Message;

			HttpUtils.AddOptions(options, "redirect", "message", error);
			await HttpUtils.Redirect(req, res, options, "/");
		}
	}

	// POST /actors/movies/add
	public async Task AddMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var formData = (NameValueCollection?)options["req.form"] ?? [];
		var aid = int.TryParse(formData["aid"], out int a) ? a : -1;
		var mid = int.TryParse(formData["mid"], out int m) ? m : -1;
		var rolename = formData["rolename"] ?? "Popo";

		var result = await actorMovieService.Create(new ActorMovie(-1, aid, mid, rolename));

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie added succesfully!");

			await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={aid}");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			HttpUtils.AddOptions(options, "redirect", formData);

			await HttpUtils.Redirect(req, res, options, $"/actors/movies/add?aid={aid}");
		}
	}

	// POST /movies/actors/add
	public async Task AddActorsByMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var formData = (NameValueCollection?)options["req.form"] ?? [];
		var mid = int.TryParse(formData["mid"], out int m) ? m : -1;
		var aid = int.TryParse(formData["aid"], out int a) ? a : -1;
		var rolename = formData["rolename"] ?? "Popo";

		var result = await actorMovieService.Create(new ActorMovie(-1, aid, mid, rolename));

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie added succesfully!");

			await HttpUtils.Redirect(req, res, options, $"/movies/actors?mid={mid}");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			HttpUtils.AddOptions(options, "redirect", formData);

			await HttpUtils.Redirect(req, res, options, $"/movies/actors/add?mid={mid}");
		}
	}

	// POST /actors/movies/remove?amid=1
	public async Task RemoveMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : -1;

		Result<ActorMovie> result = await actorMovieService.Delete(amid);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie removed succesfully!");
			await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={result.Payload!.ActorId}");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, $"/actors");
		}
	}

	// POST /movies/actors/remove?amid=1
	public async Task RemoveActorsByMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : -1;

		Result<ActorMovie> result = await actorMovieService.Delete(amid);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie removed succesfully!");
			await HttpUtils.Redirect(req, res, options, $"/movies/actors?mid={result.Payload!.MovieId}");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, $"/movies");
		}
	}
}
