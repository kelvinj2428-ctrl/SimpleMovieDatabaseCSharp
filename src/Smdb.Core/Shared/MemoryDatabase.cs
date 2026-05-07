namespace Smdb.Core.Shared;

using Smdb.Core.Actors;
using Smdb.Core.ActorsMovies;
using Smdb.Core.Movies;
using Smdb.Core.Users;

public class MemoryDatabase
{
	public List<Actor> Actors { get; }
	public List<ActorMovie> ActorsMovies { get; }
	public List<Movie> Movies { get; }
	public List<User> Users { get; }

	private int nextActorId;
	private int nextActorMovieId;
	private int nextMovieId;
	private int nextUserId;

	public MemoryDatabase()
	{
		Actors = [];
		ActorsMovies = [];
		Movies = [];
		Users = [];

		nextActorId = Actors.Count;
		nextActorMovieId = ActorsMovies.Count;
		nextMovieId = Movies.Count;
		nextUserId = Users.Count;

		SeedActors();
		SeedActorsMovies();
		SeedMovies();
		SeedUsers();
	}

	private void SeedActors()
	{
		// Pre‐populated arrays of 100 real‐sounding first and last names
		var firstNames = new[]
		{
			"James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
			"William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
			"Thomas", "Sarah", "Charles", "Karen", "Christopher", "Nancy", "Daniel", "Lisa",
			"Matthew", "Margaret", "Anthony", "Betty", "Donald", "Sandra", "Mark", "Ashley",
			"Paul", "Dorothy", "Steven", "Kimberly", "Andrew", "Emily", "Kenneth", "Donna",
			"George", "Michelle", "Joshua", "Carol", "Kevin", "Amanda", "Brian", "Melissa",
			"Edward", "Deborah", "Ronald", "Stephanie", "Timothy", "Rebecca", "Jason", "Laura",
			"Jeffrey", "Sharon", "Ryan", "Cynthia", "Jacob", "Kathleen", "Gary", "Helen",
			"Nicholas", "Amy", "Eric", "Shirley", "Stephen", "Angela", "Jonathan", "Anna",
			"Larry", "Brenda", "Justin", "Pamela", "Scott", "Nicole", "Brandon", "Emma",
			"Benjamin", "Samantha", "Samuel", "Katherine", "Gregory", "Christine", "Frank",
			"Debra", "Alexander", "Rachel", "Raymond", "Catherine", "Patrick", "Carolyn",
			"Jack", "Janet", "Dennis", "Ruth", "Peter", "Heather"
		};

		var lastNames = new[]
		{
			"Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
			"Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas",
			"Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White",
			"Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young",
			"Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
			"Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell",
			"Carter", "Roberts", "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker",
			"Cruz", "Edwards", "Collins", "Reyes", "Stewart", "Morris", "Morales", "Murphy",
			"Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper", "Peterson", "Bailey",
			"Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
			"Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza",
			"Ruiz", "Hughes", "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers",
			"Long", "Ross", "Foster", "Jimenez"
		};

		// A small palette of “career highlights” to vary each bio
		var highlights = new[]
		{
			"drama and action films",
			"comedies and musicals",
			"independent art‐house features",
			"television drama series",
			"blockbuster sci‐fi movies",
			"thriller and horror genres",
			"historical biopics",
			"romantic comedies",
			"animated feature films",
			"stage and screen productions"
		};

		var random = new Random();

		for (int i = 0; i < firstNames.Length; i++)
		{
			// generate a rating between 0.0 and 10.0, rounded to one decimal place
			float rating = (float)Math.Round(random.NextDouble() * 10, 1);

			// pick one of the highlights to personalize the bio
			string career = highlights[i % highlights.Length];

			// build a short bio
			string bio = $"{firstNames[i]} {lastNames[i]} is an actor known for their work in {career}. " +
									 "Over their career, they have garnered critical acclaim and a devoted fan base.";

			// create and add the actor
			Actors.Add(new Actor(nextActorId++, firstNames[i], lastNames[i], bio, rating));
		}
	}

