namespace Smdb.Core.Actors;

using Abcs.Http;

public interface IActorService
{
	public Task<Result<Actor>> Create(Actor newData);
	public Task<Result<Actor>> Read(int id);
	public Task<Result<Actor>> Update(int id, Actor updatedData);
	public Task<Result<Actor>> Delete(int id);
	public Task<Result<PagedResult<Actor>>> List(int page, int size);
}
