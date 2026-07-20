using System.Text.Json;

namespace TimeOrder
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            List<Card> cards = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText("./data.json")) ?? new List<Card>();

            Application.Run(new Gameplay(cards,"test"));
        }
    }
}