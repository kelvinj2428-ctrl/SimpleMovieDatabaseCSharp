namespace Smdb.Api.Auths;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Abcs.Http;
using Smdb.Core.Auth;

public class AuthsController
{
	private IAuthService authService;

	public AuthsController(IAuthService authService)
	{
		this.authService = authService;
	}

	// curl -X GET "http://localhost:8080/api/v1/auths?page=2&size=10"

	public async Task ReadAuths(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;

		var result = await authService.ReadAuths(page, size);

		await JsonUtils.SendPagedResultResponse(req, res, props, result, page, size);

		await next();
	}

	// curl -X POST "http://localhost:8080/auths" -H "Content-Type: application/json" -d "{ \"id\": -1, \"title\": \"Inception\", \"year\": 2010, \"description\": \"A skilled thief who enters dreams to steal secrets.\" }"

	public async Task CreateAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var text = (string)props["req.text"]!;
		var auth = JsonSerializer.Deserialize<Auth>(text, JsonUtils.DefaultOptions);
		var result = await authService.CreateAuth(auth!);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X GET "http://localhost:8080/auths/1"

	public async Task ReadAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

		var result = await authService.ReadAuth(id);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X PUT "http://localhost:8080/auths/1" -H "Content-Type: application/json" -d "{ \"title\": \"Joker 2\", \"year\": 2020, \"description\": \"A man that is a joke.\" }"

	public async Task UpdateAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var text = (string)props["req.text"]!;
		var auth = JsonSerializer.Deserialize<Auth>(text, JsonUtils.DefaultOptions);
		var result = await authService.UpdateAuth(id, auth!);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X DELETE http://localhost:8080/auths/1

	public async Task DeleteAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

		var result = await authService.DeleteAuth(id);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}
}
