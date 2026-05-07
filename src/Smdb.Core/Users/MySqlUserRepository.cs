namespace Smdb.Core.Users;

using System.Data;
using MySql.Data.MySqlClient;
using Abcs.Http;

public class MySqlUserRepository : IUserRepository
{
	private string connectionString;

	public MySqlUserRepository(string connectionString)
	{
		this.connectionString = connectionString;
		//Init();
	}

	private void Init()
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();

		cmd.CommandText = @"
    CREATE TABLE IF NOT EXISTS Users
    (
      id int AUTO_INCREMENT PRIMARY KEY,
      username NVARCHAR(64) NOT NULL UNIQUE,
      password NVARCHAR(64) NOT NULL,
      salt NVARCHAR(64) NOT NULL,
      role ENUM('Admin', 'User') NOT NULL
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

	public async Task<User?> Create(User newData)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
      INSERT INTO Users (username, password, salt, role)
        VALUES(@username, @password, @salt, @role);
      SELECT LAST_INSERT_ID();";
		cmd.Parameters.AddWithValue("@username", newData.Username);
		cmd.Parameters.AddWithValue("@password", newData.Password);
		cmd.Parameters.AddWithValue("@salt", newData.Salt);
		cmd.Parameters.AddWithValue("@role", newData.Role);

		newData.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

		return newData;
	}

	public async Task<User?> Read(int id)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "SELECT * FROM Users WHERE id = @id";
		cmd.Parameters.AddWithValue("@id", id);

		using var rows = await cmd.ExecuteReaderAsync();

		if (await rows.ReadAsync())
		{
			return new User
			{
				Id = rows.GetInt32("id"),
				Username = rows.GetString("username"),
				Password = rows.GetString("password"),
				Salt = rows.GetString("salt"),
				Role = rows.GetString("role")
			};
		}

		return null;
	}

	public async Task<User?> Update(int id, User updatedData)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = @"
    UPDATE Users SET
      username = @username,
      password = @password,
      salt = @salt,
      role = @role
    WHERE id = @id";

		cmd.Parameters.AddWithValue("@id", id);
		cmd.Parameters.AddWithValue("@username", updatedData.Username);
		cmd.Parameters.AddWithValue("@password", updatedData.Password);
		cmd.Parameters.AddWithValue("@salt", updatedData.Salt);
		cmd.Parameters.AddWithValue("@role", updatedData.Role);

		return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? updatedData : null;
	}

	public async Task<User?> Delete(int id)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "DELETE FROM Users WHERE id = @id";
		cmd.Parameters.AddWithValue("@id", id);

		User? User = await Read(id);

		return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? User : null;
	}

	public async Task<PagedResult<User>> List(int page, int size)
	{
		using var dbc = OpenDb();

		using var countCmd = dbc.CreateCommand();

		countCmd.CommandText = "SELECT COUNT(*) FROM Users";
		int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "SELECT * FROM Users LIMIT @offset, @limit";
		cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
		cmd.Parameters.AddWithValue("@limit", size);

		using var rows = await cmd.ExecuteReaderAsync();
		var users = new List<User>();

		while (await rows.ReadAsync())
		{
			users.Add(new User
			{
				Id = rows.GetInt32("id"),
				Username = rows.GetString("username"),
				Password = rows.GetString("password"),
				Salt = rows.GetString("salt"),
				Role = rows.GetString("role")
			});
		}

		return new PagedResult<User>(totalCount, users);
	}

	public async Task<User?> GetUserByUsername(string username)
	{
		using var dbc = OpenDb();

		using var cmd = dbc.CreateCommand();
		cmd.CommandText = "SELECT * FROM Users WHERE username = @username";
		cmd.Parameters.AddWithValue("@username", username);

		using var rows = await cmd.ExecuteReaderAsync();

		if (await rows.ReadAsync())
		{
			return new User
			{
				Id = rows.GetInt32("id"),
				Username = rows.GetString("username"),
				Password = rows.GetString("password"),
				Salt = rows.GetString("salt"),
				Role = rows.GetString("role")
			};
		}

		return null;
	}
}
