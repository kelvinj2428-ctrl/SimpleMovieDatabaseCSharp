namespace Smdb.Core.Users;

using Abcs.Http;

public interface IUserRepository
{
	public Task<User?> Create(User newData);
	public Task<User?> Read(int id);
	public Task<User?> Update(int id, User updatedData);
	public Task<User?> Delete(int id);
	public Task<PagedResult<User>> List(int page, int size);
	public Task<User?> GetUserByUsername(string username);
}
