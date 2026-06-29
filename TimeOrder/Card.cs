using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeOrder
{
    public class Card
    {
        public int id { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Date_Birth { get; private set; }
        public string Date_Death { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public bool IsDateVisible { get; private set; }

        public Card(int id, string name, string type, string date_birth, string date_death, string description, string imagePath, bool isDateVisible)
        {
            
            if ( id != 0 && name != "" && type != "" && date_birth!= "" && date_death != "" &&  description != "" && imagePath!= "" )
            {
                this.id = id;
                Name = name;
                Type = type;
                Date_Birth = date_birth;
                Date_Death = date_death;
                Description = description;
                ImagePath = imagePath;
                IsDateVisible = isDateVisible;
            } else
            {
                MessageBox.Show("Please enter a valid argument.");
            }
        }

        public override string ToString()
        {
            return ($"Id: {id}, Name : {Name}, Type : {Type}, Date of birth : {Date_Birth} , Date of death : {Date_Death}, Description : {Description}, ImagePath : {ImagePath}, IsDateVisible : {IsDateVisible}");
        }
    }
}
