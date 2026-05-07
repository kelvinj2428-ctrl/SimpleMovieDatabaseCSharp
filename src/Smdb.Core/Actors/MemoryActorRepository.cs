namespace Smdb.Core.Actors;

using Abcs.Http;
using Smdb.Core.Shared;

public class MemoryActorRepository : IActorRepository
{
	private MemoryDatabase db;

	public MemoryActorRepository(MemoryDatabase db)
	{
		this.db = db;
	}

	public async Task<Actor?> Create(Actor newData)
	{
		newData.Id = db.NextActorId();
		db.Actors.Add(newData);

		return await Task.FromResult(newData);
	}

	public async Task<Actor?> Read(int id)
	{
		Actor? actor = db.Actors.FirstOrDefault((u) => u.Id == id);

		return await Task.FromResult(actor);
	}

	public async Task<Actor?> Update(int id, Actor updatedData)
	{
		Actor? actor = db.Actors.FirstOrDefault((u) => u.Id == id);

		if (actor != null)
		{
			actor.FirstName = updatedData.FirstName;
			actor.LastName = updatedData.LastName;
			actor.Bio = updatedData.Bio;
			actor.Rating = updatedData.Rating;
		}

		return await Task.FromResult(actor);
	}

	public async Task<Actor?> Delete(int id)
	{
		Actor? actor = db.Actors.FirstOrDefault((u) => u.Id == id);

		if (actor != null)
		{
			db.Actors.Remove(actor);
		}

		return await Task.FromResult(actor);
	}

	public async Task<PagedResult<Actor>> List(int page, int size)
	{
		int totalCount = db.Actors.Count;
		int start = Math.Clamp((page - 1) * size, 0, totalCount);
		int length = Math.Clamp(size, 0, totalCount - start);
		List<Actor> values = db.Actors.Slice(start, length);
		var pagedResult = new PagedResult<Actor>(totalCount, values);

		return await Task.FromResult(pagedResult);
	}
}
