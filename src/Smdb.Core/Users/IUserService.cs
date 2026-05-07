namespace Smdb.Core.Users;

using System.Collections.Specialized;

using Abcs.Http;

public interface IUserService
{
	public Task<Result<User>> Create(User newData);
	public Task<Result<User>> Read(int id);
	public Task<Result<User>> Update(int id, User updatedData);
	public Task<Result<User>> Delete(int id);
	public Task<Result<PagedResult<User>>> List(int page, int size);
	public Task<Result<string>> GetToken(string username, string password);
	public Task<Result<NameValueCollection>> ValidateToken(string token);
}
