namespace Smdb.Core.Actors;

using Abcs.Http;

public interface IActorRepository
{
	public Task<Actor?> Create(Actor newData);
	public Task<Actor?> Read(int id);
	public Task<Actor?> Update(int id, Actor updatedData);
	public Task<Actor?> Delete(int id);
	public Task<PagedResult<Actor>> List(int page, int size);
}
