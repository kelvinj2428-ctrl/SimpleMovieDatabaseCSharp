namespace Smdb.Api.Users;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using Abcs.Http;
using Smdb.Core.Users;

public class UsersController
{
	private IUserService userService;

	public UsersController(IUserService userService)
	{
		this.userService = userService;
	}

	// curl -X GET "http://localhost:8080/api/v1/users?page=2&size=10"

	public async Task ReadUsers(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;

		var result = await userService.ReadUsers(page, size);

		await JsonUtils.SendPagedResultResponse(req, res, props, result, page, size);

		await next();
	}

	// curl -X POST "http://localhost:8080/users" -H "Content-Type: application/json" -d "{ \"id\": -1, \"title\": \"Inception\", \"year\": 2010, \"description\": \"A skilled thief who enters dreams to steal secrets.\" }"

	public async Task CreateUser(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var text = (string)props["req.text"]!;
		var user = JsonSerializer.Deserialize<User>(text, JsonUtils.DefaultOptions);
		var result = await userService.CreateUser(user!);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X GET "http://localhost:8080/users/1"

	public async Task ReadUser(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

		var result = await userService.ReadUser(id);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X PUT "http://localhost:8080/users/1" -H "Content-Type: application/json" -d "{ \"title\": \"Joker 2\", \"year\": 2020, \"description\": \"A man that is a joke.\" }"

	public async Task UpdateUser(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;
		var text = (string)props["req.text"]!;
		var user = JsonSerializer.Deserialize<User>(text, JsonUtils.DefaultOptions);
		var result = await userService.UpdateUser(id, user!);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}

	// curl -X DELETE http://localhost:8080/users/1

	public async Task DeleteUser(HttpListenerRequest req, HttpListenerResponse res, Hashtable props, Func<Task> next)
	{
		var uParams = (NameValueCollection)props["urlParams"]!;
		int id = int.TryParse(uParams["id"]!, out int i) ? i : -1;

		var result = await userService.DeleteUser(id);

		await JsonUtils.SendResultResponse(req, res, props, result);

		await next();
	}
}
