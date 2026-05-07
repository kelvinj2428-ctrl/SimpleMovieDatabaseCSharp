namespace Smdb.Ssr.Auth;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using Smdb.Core.Shared;
using Smdb.Core.Users;
using Abcs.Http;

public class AuthController
{
	private IUserService userService;

	public AuthController(IUserService userService)
	{
		this.userService = userService;
	}

	// GET /
	public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		string html = $@"
			<main class=""landing-page"">
				<section class=""hero"">
					<h1>Discover Movies, Casts, and Ratings</h1>
					<p>
						Browse a simple movie database with popular titles, genres, release years,
						cast details, and audience ratings.
					</p>

					<form class=""search-box"">
						<input type=""text"" placeholder=""Search for a movie..."" />
						<button type=""submit"">Search</button>
					</form>
				</section>

				<section class=""featured"">
					<h2>Featured Movies</h2>

					<div class=""movie-grid"">
						<article>This Feature is Coming Soon!</article>
					</div>
				</section>

				<section class=""categories"">
					<h2>Browse by Genre</h2>

					<div class=""genre-list"">
						<article>This Feature is Coming Soon!</article>
					</div>
				</section>

				<section class=""about"">
					<h2>About This Database</h2>
					<p>
						This movie database helps users quickly find movie information, compare
						ratings, and explore films by genre, year, or title.
					</p>
				</section>
			</main>
    ";

		string content = HtmlTemplates.Base("SimpleMDB", "Simple Movie Database", html, message);
		await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
	}

	// GET /register
	public async Task RegisterGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var returnUrl = req.QueryString["returnUrl"] ?? "";
		var rQuery = string.IsNullOrWhiteSpace(returnUrl) ? "" : $"?returnUrl={HttpUtility.UrlEncode(returnUrl)}";

		string message = req.QueryString["message"] ?? "";
		string username = req.QueryString["username"] ?? "";

		string html = $@"
    <form class=""authform"" action=""/register{rQuery}"" method=""POST"">
      <label for=""username"">Username</label>
      <input id=""username"" name=""username"" type=""text"" placeholder=""Username"" value=""{username}"">
      <label for=""password"">Password</label>
      <input id=""password"" name=""password"" type=""password"" placeholder=""Password"">
      <label for=""cpassword"">Confirm Password</label>
      <input id=""cpassword"" name=""cpassword"" type=""password"" placeholder=""Confirm Password"">
      <input type=""submit"" value=""Register"">
    </form>
    ";

		string content = HtmlTemplates.Base("SimpleMDB", "Register Page", html, message);
		await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
	}

	// POST /register
	public async Task RegisterPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var returnUrl = req.QueryString["returnUrl"] ?? "";

		var formData = (NameValueCollection?)options["req.form"] ?? [];
		var username = formData["username"] ?? "";
		var password = formData["password"] ?? "";
		var cpassword = formData["cpassword"] ?? "";

		Console.WriteLine($"username={username}");

		if (password != cpassword)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "Passwords do not match.");
			HttpUtils.AddOptions(options, "redirect", "username", username);
			HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);

			await HttpUtils.Redirect(req, res, options, $"/register");
		}
		else
		{
			User newUser = new User(0, username, password, "", "");
			var result = await userService.Create(newUser);

			if (!result.IsError)
			{
				HttpUtils.AddOptions(options, "redirect", "message", "User registered successfully!");
				HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);

				await HttpUtils.Redirect(req, res, options, $"/login");
			}
			else
			{
				HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
				HttpUtils.AddOptions(options, "redirect", "username", username);
				HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);

				await HttpUtils.Redirect(req, res, options, $"/register");
			}
		}
	}

	// GET /login
	public async Task LoginGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var returnUrl = req.QueryString["returnUrl"] ?? "";
		var rQuery = string.IsNullOrWhiteSpace(returnUrl) ? "" : $"?returnUrl={HttpUtility.UrlEncode(returnUrl)}";

		string message = req.QueryString["message"] ?? "";
		string username = req.QueryString["username"] ?? "";

		string html = $@"
    <form class=""authform"" action=""/login{rQuery}"" method=""POST"">
      <label for=""username"">Username</label>
      <input id=""username"" name=""username"" type=""text"" placeholder=""Username"" value=""{username}"">
      <label for=""password"">Password</label>
      <input id=""password"" name=""password"" type=""password"" placeholder=""Password"">
      <input type=""submit"" value=""Login"">
    </form>
    ";

		string content = HtmlTemplates.Base("SimpleMDB", "Login Page", html, message);
		await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
	}

	// POST /login
	public async Task LoginPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var returnUrl = req.QueryString["returnUrl"] ?? "/";

		var formData = (NameValueCollection?)options["req.form"] ?? [];
		var username = formData["username"] ?? "";
		var password = formData["password"] ?? "";

		var result = await userService.GetToken(username, password);

		if (!result.IsError)
		{
			string token = result.Payload!;
			HttpUtils.AddOptions(options, "redirect", "message", "User logged in successfully!");
			res.SetCookie(new Cookie("token", token, "/"));
			//res.AppendCookie(new Cookie("token", token, "/"));
			res.AddHeader("Authorization", $"Bearer {token!}");

			await HttpUtils.Redirect(req, res, options, $"{returnUrl}");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			HttpUtils.AddOptions(options, "redirect", "username", username);
			HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);

			await HttpUtils.Redirect(req, res, options, "/login");
		}
	}

	// POST /logout
	public async Task LogoutPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		res.AddHeader("Set-Cookie", "token=; Max-Age=0; Path=/");
		res.AddHeader("WWW-Authenticate", @"Bearer error=""invalid_token"", error_description=""The user logged out.""");

		HttpUtils.AddOptions(options, "redirect", "message", "User logged out successfully!");

		await HttpUtils.Redirect(req, res, options, "/login");
	}

	public async Task CheckAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string token = req.Headers["Authorization"]?.Substring(7) ?? req.Cookies["token"]?.Value ?? "";
		var result = await userService.ValidateToken(token);

		if (!result.IsError)
		{
			options["claims"] = result.Payload!;
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

			await HttpUtils.Redirect(req, res, options, "/login");
		}

		await next();
	}

	public async Task CheckAdmin(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string token = req.Headers["Authorization"]?.Substring(7) ?? req.Cookies["token"]?.Value ?? "";
		var result = await userService.ValidateToken(token);

		if (!result.IsError)
		{
			if (result.Payload!["role"] == Roles.ADMIN)
			{
				options["claims"] = result.Payload!;
			}
			else
			{
				HttpUtils.AddOptions(options, "redirect", "message", "Authenticated but not authorized. You must be an adminstrator to access this page.");

				await HttpUtils.Redirect(req, res, options, "/login");
			}
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

			await HttpUtils.Redirect(req, res, options, "/login");
		}

		await next();
	}
}