	private void SeedActorsMovies()
	{
		string[] movieRoles = new string[]
		{
			"Protagonist", "Antagonist", "Hero", "Villain", "Sidekick", "Love Interest",
			"Mentor", "Comic Relief", "Detective", "Police Officer", "Spy", "Soldier",
			"General", "King", "Queen", "Prince", "Princess", "Knight", "Wizard",
			"Sorcerer", "Witch", "Alien", "Robot", "Scientist", "Doctor", "Nurse",
			"Lawyer", "Judge", "Criminal", "Thief", "Assassin", "Hitman",
			"Journalist", "Photographer", "Teacher", "Student", "Athlete",
			"Coach", "Musician", "Singer", "Actor", "Director", "Producer",
			"Businessman", "CEO", "Entrepreneur", "Farmer", "Cowboy", "Outlaw",
			"Pirate", "Sailor", "Captain", "Pilot", "Engineer", "Mechanic",
			"Driver", "Taxi Driver", "Truck Driver", "Firefighter", "Paramedic",
			"Security Guard", "Bodyguard", "Bounty Hunter", "Explorer", "Archaeologist",
			"Historian", "Librarian", "Chef", "Waiter", "Bartender", "Butler",
			"Maid", "Housekeeper", "Nanny", "Parent", "Mother", "Father",
			"Sibling", "Brother", "Sister", "Child", "Teenager", "Elder",
			"Ghost", "Vampire", "Werewolf", "Zombie", "Monster", "Superhero",
			"Supervillain", "Time Traveler", "Clone", "AI Companion", "Rebel",
			"Leader", "Follower", "Survivor", "Victim"
		};

		Random r = new Random();

		for (int aid = 0; aid < 100; aid++)
		{
			int count = r.Next(100);
			for (int j = 0; j < count; j++)
			{
				int mid = r.Next(100);
				ActorsMovies.Add(new ActorMovie(nextActorMovieId++, aid, mid, movieRoles[r.Next(movieRoles.Length)]));
			}
		}
	}

