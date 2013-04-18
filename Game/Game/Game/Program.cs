using System;
using DigitalRune;

namespace ICT309Game
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // Necessary for using the functions available in the DigitalRune engine.
            DigitalRune.Licensing.AddSerialNumber("tgCcAcuJg2I1Hs4BYfH2YgY9zwEiACNUaW1vdGh5IFZlbGV0dGEjMSMzI05vbkNvbW1lcmNpYWxAg7LDkM1RVAQDAfwPTE1CWShg8ydjAzN820/FDwY81X7iRsmnEZIi9Zr3Fz46IC9N7KhUL6OSReHCzBLn");

            using (GameManager game = new GameManager())
            {
                game.Run();
            }
        }
    }
#endif
}

