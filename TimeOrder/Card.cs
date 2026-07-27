using System;

namespace TimeOrder
{
    public class Card
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Date_Birth { get; set; }
        public string Date_Death { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsDateVisible { get; set; }

        public Card()
        {
            Name = "";
            Type = "";
            Date_Birth = "";
            Date_Death = "";
            Description = "";
            ImagePath = "";
        }

        public Card(int id, string name, string type, string date_birth, string date_death, string description, string imagePath, bool isDateVisible)
        {
            this.id = id;
            Name = name;
            Type = type;
            Date_Birth = date_birth;
            Date_Death = date_death;
            Description = description;
            ImagePath = imagePath;
            IsDateVisible = isDateVisible;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}