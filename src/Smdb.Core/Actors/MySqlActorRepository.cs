namespace Smdb.Core.Actors;

using Abcs.Http;

using System.Data;
using MySql.Data.MySqlClient;

public class MySqlActorRepository : IActorRepository
{
	private string connectionString;

	public MySqlActorRepository(string connectionString)
	{
		this.connectionString = connectionString;
		//Init();
	}

	private void Init()
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();

		cmd.CommandText = @"
    CREATE TABLE IF NOT EXISTS Actors
    (
      id int AUTO_INCREMENT PRIMARY KEY,
      firstname NVARCHAR(64) NOT NULL,
      lastname NVARCHAR(64) NOT NULL,
      bio NVARCHAR(4096),
      rating float
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

	public async Task<Actor?> Create(Actor newData)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      INSERT INTO Actors (firstname, lastname, bio, rating)
        VALUES(@firstname, @lastname, @bio, @rating);
      SELECT LAST_INSERT_ID();";
		cmd.Parameters.AddWithValue("@firstname", newData.FirstName);
		cmd.Parameters.AddWithValue("@lastname", newData.LastName);
		cmd.Parameters.AddWithValue("@bio", newData.Bio);
		cmd.Parameters.AddWithValue("@rating", newData.Rating);

		newData.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

		return newData;
	}

	public async Task<Actor?> Read(int id)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "SELECT * FROM Actors WHERE id = @id";
		cmd.Parameters.AddWithValue("@id", id);

		using var rows = await cmd.ExecuteReaderAsync();

		if (await rows.ReadAsync())
		{
			return new Actor
			{
				Id = rows.GetInt32("id"),
				FirstName = rows.GetString("firstname"),
				LastName = rows.GetString("lastname"),
				Bio = rows.GetString("bio"),
				Rating = rows.GetFloat("rating")
			};
		}

		return null;
	}

	public async Task<Actor?> Update(int id, Actor updatedData)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
    UPDATE Actors SET
      firstname = @firstname,
      lastname = @lastname,
      bio = @bio,
      rating = @rating
    WHERE id = @id";

		cmd.Parameters.AddWithValue("@id", id);
		cmd.Parameters.AddWithValue("@firstname", updatedData.FirstName);
		cmd.Parameters.AddWithValue("@lastname", updatedData.LastName);
		cmd.Parameters.AddWithValue("@bio", updatedData.Bio);
		cmd.Parameters.AddWithValue("@rating", updatedData.Rating);

		return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? updatedData : null;
	}

	public async Task<Actor?> Delete(int id)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "DELETE FROM Actors WHERE id = @id";
		cmd.Parameters.AddWithValue("@id", id);

		Actor? actor = await Read(id);

		return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? actor : null;
	}

	public async Task<PagedResult<Actor>> List(int page, int size)
	{
		using var dbc = OpenDb();

		using var countCmd = dbc.CreateCommand();

		countCmd.CommandText = "SELECT COUNT(*) FROM Actors";
		int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "SELECT * FROM Actors LIMIT @offset, @limit";
		cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
		cmd.Parameters.AddWithValue("@limit", size);

		using var rows = await cmd.ExecuteReaderAsync();
		var actors = new List<Actor>();

		while (await rows.ReadAsync())
		{
			actors.Add(new Actor
			{
				Id = rows.GetInt32("id"),
				FirstName = rows.GetString("firstname"),
				LastName = rows.GetString("lastname"),
				Bio = rows.GetString("bio"),
				Rating = rows.GetFloat("rating")
			});
		}

		return new PagedResult<Actor>(totalCount, actors);
	}
}
