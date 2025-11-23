namespace Smdb.Csr;

using Abcs.Config;
using Abcs.Http;
using System.Net;

public class App : HttpServer
{
	public App()
	{
	}

	public override void Init()
	{
		router.Use(HttpUtils.StructuredLogging);
		router.Use(HttpUtils.CentralizedErrorHandling);
		router.Use(HttpUtils.DefaultResponse);
		router.Use(HttpUtils.AddResponseCorsHeaders);
		router.Use(HttpUtils.ServeStaticFiles);
		router.UseSimpleRouteMatching();

		router.MapGet("/", async (req, res, props, next) => { res.Redirect("/index.html"); await next(); });
	}
}
