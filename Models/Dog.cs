using Microsoft.EntityFrameworkCore;

namespace codebridgeTEST.Models
{
    //names can repeat, but combination of name and other value is less possible to do so
    [PrimaryKey(nameof(Name), nameof(TailLength))]
    public class Dog
    {
        public string Name { get; set; }
        public string Color { get; set; }

        //in every example values are int, but logically double is more fitting
        public int TailLength { get; set; }
        public int Weight { get; set; }


    }
}
