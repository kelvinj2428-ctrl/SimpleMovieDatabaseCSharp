namespace Smdb.Core.Shared;

public class HtmlTemplates
{
	public static string Base(string title, string header, string content, string message = "")
	{
		return $@"
    <html>
      <head>
        <title>{title}</title>
        <link rel=""icon"" type=""image/x-icon"" href=""favicon.png"">
        <link rel=""stylesheet"" type=""text/css"" href=""/styles/main.css"">
        <script type=""text/javascript"" src=""/scripts/main.js"" defer></script>
      </head>
      <body>
        <h1 class=""header"">{header}</h1>
				<nav>
					<ul>
						<li><a href=""/"">Home</a></li>
						<li><a href=""/register"">Register</a></li>
						<li><a href=""/login"">Login</a></li>
						<li>
							<form action=""/logout"" method=""POST"">
								<input type=""submit"" value=""Logout"">
							</form>
						</li>
						<li><a href=""/actors"">Actors</a></li>
						<!-- <li><a href=""/actorsmovies"">Actors Movies</a></li>-->
						<li><a href=""/movies"">Movies</a></li>
						<li><a href=""/users"">Users</a></li>
					</ul>
				</nav>
        <div class=""content"">{content}</div>
        <div class=""message"">{message}</div>
      </body>
    </html>
    ";
	}
}
