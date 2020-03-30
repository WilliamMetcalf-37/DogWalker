using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogWalker.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }

        //T his is to hold the actual foreign key integer
        public int OwnerId { get; set; }

        // This property is for storing the C# object representing the department
        public Owner Owner { get; set; }
    }
}
