using DTOGenerator.Attributes;

namespace Demo
{
    [GenerateDto]
    public class Zetter
    {
        [ExcludeProperty]
        public string FirstName { get; set; }
        [ExcludeProperty]
        public string LastName { get; set; }
        public int Age { get; set; }

        public Station Station { get; set; } = new Station()
        {
            Level = 5
        };
    }
}