using BenchmarkDotNet.Attributes;

namespace CallbackBenchMarks;

[MemoryDiagnoser]
public class Benchmarks
{
    private static readonly List<Person> _people = GetRandomPeople(100_000).ToList();
    private static readonly Dictionary<Guid, DateTime> _deathDates = GetDeathDates(_people);

    [Benchmark(Baseline = true)]
    public void CallingDirectlyFromHashMap()
    {
        for (int i = 0; i < _people.Count; i++)
        {
            Process(_people[i], _deathDates[_people[i].Id]);
        }
    }

    [Benchmark]
    public void CallingUsingCallBack()
    {
        for (int i = 0; i < _people.Count; i++)
        {
            Process(_people[i], (id) => _deathDates[id]);
        }
    }

    private static void Process(Person person, DateTime deathDate)
    {
        if (person.Age >= 18)
        {
            person.SetDeathDate(deathDate);
        }
    }

    private static void Process(Person person, Func<Guid, DateTime> getDeathDate)
    {
        if (person.Age >= 18)
        {
            person.SetDeathDate(getDeathDate(person.Id));
        }
    }

    private static IEnumerable<Person> GetRandomPeople(int count)
    {
        var random = new Random();
        var people = new List<Person>();
        for (int i = 0; i < count; i++)
        {
            people.Add(new Person
            {
                Id = Guid.NewGuid(),
                Name = $"Person {i}",
                Age = i % 2 == 0 ? random.Next(18, 100) : random.Next(0, 17)
            });
        }
        return people;
    }

    private static Dictionary<Guid, DateTime> GetDeathDates(IEnumerable<Person> people)
    {
        var deathDates = new Dictionary<Guid, DateTime>();
        foreach (var person in people)
        {
            deathDates.Add(person.Id, DateTime.Now.AddDays(person.Age));
        }

        return deathDates;
    }

    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime? DeathDate { get; set; }

        public void SetDeathDate(DateTime deathDate)
        {
            DeathDate = deathDate;
        }
    }
}