	private void SeedMovies()
	{
		Movies.AddRange(new Movie[]
		{
			new Movie(1, "The Godfather", 1972, "A mafia patriarch hands the family empire to his reluctant son."),
			new Movie(2, "The Godfather Part II", 1974, "Michael consolidates power as flashbacks trace Vito Corleone’s rise."),
			new Movie(3, "The Dark Knight", 2008, "Batman faces the Joker, who pushes Gotham into chaos."),
			new Movie(4, "The Shawshank Redemption", 1994, "An innocent banker forms a life-saving friendship in prison."),
			new Movie(5, "Pulp Fiction", 1994, "Interlocking LA crime stories unfold with dark humor."),
			new Movie(6, "Schindler's List", 1993, "A businessman saves Jewish workers during the Holocaust."),
			new Movie(7, "The Lord of the Rings: The Return of the King", 2003, "The final push to destroy the One Ring decides Middle-earth’s fate."),
			new Movie(8, "Fight Club", 1999, "An insomnia-plagued worker joins a charismatic anarchist’s secret club."),
			new Movie(9, "Forrest Gump", 1994, "A kind man unwittingly drifts through historic American moments."),
			new Movie(10, "Inception", 2010, "A thief enters dreams to plant an idea in a target’s mind."),
			new Movie(11, "The Matrix", 1999, "A hacker learns reality is a simulated prison for humanity."),
			new Movie(12, "Se7en", 1995, "Two detectives hunt a killer using the seven deadly sins."),
			new Movie(13, "Goodfellas", 1990, "Henry Hill’s rise and fall inside the New York mob."),
			new Movie(14, "The Silence of the Lambs", 1991, "An FBI trainee consults Hannibal Lecter to catch a serial killer."),
			new Movie(15, "Star Wars: Episode IV – A New Hope", 1977, "A farm boy joins rebels to destroy the Empire’s Death Star."),
			new Movie(16, "The Empire Strikes Back", 1980, "The Rebels scatter as Luke confronts Darth Vader."),
			new Movie(17, "Interstellar", 2014, "Astronauts travel through a wormhole to save a dying Earth."),
			new Movie(18, "Parasite", 2019, "A poor family infiltrates a wealthy household with unforeseen fallout."),
			new Movie(19, "Spirited Away", 2001, "A girl navigates a spirit bathhouse to free her parents."),
			new Movie(20, "City of God", 2002, "Two boys take diverging paths amid Rio’s gang wars."),
			new Movie(21, "Saving Private Ryan", 1998, "A squad risks everything to bring a paratrooper home."),
			new Movie(22, "The Green Mile", 1999, "Death-row guards encounter a prisoner with miraculous gifts."),
			new Movie(23, "Gladiator", 2000, "A betrayed general becomes Rome’s fiercest arena fighter."),
			new Movie(24, "The Lion King", 1994, "An exiled lion cub returns to claim his destiny."),
			new Movie(25, "Back to the Future", 1985, "A teen time-travels and risks erasing his own existence."),
			new Movie(26, "The Departed", 2006, "An infiltrator and a mole play cat-and-mouse in Boston."),
			new Movie(27, "Whiplash", 2014, "A jazz drummer endures a brutal mentor in pursuit of greatness."),
			new Movie(28, "The Prestige", 2006, "Rival magicians wage a dangerous war of one-upmanship."),
			new Movie(29, "The Usual Suspects", 1995, "A survivors’ tale hints at the legend of Keyser Söze."),
			new Movie(30, "Terminator 2: Judgment Day", 1991, "A reprogrammed cyborg protects the future leader of humanity."),
			new Movie(31, "Alien", 1979, "A crew is stalked by a lethal lifeform aboard a spaceship."),
			new Movie(32, "Aliens", 1986, "Ripley returns to face a hive of xenomorphs on LV-426."),
			new Movie(33, "Blade Runner", 1982, "A detective hunts rogue androids in a neon-soaked future."),
			new Movie(34, "Apocalypse Now", 1979, "A captain journeys upriver to terminate a renegade officer."),
			new Movie(35, "One Flew Over the Cuckoo's Nest", 1975, "A rebel patient challenges a tyrannical nurse in a psych ward."),
			new Movie(36, "Taxi Driver", 1976, "A disturbed NYC cabbie spirals toward violence."),
			new Movie(37, "Oldboy", 2003, "A man seeks answers after 15 years of inexplicable captivity."),
			new Movie(38, "Amélie", 2001, "A shy Parisian decides to secretly improve others’ lives."),
			new Movie(39, "The Pianist", 2002, "A Jewish pianist struggles to survive Warsaw’s ghetto."),
			new Movie(40, "American Beauty", 1999, "A suburban man’s midlife crisis upends his family."),
			new Movie(41, "No Country for Old Men", 2007, "A stolen briefcase triggers relentless pursuit across Texas."),
			new Movie(42, "There Will Be Blood", 2007, "An oilman’s ambition consumes everything around him."),
			new Movie(43, "Mad Max: Fury Road", 2015, "A desert chase pits a warlord against a defiant road warrior."),
			new Movie(44, "La La Land", 2016, "A musician and an actress chase dreams in modern LA."),
			new Movie(45, "Joker", 2019, "A marginalized comedian’s breakdown sparks violent unrest."),
			new Movie(46, "Avengers: Infinity War", 2018, "Earth’s heroes battle Thanos for the fate of half the universe."),
			new Movie(47, "Avengers: Endgame", 2019, "Survivors attempt a time-heist to undo cosmic devastation."),
			new Movie(48, "Toy Story", 1995, "Rivalry between a cowboy doll and a space ranger turns to friendship."),
			new Movie(49, "Inside Out", 2015, "A girl’s emotions guide her through a difficult move."),
			new Movie(50, "The Social Network", 2010, "Facebook’s founding sparks friendship and legal battles.")
		});
	}

	private void SeedUsers()
	{
		var usernames = new string[]
		{
			"papo", "pepo", "popo", "pipo",
			"momo", "moma", "mama", "papa",
			"lalo", "lola", "lala", "lilo"
		};

		Random r = new Random();

		foreach (var username in usernames)
		{
			var pass = Path.GetRandomFileName();
			var salt = Path.GetRandomFileName();
			var role = Roles.ROLES[r.Next(Roles.ROLES.Length)];
			User user = new User(nextUserId++, username, pass, salt, role);
			Users.Add(user);
		}
	}

	public int NextActorId()
	{
		return ++nextActorId;
	}

	public int NextActorMovieId()
	{
		return ++nextActorMovieId;
	}

	public int NextMovieId()
	{
		return ++nextMovieId;
	}

	public int NextUserId()
	{
		return ++nextUserId;
	}
}
