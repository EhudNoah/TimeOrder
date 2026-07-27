namespace TimeOrder
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Deck deck = Deck.LoadFromJson("./data.json");

            Application.Run(new Form1(deck));
        }
    }
}