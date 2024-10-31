using Microsoft.EntityFrameworkCore;

namespace codebridgeTEST.Models
{
    [PrimaryKey(nameof(Name), nameof(TailLength))]
    public class Dog
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int TailLength { get; set; }
        public int Weight { get; set; }


    }
}
