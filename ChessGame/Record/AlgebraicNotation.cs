using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;

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
        /// <summary>
        /// Properties will be set in <see cref="ParseTag"/>
        /// </summary>
        // Must
        public string Event { get; private set; } = ""; // Name of the tournament or match event.
        public string Site { get; private set; } = ""; // Location of the event. This is in City, Region COUNTRY format, where COUNTRY is the three-letter International Olympic Committee code for the country. An example is New York City, NY USA. Although not part of the specification, some online chess platforms will include a URL or website as the site value.[3]
        public DateTime Date { get; private set; } // Starting date of the game, in YYYY.MM.DD form. ?? is used for unknown values.
        public int Round { get; private set; } // Playing round ordinal of the game within the event.
        public string White { get; private set; } = ""; // Player of the white pieces, in Lastname, Firstname format.
        public string Black { get; private set; } = ""; // Player of the black pieces, same format as White.
        public string Result { get; private set; } = ""; // Result of the game. It is recorded as White score, dash, then Black score, or * (other, e.g., the game is ongoing).
        public string Movetext { get; private set; } = "";
        public List<MoveInfo> Moveinfos = new();
        // Optional
        public int PlyCount { get; private set; } // String value denoting the total number of half-moves played.
        public FEN Fen { get; private set; } = FEN.Default; // The initial position of the chessboard

        public PGN() { }

        public void Load(string path)
        {
            if (!File.Exists(path))
                return;

            // Set value to properties
            var lines = File.ReadLines(path);
            ReadlineState state = ReadlineState.Tags;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                switch (state)
                {
                    case ReadlineState.Tags:
                        char firstChar = line[0];
                        switch (firstChar)
                        {
                            case '[': // Tags
                                if (!ParseTag(line))
                                    state = ReadlineState.Error;
                                break;
                            case '1':
                                Movetext += line;
                                state = ReadlineState.Movetext;
                                break;
                        }
                        break;
                    case ReadlineState.Movetext:
                        Movetext += line;
                        break;
                    case ReadlineState.Error:
                        Debug.WriteLine("Wring format");
                        break;
                }
            }
            Movetext = RemoveComments(Movetext);
            ParseMovetext();
        }

        private bool ParseTag(string line)
        {
            var name = line.Substring(1, line.IndexOf(' '));
            var firstQuote = line.IndexOf('"');
            var secondQuotoe = line.IndexOf('"', firstQuote + 1);
            var value = line.Substring(firstQuote + 1, secondQuotoe - 1);
            var property = GetType().GetProperty(name);
            if (property == null)
            {
                Debug.WriteLine($"Unsuppoort Tag Name: {name}");
                return false;
            }
            property.SetValue(this, value);
            return true;
        }

        private static string RemoveComments(string movetext)
        {
            // Remove comments starting with '{' and ending with '}'
            string pattern = @"{[^}]*}";
            string result = Regex.Replace(movetext, pattern, " ");

            // Remove comments starting with ';'
            pattern = @";[^\n\r]*";
            return Regex.Replace(result, pattern, " ");
        }

        private void ParseMovetext()
        {
            var words = Movetext.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var state = MovetextState.Number;
            foreach (var word in words)
            {
                switch (state)
                {
                    case MovetextState.Number:
                        if (Regex.IsMatch(word, @"^\d+\.$"))
                            state = MovetextState.WhiteMove;
                        else if (IsBlackNumber(word))
                            state = MovetextState.BlackMove;
                        else
                            state = MovetextState.Error;
                        break;
                    case MovetextState.WhiteMove:
                        var info = ParsePGNMove(word);
                        Moveinfos.Add(info);
                        if (info != null)
                            state = MovetextState.BlackNumberMove;
                        else
                            state = MovetextState.Error;
                        break;
                    case MovetextState.BlackNumberMove:
                        if ((IsBlackNumber(word)))
                            state = MovetextState.BlackMove;
                        else
                            state = BlackMove(word);
                        break;
                    case MovetextState.BlackMove:
                        state = BlackMove(word);
                        break;
                    case MovetextState.Error:
                        Debug.WriteLine("Wring format in Movetext");
                        break;
                }
            }

            static bool IsBlackNumber(string number)
            {
                return Regex.IsMatch(number, @"^\d+\.\.\.$");
            }
            //bool IsValidMove(string word)
            //{
            //    string pattern = @"^[PNBRQK]?[a-h]?[1-8]?[x]?[a-h][1-8][+#]?";
            //    return Regex.IsMatch(word, pattern);
            //}
            static MoveInfo ParsePGNMove(string move)
            {
                // 定義正則表達式模式，用於捕獲 PGN 中的移動信息
                string pattern = @"(?<piece>[NBRQK]?)(?<startFile>[a-h]?)(?<startRank>[1-8]?)(?<capture>x?)(?<endFile>[a-h])(?<endRank>[1-8])(?<promotion>(=[NBRQ])?)(?<check>[+#]?)";

                // 使用 Regex.Match 方法進行匹配
                Match match = Regex.Match(move, pattern);

                // 初始化 MoveInfo 對象，用於結構化存儲移動信息
                MoveInfo moveInfo = new MoveInfo();

                // 如果匹配成功，則結構化存儲移動信息
                if (match.Success)
                {
                    moveInfo.Piece = match.Groups["piece"].Value;
                    moveInfo.StartCoord = MoveInfo.ConvertToCoord(match.Groups["startFile"].Value + match.Groups["startRank"].Value);
                    moveInfo.EndCoord = MoveInfo.ConvertToCoord(match.Groups["endFile"].Value + match.Groups["endRank"].Value);
                    moveInfo.PromotedPiece = match.Groups["promotion"].Value;
                    // 這裡省略了捕獲和晉升的部分，可以根據需要添加相應的邏輯
                }

                return moveInfo;
            }
            MovetextState BlackMove(string move)
            {
                MovetextState state;
                var info = ParsePGNMove(move);
                Moveinfos.Add(info);
                if (info != null)
                {
                    // ...
                    state = MovetextState.Number;
                }
                else if (IsEnd(move))
                {
                    state = MovetextState.End;
                }
                else
                    state = MovetextState.Error;
                return state;
            }
            bool IsEnd(string word)
            {
                return new string[] { "1-0", "0-1", "1/2-1/2" }.Contains(word);
            }

        }

        private enum ReadlineState
        {
            Error = -1,
            //Start,
            Tags,
            Movetext,
            //End,
        }

        private enum MovetextState
        {
            Error = -1,
            Number,
            WhiteMove,
            BlackNumberMove,
            BlackMove,
            End
        }

        public class MoveInfo
        {
            public string Piece { get; set; }
            public Coord StartCoord { get; set; }
            public Coord EndCoord { get; set; }
            public string PromotedPiece { get; set; }

            public static Coord ConvertToCoord(string fileAndRank)
            {
                if (fileAndRank.Length != 2)
                    throw new ArgumentException("Invalid input. The string length must be 2.");

                char fileChar = fileAndRank[0];
                char rankChar = fileAndRank[1];

                if (fileChar < 'a' || fileChar > 'h' || rankChar < '1' || rankChar > '8')
                    throw new ArgumentException("Invalid input. The input must be in the format 'file and rank' within the range of 'a' to 'h' for file and '1' to '8' for rank, e.g., 'f4', 'a1', etc.");

                int file = fileChar - 'a'; // 將字母表示的列轉換為整數
                int rank = 7 - (rankChar - '1'); // 將數字表示的行轉換為整數

                return new Coord(rank, file);
            }
        }
    }
}
