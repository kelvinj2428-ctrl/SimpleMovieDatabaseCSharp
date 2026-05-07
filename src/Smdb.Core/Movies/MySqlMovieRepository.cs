namespace Smdb.Core.Movies;

using System.Data;
using MySql.Data.MySqlClient;
using Abcs.Http;

public class MySqlMovieRepository : IMovieRepository
{
	private string connectionString;

	public MySqlMovieRepository(string connectionString)
	{
		this.connectionString = connectionString;
		//Init();
	}

	private void Init()
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();

		cmd.CommandText = @"
    CREATE TABLE IF NOT EXISTS Movies
    (
      id int AUTO_INCREMENT PRIMARY KEY,
      title NVARCHAR(256) NOT NULL,
      year int NOT NULL,
      description NVARCHAR(4096),
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

	public async Task<Movie?> Create(Movie newData)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      INSERT INTO Movies (title, year, description, rating)
        VALUES(@title, @year, @description, @rating);
      SELECT LAST_INSERT_ID();";
		cmd.Parameters.AddWithValue("@title", newData.Title);
		cmd.Parameters.AddWithValue("@year", newData.Year);
		cmd.Parameters.AddWithValue("@description", newData.Description);
		cmd.Parameters.AddWithValue("@rating", newData.Rating);

		newData.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

		return newData;
	}

	public async Task<Movie?> Read(int id)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "SELECT * FROM Movies WHERE id = @id";
		cmd.Parameters.AddWithValue("@id", id);

		using var rows = await cmd.ExecuteReaderAsync();

		if (await rows.ReadAsync())
		{
			return new Movie
			{
				Id = rows.GetInt32("id"),
				Title = rows.GetString("title"),
				Year = rows.GetInt32("year"),
				Description = rows.GetString("description"),
				Rating = rows.GetFloat("rating")
			};
		}

		return null;
	}

	public async Task<Movie?> Update(int id, Movie updatedData)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
    UPDATE Movies SET
      title = @title,
      year = @year,
      description = @description,
      rating = @rating
    WHERE id = @id";

		cmd.Parameters.AddWithValue("@id", id);
		cmd.Parameters.AddWithValue("@title", updatedData.Title);
		cmd.Parameters.AddWithValue("@year", updatedData.Year);
		cmd.Parameters.AddWithValue("@description", updatedData.Description);
		cmd.Parameters.AddWithValue("@rating", updatedData.Rating);

		return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? updatedData : null;
	}

	public async Task<Movie?> Delete(int id)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "DELETE FROM Movies WHERE id = @id";
		cmd.Parameters.AddWithValue("@id", id);

		Movie? movie = await Read(id);

		return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? movie : null;
	}

	public async Task<PagedResult<Movie>> List(int page, int size)
	{
		using var dbc = OpenDb();

		using var countCmd = dbc.CreateCommand();

		countCmd.CommandText = "SELECT COUNT(*) FROM Movies";
		int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "SELECT * FROM Movies LIMIT @offset, @limit";
		cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
		cmd.Parameters.AddWithValue("@limit", size);

		using var rows = await cmd.ExecuteReaderAsync();
		var movies = new List<Movie>();

		while (await rows.ReadAsync())
		{
			movies.Add(new Movie
			{
				Id = rows.GetInt32("id"),
				Title = rows.GetString("title"),
				Year = rows.GetInt32("year"),
				Description = rows.GetString("description"),
				Rating = rows.GetFloat("rating")
			});
		}

		return new PagedResult<Movie>(totalCount, movies);
	}
}
