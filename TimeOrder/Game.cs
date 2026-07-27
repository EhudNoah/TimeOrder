using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeOrder
{
    public class Game
    {
        public Deck Deck { get; set; } = Deck.LoadFromJson("./data.json");
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Card> PlayedCards { get; set; } = new List<Card>();
        public bool IsStarted { get; set; } = false;

        public void ShareCards()
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (Player player in Players)
                {
                    player.DrawCard(Deck);
                }
            }
        }
    }
}
