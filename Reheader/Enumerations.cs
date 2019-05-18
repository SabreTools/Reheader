using System;
using System.Collections.Generic;
using System.Text;

namespace Reheader
{
    public enum GameConsole
    {
        NONE,

        Dendy,
        Famicom,
        NesNtsc,
        NesPal,
        Playchoice,
        VsSystem,
    }

    public enum Mirroring
    {
        NONE,

        Vertical,
        Horizontal,
        FourScreen,

        // Unknown
        A,
    }

    public static class Extensions
    {
        /// <summary>
        /// Get long name representation for a GameConsole
        /// </summary>
        /// <param name="console">GameConsole value</param>
        /// <returns>Long name representation of input, if possible</returns>
        public static string Name(this GameConsole console)
        {
            switch (console)
            {
                case GameConsole.Dendy:
                    return "Steepler Dendy";
                case GameConsole.Famicom:
                    return "Nintendo Famicom";
                case GameConsole.NesNtsc:
                    return "Nintendo Entertainment System (NTSC)";
                case GameConsole.NesPal:
                    return "Nintendo Entertainment System (PAL)";
                case GameConsole.Playchoice:
                    return "Nintendo PlayChoice-10";
                case GameConsole.VsSystem:
                    return "Nintendo Vs. System";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// Get long name representation for a Mirroring
        /// </summary>
        /// <param name="mirroring">Mirroring value</param>
        /// <returns>Long name representation of input, if possible</returns>
        public static string Name(this Mirroring mirroring)
        {
            switch (mirroring)
            {
                case Mirroring.Vertical:
                    return "Vertical";
                case Mirroring.Horizontal:
                    return "Horizontal";
                case Mirroring.A:
                    return "A (Unknown)";
                case Mirroring.FourScreen:
                    return "Four-Screen VRAM";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// Get GameConsole value from a string
        /// </summary>
        /// <param name="console">String value</param>
        /// <returns>GameConsole representation of the input, if possible</returns>
        public static GameConsole GameConsoleFromString(string console)
        {
            switch (console)
            {
                case "Dendy":
                    return GameConsole.Dendy;
                case "Famicom":
                    return GameConsole.Famicom;
                case "NesNtsc":
                    return GameConsole.NesNtsc;
                case "NesPal":
                    return GameConsole.NesPal;
                case "Playchoice":
                    return GameConsole.Playchoice;
                case "VsSystem":
                    return GameConsole.VsSystem;
                default:
                    return GameConsole.NONE;
            }
        }

        /// <summary>
        /// Get Mirroring value from a string
        /// </summary>
        /// <param name="mirroring">String value</param>
        /// <returns>Mirroring representation of the input, if possible</returns>
        public static Mirroring MirroringFromString(string mirroring)
        {
            switch (mirroring)
            {
                case "h":
                    return Mirroring.Horizontal;
                case "v":
                    return Mirroring.Vertical;
                case "4":
                    return Mirroring.FourScreen;
                case "a":
                    return Mirroring.A;
                default:
                    return Mirroring.NONE;
            }
        }
    }
}
