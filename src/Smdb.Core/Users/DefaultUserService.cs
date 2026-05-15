namespace Smdb.Core.Users;

using System.Collections.Specialized;
using System.Text;
using System.Web;

using Abcs.Http;
using Smdb.Core.Shared;

public class DefaultUserService : IUserService
{
	private IUserRepository userRepository;

	public DefaultUserService(IUserRepository userRepository)
	{
		this.userRepository = userRepository;
		_ = Create(new User(0, "Admin", "masteroftheuniverse", "", Roles.ADMIN));
	}

	public async Task<Result<User>> Create(User newData)
	{
		if (string.IsNullOrWhiteSpace(newData.Role))
		{
			newData.Role = Roles.USER;
		}

		if (string.IsNullOrWhiteSpace(newData.Username))
		{
			return new Result<User>(new Exception("Username cannot be empty."));
		}
		else if (newData.Username.Length > 16)
		{
			return new Result<User>(new Exception("Username cannot have more than 16 characters."));
		}
		else if (await userRepository.GetUserByUsername(newData.Username) != null)
		{
			return new Result<User>(new Exception("Username already taken. Choose another username."));
		}

		if (string.IsNullOrWhiteSpace(newData.Password))
		{
			return new Result<User>(new Exception("Password cannot be empty."));
		}
		else if (newData.Password.Length < 16)
		{
			return new Result<User>(new Exception("Password cannot have less than 16 characters."));
		}

		if (!Roles.IsValid(newData.Role))
		{
			return new Result<User>(new Exception("Role is not valid."));
		}

		newData.Salt = Path.GetRandomFileName();
		newData.Password = Encode(newData.Password + newData.Salt);

		User? user = await userRepository.Create(newData);

		var result = (user == null) ?
			new Result<User>(new Exception("User could not be created.")) :
			new Result<User>(user);

		return result;
	}

	public async Task<Result<User>> Read(int id)
	{
		User? user = await userRepository.Read(id);

		var result = (user == null) ?
			new Result<User>(new Exception("User could not be read.")) :
			new Result<User>(user);

		return result;
	}

	public async Task<Result<User>> Update(int id, User updatedData)
	{
		if (string.IsNullOrWhiteSpace(updatedData.Role))
		{
			updatedData.Role = Roles.USER;
		}

		if (string.IsNullOrWhiteSpace(updatedData.Username))
		{
			return new Result<User>(new Exception("Username cannot be empty."));
		}
		else if (updatedData.Username.Length > 16)
		{
			return new Result<User>(new Exception("Username cannot have more than 16 characters."));
		}
		else
		{
			User? existingUser = await userRepository.GetUserByUsername(updatedData.Username);
			if (existingUser != null && existingUser.Id != id)
			{
				return new Result<User>(new Exception("Username already taken. Choose another username."));
			}
		}

		if (string.IsNullOrWhiteSpace(updatedData.Password))
		{
			return new Result<User>(new Exception("Password cannot be empty."));
		}
		else if (updatedData.Password.Length < 16)
		{
			return new Result<User>(new Exception("Password cannot have less than 16 characters."));
		}

		if (!Roles.IsValid(updatedData.Role))
		{
			return new Result<User>(new Exception("Role is not valid."));
		}

		updatedData.Salt = Path.GetRandomFileName();
		updatedData.Password = Encode(updatedData.Password + updatedData.Salt);

		User? user = await userRepository.Update(id, updatedData);

		var result = (user == null) ?
			new Result<User>(new Exception("User could not be updated.")) :
			new Result<User>(user);

		return result;
	}

	public async Task<Result<User>> Delete(int id)
	{
		User? user = await userRepository.Delete(id);

		var result = (user == null) ?
			new Result<User>(new Exception("User could not be deleted.")) :
			new Result<User>(user);

		return result;
	}

	public async Task<Result<PagedResult<User>>> List(int page, int size)
	{
		var pagedResult = await userRepository.List(page, size);

		var result = (pagedResult == null) ?
			new Result<PagedResult<User>>(new Exception("No results found.")) :
			new Result<PagedResult<User>>(pagedResult);

		return result;
	}

	public async Task<Result<string>> GetToken(string username, string password)
	{
		User? user = await userRepository.GetUserByUsername(username);

		if (user != null && string.Equals(user.Password, Encode(password + user.Salt)))
		{
			return new Result<string>(Encode($"username={user.Username}&role={user.Role}&expires={DateTime.Now.AddMinutes(60)}"));
		}
		else
		{
			return new Result<string>(new Exception("Invalid username or password."));
		}
	}

	public async Task<Result<NameValueCollection>> ValidateToken(string token)
	{
		if (!string.IsNullOrWhiteSpace(token))
		{
			NameValueCollection? claims = HttpUtility.ParseQueryString(Decode(token));

			// if(claims["expires"] < DateTime.Now) { // send null }
			return new Result<NameValueCollection>(claims);
		}
		else
		{
			var result = new Result<NameValueCollection>(new Exception("Invalid token."));
			return await Task.FromResult(result);
		}
	}

	public static string Encode(string plaintext)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext));
	}

	public static string Decode(string cyphertext)
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(cyphertext));
	}
}
