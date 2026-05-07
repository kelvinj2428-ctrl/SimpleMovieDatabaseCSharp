namespace Smdb.Api.ActorsMovies;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Abcs.Http;
using Smdb.Core.ActorsMovies;

public class ActorsMoviesController
{
	private IActorMovieService actorsMovieservice;

	public ActorsMoviesController(IActorMovieService actorsMovieservice)
	{
		this.actorsMovieservice = actorsMovieservice;
	}

	// curl -X GET "http://localhost:8080/api/v1/actorsmovies?page=2&size=10"

	public async Task ReadActorsMovies(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;

		var result = await actorsMovieservice.ReadActorsMovies(page, size);

		await JsonUtils.SendPagedResultResponse(req, res, props, result, page, size);

		await next();
	}

	// curl -X POST "http://localhost:8080/actorsmovies" -H "Content-Type: application/json" -d "{ \"id\": -1, \"title\": \"Inception\", \"year\": 2010, \"description\": \"A skilled thief who enters dreams to steal secrets.\" }"

	public async Task CreateActorMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var text = (string)props["req.text"]!;
		var actormovie = JsonSerializer.Deserialize<ActorMovie>(text, JsonUtils.DefaultOptions);
		var result = await actorsMovieservice.CreateActorMovie(actormovie!);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X GET "http://localhost:8080/actorsmovies/1"

	public async Task ReadActorMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

		var result = await actorsMovieservice.ReadActorMovie(id);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X PUT "http://localhost:8080/actorsmovies/1" -H "Content-Type: application/json" -d "{ \"title\": \"Joker 2\", \"year\": 2020, \"description\": \"A man that is a joke.\" }"

	public async Task UpdateActorMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var text = (string)props["req.text"]!;
		var actormovie = JsonSerializer.Deserialize<ActorMovie>(text, JsonUtils.DefaultOptions);
		var result = await actorsMovieservice.UpdateActorMovie(id, actormovie!);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X DELETE http://localhost:8080/actorsmovies/1

	public async Task DeleteActorMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

		var result = await actorsMovieservice.DeleteActorMovie(id);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}
}
