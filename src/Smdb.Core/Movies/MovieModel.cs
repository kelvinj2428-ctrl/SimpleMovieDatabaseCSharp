namespace Smdb.Core.Movies;

public class Movie
{
	public int Id { get; set; }
	public string Title { get; set; }
	public int Year { get; set; }
	public string Description { get; set; }
	public float Rating { get; set; }

	public Movie(int id = -1, string title = "", int year = 1888, string description = "", float rating = 5F)
	{
		Id = id;
		Title = title;
		Year = year;
		Description = description;
		Rating = rating;
	}

	public override string ToString()
	{
		return $"Movie[Id={Id}, Title={Title}, Year={Year}, Description={Description}, Rating={Rating}]";
	}
}
