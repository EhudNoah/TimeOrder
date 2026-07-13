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

            Card Einstein = new Card(
                1,
                "Einstein",
                "Person",
                "1879",
                "1955",
                "Il publie sa théorie de la relativité restreinte en 1905 et sa théorie de la gravitation, dite relativité générale, en 1915.",
                @"D:\.NET C#\Images\Albert_Einstein.jpg",
                false
            );

            Card Newton = new Card(
                1,
                "Newton",
                "Person",
                "1643",
                "1727",
                "Isaac Newton est un mathématicien, physicien, philosophe, alchimiste, astronome et théologien anglais, puis britannique.",
                @"D:\.NET C#\Images\Isaac_Newton.jpg",
                false
            );

            Card Descartes = new Card(
                1,
                "Descartes",
                "Person",
                "1596",
                "1650",
                "Toutes les occurrences du nom de lieu « La Haye » dans cet article font référence à La Haye-en-Touraine, aujourd'hui Descartes et non à La Haye.",
                @"D:\.NET C#\Images\Rene_Descartes.jpg",
                false
            );


            Application.Run(new Gameplay(Einstein, Newton, Descartes, "test"));
        }
    }
}