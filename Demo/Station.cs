using DTOGenerator.Attributes;

namespace Demo
{
    [GenerateDto("StationDTO", "StationWithNoNameDTO")]
    public class Station
    {
        [ExcludeProperty("StationWithNoNameDTO")]
        public string Name { get; set; }

        [ExcludeProperty]
        public int Level { get; set; }
    }
}