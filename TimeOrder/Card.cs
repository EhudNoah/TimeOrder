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
        public string Date { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public bool IsDateVisible { get; private set; }

        public Card(int id, string name, string type, string date, string description, string imagePath, bool isDateVisible)
        {
            
            if ( id != 0 && name != "" && type != "" && date!= "" &&  description != "" && imagePath!= "" && isDateVisible != false )
            {
                this.id = id;
                Name = name;
                Type = type;
                Date = date;
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
            return ($"Id: {id}, Name : {Name}, Type : {Type}, Date : {Date} , Description : {Description}, ImagePath : {ImagePath}, IsDateVisible : {IsDateVisible}");
        }
    }
}
