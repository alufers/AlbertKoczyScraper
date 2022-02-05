using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbertKoczyScraper
{
    internal class Lexer
    {
        Queue<Token> tokens = new Queue<Token>();
        Queue<char> chars = new Queue<char>();

        public Lexer(string source)
        {
            foreach (char c in source)
            {
                chars.Enqueue(c);
            }
        }

        public Queue<Token> Scan()
        {
            while (chars.Count > 0)
            {
                char c = chars.Dequeue();

                switch (c)
                {
                    case '<':

                        // remove comments
                        if (chars.Peek() == '!' && chars.Skip(1).First() == '-' && chars.Skip(2).First() == '-')
                        {
                            chars.Dequeue();
                            chars.Dequeue();
                            while (chars.Peek() != '-' || chars.Skip(1).First() != '-' || chars.Skip(2).First() != '>')
                            {
                                chars.Dequeue();
                            }
                            chars.Dequeue();
                            chars.Dequeue();
                            break;
                        }

                        tokens.Enqueue(new Token.LeftChevron());
                        break;
                    case '>':
                        tokens.Enqueue(new Token.RightChevron());
                        break;
                    case '/':
                        tokens.Enqueue(new Token.Slash());
                        break;
                    case '=':
                        tokens.Enqueue(new Token.EqualsSign());
                        break;
                    case '"':
                        tokens.Enqueue(new Token.Quote());
                        break;
                    case '\'':
                        tokens.Enqueue(new Token.SingleQuote());
                        break;
                    case '&':
                        tokens.Enqueue(new Token.Ampersand());
                        break;
                    case ';':
                        tokens.Enqueue(new Token.Semi());
                        break;
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        if (!typeof(Token.Whitespace).IsInstanceOfType(tokens.Last()))
                        {
                            tokens.Enqueue(new Token.Whitespace(""));
                        }
                        tokens.Last().lexeme += c;
                        break;
                    default:
                        if (!typeof(Token.Text).IsInstanceOfType(tokens.Last()))
                        {
                            tokens.Enqueue(new Token.Text(""));
                        }
                        tokens.Last().lexeme += c;
                        break;


                }
            }

            return tokens;
        }

    }
}
