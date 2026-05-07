namespace Smdb.Ssr.Movies;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using Abcs.Http;
using Smdb.Core.Movies;
using Smdb.Core.Shared;

public class MovieController
{
	private IMovieService movieService;

	public MovieController(IMovieService movieService)
	{
		this.movieService = movieService;
	}

	// GET /movies?page=1&size=5
	public async Task ViewAllMoviesGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

		Result<PagedResult<Movie>> result = await movieService.List(page, size);

		if (!result.IsError)
		{
			PagedResult<Movie> pagedResult = result.Payload!;
			List<Movie> movies = pagedResult.Values;
			int movieCount = pagedResult.TotalCount;

			string html = MovieHtmlTemplates.ViewAllMoviesGet(movies, movieCount, page, size);
			string content = HtmlTemplates.Base("SimpleMDB", "Movies View All Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/");
		}
	}

	// GET /movies/add
	public async Task AddMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string title = req.QueryString["title"] ?? "";
		string year = req.QueryString["year"] ?? "";
		string description = req.QueryString["description"] ?? "";
		string rating = req.QueryString["rating"] ?? "";
		string message = req.QueryString["message"] ?? "";

		string html = MovieHtmlTemplates.AddMovieGet(title, year, description, rating);
		string content = HtmlTemplates.Base("SimpleMDB", "Movies Add Page", html, message);

		await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
	}

	// POST /movies/add
	public async Task AddMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var formData = (NameValueCollection?)options["req.form"] ?? [];

		string title = formData["title"] ?? "";
		int year = int.TryParse(formData["year"], out int y) ? y : DateTime.Now.Year;
		string description = formData["description"] ?? "";
		float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

		Movie newMovie = new Movie(0, title, year, description, rating);

		Result<Movie> result = await movieService.Create(newMovie);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "Movie added succesfully!");

			await HttpUtils.Redirect(req, res, options, "/movies"); // PRG
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			HttpUtils.AddOptions(options, "redirect", formData);

			await HttpUtils.Redirect(req, res, options, "/movies/add");
		}
	}

	// GET /movies/view?mid=1
	public async Task ViewMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : -1;

		Result<Movie> result = await movieService.Read(mid);

		if (!result.IsError)
		{
			Movie movie = result.Payload!;

			string html = MovieHtmlTemplates.ViewMovieGet(movie);
			string content = HtmlTemplates.Base("SimpleMDB", "Movies View Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/movies");
		}
	}

	// GET /movies/edit?mid=1
	public async Task EditMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : -1;

		Result<Movie> result = await movieService.Read(mid);

		if (!result.IsError)
		{
			Movie movie = result.Payload!;

			string html = MovieHtmlTemplates.EditMovieGet(mid, movie);
			string content = HtmlTemplates.Base("SimpleMDB", "Movies Edit Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/movies");
		}
	}

	// POST /movies/edit?mid=1
	public async Task EditMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : -1;

		var formData = (NameValueCollection?)options["req.form"] ?? [];

		string title = formData["title"] ?? "";
		int year = int.TryParse(formData["year"], out int y) ? y : DateTime.Now.Year;
		string description = formData["description"] ?? "";
		float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

		Movie newMovie = new Movie(0, title, year, description, rating);

		Result<Movie> result = await movieService.Update(mid, newMovie);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "Movie edited succesfully!");
			await HttpUtils.Redirect(req, res, options, "/movies");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

			await HttpUtils.Redirect(req, res, options, $"/movies/edit?mid={mid}");
		}
	}

	// POST /movies/remove?mid=1
	public async Task RemoveMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : -1;

		Result<Movie> result = await movieService.Delete(mid);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "Movie removed succesfully!");
			await HttpUtils.Redirect(req, res, options, "/movies");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/movies");
		}
	}
}
