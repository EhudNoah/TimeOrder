using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace TimeOrder
{
    public class Deck
    {
        private List<Card> _cards;
        private Random _random;

        public List<Card> AllCards { get; private set; }

        public int Count
        {
            get
            {
                return _cards.Count;
            }
        }

        public Deck(List<Card> cards)
        {
            AllCards = new List<Card>(cards);
            _cards = new List<Card>(cards);
            _random = new Random();
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
                        card.Date_Birth = "";
                    }

                    if (card.Date_Death == null)
                    {
                        card.Date_Death = "";
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

                return new Deck(validCards);
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

            if (int.TryParse(card.Date_Birth, out int birthYear) == false)
            {
                return false;
            }

            if (int.TryParse(card.Date_Death, out int deathYear) == false)
            {
                return false;
            }

            if (deathYear < birthYear)
            {
                return false;
            }

            return true;
        }

        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int randomIndex = _random.Next(0, i + 1);

                Card temporaryCard = _cards[i];
                _cards[i] = _cards[randomIndex];
                _cards[randomIndex] = temporaryCard;
            }
        }

        public Card DrawCard()
        {
            if (_cards.Count == 0)
            {
                return null;
            }

            Card card = _cards[0];

            _cards.RemoveAt(0);

            return card;
        }
    }
}