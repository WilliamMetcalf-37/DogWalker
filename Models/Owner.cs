using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogWalker.Models
{
    public class Owner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        //T his is to hold the actual foreign key integer
        public int NeighborhoodId { get; set; }

        // This property is for storing the C# object representing the department
        public Neighborhood Neighborhood { get; set; }
        public string Phone { get; set; }

        public List<Dog> Doggos { get; set;}
    }
}
