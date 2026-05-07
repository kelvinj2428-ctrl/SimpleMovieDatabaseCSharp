namespace Smdb.Core.Users;

using Abcs.Http;
using Smdb.Core.Shared;

public class MemoryUserRepository : IUserRepository
{
	private MemoryDatabase db;

	public MemoryUserRepository(MemoryDatabase db)
	{
		this.db = db;
	}

	public async Task<User?> Create(User newData)
	{
		newData.Id = db.NextUserId();
		db.Users.Add(newData);

		return await Task.FromResult(newData);
	}

	public async Task<User?> Read(int id)
	{
		User? user = db.Users.FirstOrDefault((u) => u.Id == id);

		return await Task.FromResult(user);
	}

	public async Task<User?> Update(int id, User updatedData)
	{
		User? user = db.Users.FirstOrDefault((u) => u.Id == id);

		if (user != null)
		{
			user.Username = updatedData.Username;
			user.Password = updatedData.Password;
			user.Salt = updatedData.Salt;
			user.Role = updatedData.Role;
		}

		return await Task.FromResult(user);
	}

	public async Task<User?> Delete(int id)
	{
		User? user = db.Users.FirstOrDefault((u) => u.Id == id);

		if (user != null)
		{
			db.Users.Remove(user);
		}

		return await Task.FromResult(user);
	}

	public async Task<PagedResult<User>> List(int page, int size)
	{
		int totalCount = db.Users.Count;
		int start = Math.Clamp((page - 1) * size, 0, totalCount);
		int length = Math.Clamp(size, 0, totalCount - start);
		List<User> values = db.Users.Slice(start, length);
		var pagedResult = new PagedResult<User>(totalCount, values);

		return await Task.FromResult(pagedResult);
	}

	public async Task<User?> GetUserByUsername(string username)
	{
		User? user = db.Users.FirstOrDefault((u) => u.Username == username);

		return await Task.FromResult(user);
	}
}
