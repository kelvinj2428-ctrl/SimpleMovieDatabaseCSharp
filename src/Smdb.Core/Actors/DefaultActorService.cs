namespace Smdb.Core.Actors;

using Abcs.Http;

public class DefaultActorService : IActorService
{
	private IActorRepository actorRepository;

	public DefaultActorService(IActorRepository actorRepository)
	{
		this.actorRepository = actorRepository;
	}

	public async Task<Result<Actor>> Create(Actor newData)
	{
		if (string.IsNullOrWhiteSpace(newData.FirstName))
		{
			return new Result<Actor>(new Exception("First name cannot be empty."));
		}
		else if (newData.FirstName.Length > 16)
		{
			return new Result<Actor>(new Exception("First name cannot have more than 16 characters."));
		}
		else if (string.IsNullOrWhiteSpace(newData.LastName))
		{
			return new Result<Actor>(new Exception("Last name cannot be empty."));
		}
		else if (newData.LastName.Length > 16)
		{
			return new Result<Actor>(new Exception("Last name cannot have more than 16 characters."));
		}

		Actor? actor = await actorRepository.Create(newData);

		var result = (actor == null) ?
			new Result<Actor>(new Exception("Actor could not be created.")) :
			new Result<Actor>(actor);

		return await Task.FromResult(result);
	}

	public async Task<Result<Actor>> Read(int id)
	{
		Actor? actor = await actorRepository.Read(id);

		var result = (actor == null) ?
			new Result<Actor>(new Exception("Actor could not be read.")) :
			new Result<Actor>(actor);

		return await Task.FromResult(result);
	}

	public async Task<Result<Actor>> Update(int id, Actor updatedData)
	{
		if (string.IsNullOrWhiteSpace(updatedData.FirstName))
		{
			return new Result<Actor>(new Exception("First name cannot be empty."));
		}
		else if (updatedData.FirstName.Length > 16)
		{
			return new Result<Actor>(new Exception("First name cannot have more than 16 characters."));
		}
		else if (string.IsNullOrWhiteSpace(updatedData.LastName))
		{
			return new Result<Actor>(new Exception("Last name cannot be empty."));
		}
		else if (updatedData.LastName.Length > 16)
		{
			return new Result<Actor>(new Exception("Last name cannot have more than 16 characters."));
		}

		Actor? actor = await actorRepository.Update(id, updatedData);

		var result = (actor == null) ?
			new Result<Actor>(new Exception("Actor could not be updated.")) :
			new Result<Actor>(actor);

		return await Task.FromResult(result);
	}

	public async Task<Result<Actor>> Delete(int id)
	{
		Actor? actor = await actorRepository.Delete(id);

		var result = (actor == null) ?
			new Result<Actor>(new Exception("Actor could not be deleted.")) :
			new Result<Actor>(actor);

		return await Task.FromResult(result);
	}

	public async Task<Result<PagedResult<Actor>>> List(int page, int size)
	{
		var pagedResult = await actorRepository.List(page, size);

		var result = (pagedResult == null) ?
			new Result<PagedResult<Actor>>(new Exception("No results found.")) :
			new Result<PagedResult<Actor>>(pagedResult);

		return await Task.FromResult(result);
	}
}
