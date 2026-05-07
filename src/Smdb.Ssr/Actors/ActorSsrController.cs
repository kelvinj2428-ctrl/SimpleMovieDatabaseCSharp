namespace Smdb.Ssr.Actors;

using System.Collections;
using System.Collections.Specialized;
using System.Net;
using Abcs.Http;
using Smdb.Core.Actors;
using Smdb.Core.Shared;

public class ActorController
{
	private IActorService actorService;

	public ActorController(IActorService actorService)
	{
		this.actorService = actorService;
	}

	// GET /actors?page=1&size=5
	public async Task ViewAllActorsGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
		int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

		Result<PagedResult<Actor>> result = await actorService.List(page, size);

		if (!result.IsError)
		{
			PagedResult<Actor> pagedResult = result.Payload!;
			List<Actor> actors = pagedResult.Values;
			int actorCount = pagedResult.TotalCount;

			string html = ActorHtmlTemplates.ViewAllActorsGet(actors, actorCount, page, size);
			string content = HtmlTemplates.Base("SimpleMDB", "Actors View All Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/");
		}
	}

	// GET /actors/add
	public async Task AddActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string firstname = req.QueryString["firstname"] ?? "";
		string lastname = req.QueryString["lastname"] ?? "";
		string bio = req.QueryString["bio"] ?? "";
		string rating = req.QueryString["rating"] ?? "";
		string message = req.QueryString["message"] ?? "";

		string html = ActorHtmlTemplates.AddActorGet(firstname, lastname, bio, rating);
		string content = HtmlTemplates.Base("SimpleMDB", "Actors Add Page", html, message);

		await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
	}

	// POST /actors/add
	public async Task AddActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		var formData = (NameValueCollection?)options["req.form"] ?? [];

		string firstname = formData["firstname"] ?? "";
		string lastname = formData["lastname"] ?? "";
		string bio = formData["bio"] ?? "";
		float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

		Actor newActor = new Actor(0, firstname, lastname, bio, rating);

		Result<Actor> result = await actorService.Create(newActor);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "Actor added succesfully!");

			await HttpUtils.Redirect(req, res, options, "/actors"); // PRG
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			HttpUtils.AddOptions(options, "redirect", formData);

			await HttpUtils.Redirect(req, res, options, "/actors/add");
		}
	}

	// GET /actors/view?aid=1
	public async Task ViewActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

		Result<Actor> result = await actorService.Read(aid);

		if (!result.IsError)
		{
			Actor actor = result.Payload!;

			string html = ActorHtmlTemplates.ViewActorGet(actor);
			string content = HtmlTemplates.Base("SimpleMDB", "Actors View Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/actors");
		}
	}

	// GET /actors/edit?aid=1
	public async Task EditActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		string message = req.QueryString["message"] ?? "";

		int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

		Result<Actor> result = await actorService.Read(aid);

		if (!result.IsError)
		{
			Actor actor = result.Payload!;

			string html = ActorHtmlTemplates.EditActorGet(aid, actor);
			string content = HtmlTemplates.Base("SimpleMDB", "Actors Edit Page", html, message);

			await HttpUtils.SendResponse(req, res, options, (int)HttpStatusCode.OK, content);
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/actors");
		}
	}

	// POST /actors/edit?aid=1
	public async Task EditActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

		var formData = (NameValueCollection?)options["req.form"] ?? [];

		string firstname = formData["firstname"] ?? "";
		string lastname = formData["lastname"] ?? "";
		string bio = formData["bio"] ?? "";
		float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

		Actor newActor = new Actor(0, firstname, lastname, bio, rating);

		Result<Actor> result = await actorService.Update(aid, newActor);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "Actor edited succesfully!");
			await HttpUtils.Redirect(req, res, options, "/actors");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

			await HttpUtils.Redirect(req, res, options, $"/actors/edit?aid={aid}");
		}
	}

	// POST /actors/remove?aid=1
	public async Task RemoveActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, Func<Task> next)
	{
		int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

		Result<Actor> result = await actorService.Delete(aid);

		if (!result.IsError)
		{
			HttpUtils.AddOptions(options, "redirect", "message", "Actor removed succesfully!");
			await HttpUtils.Redirect(req, res, options, "/actors");
		}
		else
		{
			HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
			await HttpUtils.Redirect(req, res, options, "/actors");
		}
	}
}
