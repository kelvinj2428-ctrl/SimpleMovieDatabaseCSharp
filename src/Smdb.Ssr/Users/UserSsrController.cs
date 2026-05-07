namespace Smdb.Ssr.Users;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using Abcs.Http;
using Smdb.Core.Shared;
using Smdb.Core.Users;

public class UserController
{
	private IUserService userService;

	public UserController(IUserService userService)
	{
		this.userService = userService;
	}

	// GET /users?page=1&size=5
	public async Task ViewAllUsersGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

		Result<PagedResult<User>> result = await userService.List(page, size);

		if (!result.IsError)
		{
			PagedResult<User> pagedResult = result.Payload!;
			List<User> users = pagedResult.Values;
			int userCount = pagedResult.TotalCount;

			string html = UserHtmlTemplates.ViewAllUsersGet(users, userCount, page, size);
			string content = HtmlTemplates.Base("SimpleMDB", "Users View All Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/");
		}
	}

	// GET /users/add
	public async Task AddUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string username = req.QueryString["username"] ?? "";
		string role = req.QueryString["role"] ?? "";
		string message = req.QueryString["message"] ?? "";

		string html = UserHtmlTemplates.AddUserGet(username, role);
		string content = HtmlTemplates.Base("SimpleMDB", "Users Add Page", html, message);

		await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
	}

	// POST /users/add
	public async Task AddUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var formData = (NameValueCollection?)options["req.form"] ?? [];

		string username = formData["username"] ?? "";
		string password = formData["password"] ?? "";
		string role = formData["role"] ?? "";

		User newUser = new User(0, username, password, "", role);

		Result<User> result = await userService.Create(newUser);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "User added succesfully!");

			await HttpUtils.Redirect(req, res, options, "/users"); // PRG
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			HttpUtils.AddOptions(options, "redirect", "username", username);
			HttpUtils.AddOptions(options, "redirect", "role", role);

			await HttpUtils.Redirect(req, res, options, "/users/add");
		}
	}

	// GET /users/view?uid=1
	public async Task ViewUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

		Result<User> result = await userService.Read(uid);

		if (!result.IsError)
		{
			User user = result.Payload!;

			string html = UserHtmlTemplates.ViewUserGet(user);
			string content = HtmlTemplates.Base("SimpleMDB", "Users View Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/users");
		}
	}

	// GET /users/edit?uid=1
	public async Task EditUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

		Result<User> result = await userService.Read(uid);

		if (!result.IsError)
		{
			User user = result.Payload!;

			string html = UserHtmlTemplates.EditUserGet(uid, user);
			string content = HtmlTemplates.Base("SimpleMDB", "Users Edit Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/users");
		}
	}

	// POST /users/edit?uid=1
	public async Task EditUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

		var formData = (NameValueCollection?)options["req.form"] ?? [];

		string username = formData["username"] ?? "";
		string password = formData["password"] ?? "";
		string role = formData["role"] ?? "";

		User newUser = new User(0, username, password, "", role);

		Result<User> result = await userService.Update(uid, newUser);

		Console.WriteLine("#######");
		Console.WriteLine(newUser);
		Console.WriteLine(result);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "User edited succesfully!");
			await HttpUtils.Redirect(req, res, options, "/users");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, $"/users/edit?uid={uid}");
		}
	}

	// POST /users/remove?uid=1
	public async Task RemoveUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

		Result<User> result = await userService.Delete(uid);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "User removed succesfully!");
			await HttpUtils.Redirect(req, res, options, "/users");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/users");
		}
	}
}
