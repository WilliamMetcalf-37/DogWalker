using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogWalker.Models
{
    public class Walk
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }

        //T his is to hold the actual foreign key integer
        public int DogId { get; set; }
        public int WalkerId { get; set; }
        // This property is for storing the C# object representing the department
        public Dog Dog { get; set; }
        public Walker Walker { get; set; }
    }
}
