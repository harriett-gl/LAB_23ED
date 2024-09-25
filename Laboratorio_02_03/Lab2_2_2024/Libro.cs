using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_2_2024
{
    public class Libro
    {
        public string Isbn { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }

        
        public Libro(string isbn, string name, string author, string category, string price, string quantity)
        {
            Isbn = isbn;
            Name = name;
            Author = author;
            Category = category;
            Price = price;
            Quantity = quantity;
        }

        
        public override bool Equals(object obj)
        {
            if (obj is Libro other)
            {
                return Isbn == other.Isbn;
            }
            return false;
        }

        
        public override int GetHashCode()
        {
            return Isbn.GetHashCode();
        }
    }

}
