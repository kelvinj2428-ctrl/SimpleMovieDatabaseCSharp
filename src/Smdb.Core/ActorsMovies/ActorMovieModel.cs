namespace Smdb.Core.ActorsMovies;

public class ActorMovie
{
	public int Id { get; set; }
	public int ActorId { get; set; }
	public int MovieId { get; set; }
	public string RoleName { get; set; }

	public ActorMovie(int id = -1, int actorId = -1, int movieId = -1, string roleName = "")
	{
		Id = id;
		ActorId = actorId;
		MovieId = movieId;
		RoleName = roleName;
	}

	public override string ToString()
	{
		return $"ActorMovie[Id={Id}, ActorId={ActorId}, MovieId={MovieId}, RoleName={RoleName}]";
	}
}
