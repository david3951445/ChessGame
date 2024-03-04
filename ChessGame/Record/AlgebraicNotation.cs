using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessGame
{
    public class AlgebraicNotation
    {
        private PGN pgn = new PGN();

        public void Load(string path)
        {
            pgn.Load(path);
        }
    }

    public class PGN
    {
        // Must
        public string Event = ""; // Name of the tournament or match event.
        public string Site = ""; // Location of the event. This is in City, Region COUNTRY format, where COUNTRY is the three-letter International Olympic Committee code for the country. An example is New York City, NY USA. Although not part of the specification, some online chess platforms will include a URL or website as the site value.[3]
        public DateTime Date; // Starting date of the game, in YYYY.MM.DD form. ?? is used for unknown values.
        public int Round; // Playing round ordinal of the game within the event.
        public string White = ""; // Player of the white pieces, in Lastname, Firstname format.
        public string Black = ""; // Player of the black pieces, same format as White.
        public string Result = ""; // Result of the game. It is recorded as White score, dash, then Black score, or * (other, e.g., the game is ongoing).
        // Optional
        public int PlyCount; // String value denoting the total number of half-moves played.
        // ...

        public PGN() { }

        public void Load(string path)
        {
            if (!File.Exists(path))
                return;
            var lines = File.ReadLines(path);
            foreach (var line in lines)
            {
                int state = 1;
                if (string.IsNullOrEmpty(line))
                {
                    state = 2;
                    continue;
                }
                char firstChar = line[0];
                switch (state)
                {
                    case 1: // Read Tags
                        switch (firstChar)
                        {
                            case '[':
                                break;
                            default:
                                state = -1;
                                break;
                        }
                        break;
                    case 2: // Read 
                        if (firstChar == '1')
                        {

                            state = 3;
                        }
                        break;
                    case 3:

                    case -1:
                        Debug.WriteLine("Wring Format");
                        break;

                }
                switch (line[0])
                {
                    case '[':
                        //ParseTag(line);
                        break;
                    case '1'
                }
                Console.WriteLine(line);
            }
        }
    }
}
