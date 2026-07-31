using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace TimeOrder
{
    public class Deck
    {
        public List<Card> Cards { get; set; }
        private Random _random;

        public int Count
        {
            get
            {
                return Cards.Count;
            }
        }
        public int Min { get; set; }
        public int Max { get; set; }

        public Deck()
        {
            Cards = new List<Card>();
            _random = new Random();
        }

        public Deck(List<Card> cards)
        {
            Cards = new List<Card>(cards);
            _random = new Random();
            Min = cards.Select(card => card.Date_Birth).Min() - 10;
            Max = cards.Select(card => card.Date_Death).Max() + 10;
        }

        public static Deck LoadFromJson(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                MessageBox.Show("Le fichier JSON est introuvable : " + filePath);
                return new Deck(new List<Card>());
            }

            try
            {
                string json = File.ReadAllText(filePath);

                JsonSerializerOptions options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;

                List<Card> cards = JsonSerializer.Deserialize<List<Card>>(json, options);

                if (cards == null)
                {
                    cards = new List<Card>();
                }

                List<Card> validCards = new List<Card>();

                for (int i = 0; i < cards.Count; i++)
                {
                    Card card = cards[i];

                    if (card == null)
                    {
                        continue;
                    }

                    if (card.Name == null)
                    {
                        card.Name = "";
                    }

                    if (card.Type == null)
                    {
                        card.Type = "";
                    }

                    if (card.Date_Birth == null)
                    {
                        card.Date_Birth = 0;
                    }

                    if (card.Date_Death == null)
                    {
                        card.Date_Death = 0;
                    }

                    if (card.Description == null)
                    {
                        card.Description = "";
                    }

                    if (card.ImagePath == null)
                    {
                        card.ImagePath = "";
                    }

                    if (IsValidCard(card))
                    {
                        validCards.Add(card);
                    }
                    else
                    {
                        Console.WriteLine("Carte invalide ignorée : " + card.Name);
                    }
                }

                Deck deck = new Deck(validCards);
                deck.Shuffle();

                return deck;
            }
            catch (JsonException exception)
            {
                MessageBox.Show("Erreur dans le JSON : " + exception.Message);
                return new Deck(new List<Card>());
            }
            catch (Exception exception)
            {
                MessageBox.Show("Impossible de charger le JSON : " + exception.Message);
                return new Deck(new List<Card>());
            }
        }

        private static bool IsValidCard(Card card)
        {
            if (card.id <= 0)
            {
                return false;
            }

            if (card.Name.Trim() == "")
            {
                return false;
            }

            if (card.Date_Birth == 0)
            {
                return false;
            }

            if (card.Date_Death == 0)
            {
                return false;
            }

            if (card.Date_Death < card.Date_Birth)
            {
                return false;
            }

            return true;
        }

        public void Shuffle()
        {
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int randomIndex = _random.Next(0, i + 1);

                Card temporaryCard = Cards[i];
                Cards[i] = Cards[randomIndex];
                Cards[randomIndex] = temporaryCard;
            }
        }

        public Card DrawCard()
        {
            if (Cards.Count == 0)
            {
                return null;
            }

            Card card = Cards[0];

            Cards.RemoveAt(0);

            return card;
        }
    }
}