namespace Smdb.Core.ActorsMovies;

using Abcs.Http;
using Smdb.Core.Actors;
using Smdb.Core.Movies;

using System.Data;
using MySql.Data.MySqlClient;

public class MySqlActorMovieRepository : IActorMovieRepository
{
	private string connectionString;

	public MySqlActorMovieRepository(string connectionString)
	{
		this.connectionString = connectionString;
		//Init();
	}

	private void Init()
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();

		cmd.CommandText = @"
    CREATE TABLE IF NOT EXISTS ActorMovies
    (
      id int AUTO_INCREMENT PRIMARY KEY,
      actorId int NOT NULL,
      movieId int NOT NULL,
      rolename NVARCHAR(64),
      FOREIGN KEY(actorId) REFERENCES Actors(id) ON DELETE CASCADE,
      FOREIGN KEY(movieId) REFERENCES Movies(id) ON DELETE CASCADE
    )
    ";

		cmd.ExecuteNonQuery();
	}

	public MySqlConnection OpenDb()
	{
		var dbc = new MySqlConnection(connectionString);
		dbc.Open();
		return dbc;
	}

	public async Task<ActorMovie?> Create(ActorMovie newData)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      INSERT INTO ActorsMovies (actorId, movieId, rolename)
      VALUES (@actorId, @movieId, @rolename);
      SELECT LAST_INSERT_ID();
      ";
		cmd.Parameters.AddWithValue("@actorId", newData.ActorId);
		cmd.Parameters.AddWithValue("@movieId", newData.MovieId);
		cmd.Parameters.AddWithValue("@rolename", newData.RoleName);

		int id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
		var actorMovie = new ActorMovie(id, newData.ActorId, newData.MovieId, newData.RoleName);

		return actorMovie;
	}

	public async Task<ActorMovie?> Read(int id)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      SELECT * FROM ActorsMovies WHERE id = @id
      ";
		cmd.Parameters.AddWithValue("@id", id);

		using var rows = await cmd.ExecuteReaderAsync();

		if (await rows.ReadAsync())
		{
			return new ActorMovie(
				rows.GetInt32(0), // id
				rows.GetInt32(1), // actorId
				rows.GetInt32(2), // movieId
				rows.GetString(3) // roleName
			);
		}

		return null;
	}

	public async Task<ActorMovie?> Update(int id, ActorMovie updatedData)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
    UPDATE ActorsMovies SET
      actorId = @actorId,
      movieId = @movieId,
      roleName = @roleName,
    WHERE id = @id";

		cmd.Parameters.AddWithValue("@id", id);
		cmd.Parameters.AddWithValue("@actorId", updatedData.ActorId);
		cmd.Parameters.AddWithValue("@movieId", updatedData.MovieId);
		cmd.Parameters.AddWithValue("@roleName", updatedData.RoleName);

		return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? updatedData : null;
	}

	public async Task<ActorMovie?> Delete(int id)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      DELETE FROM ActorsMovies WHERE id = @id;
      ";
		cmd.Parameters.AddWithValue("@id", id);

		var actorMovie = await Read(id);

		return (await cmd.ExecuteNonQueryAsync() > 0) ? actorMovie : null;
	}

	public async Task<PagedResult<ActorMovie>> List(int page, int size)
	{
		using var dbc = OpenDb();

		using var countCmd = dbc.CreateCommand();

		countCmd.CommandText = "SELECT COUNT(*) FROM ActorsMovies";
		int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "SELECT * FROM ActorsMovies LIMIT @offset, @limit";
		cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
		cmd.Parameters.AddWithValue("@limit", size);

		using var rows = await cmd.ExecuteReaderAsync();
		var actorsMovies = new List<ActorMovie>();

		while (await rows.ReadAsync())
		{
			actorsMovies.Add(new ActorMovie
			{
				Id = rows.GetInt32("id"),
				ActorId = rows.GetInt32("actorId"),
				MovieId = rows.GetInt32("movieId"),
				RoleName = rows.GetString("roleName"),
			});
		}

		return new PagedResult<ActorMovie>(totalCount, actorsMovies);
	}

	public async Task<PagedResult<(ActorMovie, Movie)>> ListMoviesByActor(int actorId, int page, int size)
	{
		using var dbc = OpenDb();

		using var countCmd = dbc.CreateCommand();
		countCmd.CommandText = @"
      SELECT COUNT(*) FROM ActorsMovies as am
      JOIN Movies as m ON am.movieId = m.id
      WHERE am.actorId = @actorId
    ";
		countCmd.Parameters.AddWithValue("@actorId", actorId);

		int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      SELECT * FROM ActorsMovies as am
      JOIN Movies as m ON am.movieId = m.id
      WHERE am.actorId = @actorId
      LIMIT @offset, @limit
      ";
		cmd.Parameters.AddWithValue("@actorId", actorId);
		cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
		cmd.Parameters.AddWithValue("@limit", size);

		using var rows = await cmd.ExecuteReaderAsync();

		var amms = new List<(ActorMovie, Movie)>();

		while (await rows.ReadAsync())
		{
			ActorMovie am = new ActorMovie(
				rows.GetInt32(0),   // id
				rows.GetInt32(1),   // actorId
				rows.GetInt32(2),   // movieId
				rows.GetString(3)); // rolename

			Movie m = new Movie(
				rows.GetInt32(4),   // id
				rows.GetString(5),  // title
				rows.GetInt32(6),   // year
				rows.GetString(7),  // description
				rows.GetFloat(8)    // rating
			);

			amms.Add((am, m));
		}

		return new PagedResult<(ActorMovie, Movie)>(totalCount, amms);
	}

	public async Task<PagedResult<(ActorMovie, Actor)>> ListActorsByMovie(int movieId, int page, int size)
	{
		using var dbc = OpenDb();

		using var countCmd = dbc.CreateCommand();
		countCmd.CommandText = @"
      SELECT COUNT(*) FROM ActorsMovies as am
      JOIN Actors as a ON am.actorId = a.id
      WHERE am.movieId = @movieId
    ";
		countCmd.Parameters.AddWithValue("@movieId", movieId);

		int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      SELECT * FROM ActorsMovies as am
      JOIN Actors as a ON am.actorId = a.id
      WHERE am.movieId = @movieId
      LIMIT @offset, @limit
      ";
		cmd.Parameters.AddWithValue("@movieId", movieId);
		cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
		cmd.Parameters.AddWithValue("@limit", size);

		using var rows = await cmd.ExecuteReaderAsync();

		var amas = new List<(ActorMovie, Actor)>();

		while (await rows.ReadAsync())
		{
			ActorMovie am = new ActorMovie(
				rows.GetInt32(0),   // id
				rows.GetInt32(1),   // actorId
				rows.GetInt32(2),   // movieId
				rows.GetString(3)); // rolename

			Actor a = new Actor(
				rows.GetInt32(4),   // id
				rows.GetString(5),  // firstname
				rows.GetString(6),  // lastname
				rows.GetString(7),  // bio
				rows.GetFloat(8)    // rating
			);

			amas.Add((am, a));
		}

		return new PagedResult<(ActorMovie, Actor)>(totalCount, amas);
	}

	public async Task<List<Actor>> ListActors()
	{
		using var dbc = OpenDb();

		using var countCmd = dbc.CreateCommand();
		countCmd.CommandText = @"
      SELECT COUNT(*) FROM Actors
      ";

		int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      SELECT * FROM Actors
      ";

		using var rows = await cmd.ExecuteReaderAsync();

		var actors = new List<Actor>();

		while (await rows.ReadAsync())
		{
			Actor actor = new Actor(
				rows.GetInt32(0),   // id
				rows.GetString(1),  // firstname
				rows.GetString(2),  // lastname
				rows.GetString(3),  // bio
				rows.GetFloat(4)    // rating
			);

			actors.Add(actor);
		}

		return actors;
	}

	public async Task<List<Movie>> ListMovies()
	{
		using var dbc = OpenDb();

		using var countCmd = dbc.CreateCommand();
		countCmd.CommandText = @"
      SELECT COUNT(*) FROM Movies
      ";

		int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      SELECT * FROM Movies
      ";

		using var rows = await cmd.ExecuteReaderAsync();

		var movies = new List<Movie>();

		while (await rows.ReadAsync())
		{
			Movie movie = new Movie(
				rows.GetInt32(0),   // id
				rows.GetString(1),  // title
				rows.GetInt32(2),   // year
				rows.GetString(3),  // description
				rows.GetFloat(4)    // rating
			);

			movies.Add(movie);
		}

		return movies;
	}
}
