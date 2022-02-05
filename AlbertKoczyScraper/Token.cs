using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbertKoczyScraper
{
    internal abstract class Token
    {
        abstract public string lexeme { get; set; }
        internal class LeftChevron : Token
        {
            private string _lexeme = "<";
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
        }
        internal class RightChevron : Token
        {
            private string _lexeme = ">";
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
        }

        internal class Semi : Token
        {
            private string _lexeme = ";";
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
        }

        internal class Ampersand : Token
        {
            private string _lexeme = "&";
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
        }

        internal class Slash : Token
        {
            private string _lexeme = "/";
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
        }

        internal class EqualsSign : Token
        {
            private string _lexeme = "=";
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
        }

        internal class Quote : Token
        {
            private string _lexeme = "\"";
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
        }

        internal class SingleQuote : Token
        {
            private string _lexeme = "\'";
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
        }

        internal class Whitespace : Token
        {
            private string _lexeme;
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
            public Whitespace(string lexeme)
            {
                this._lexeme = lexeme;
            }
        }

        internal class Text : Token
        {
            private string _lexeme;
            public override string lexeme { get => _lexeme; set => _lexeme = value; }
            public Text(string lexeme)
            {
                this._lexeme = lexeme;
            }
        }
    }
}
