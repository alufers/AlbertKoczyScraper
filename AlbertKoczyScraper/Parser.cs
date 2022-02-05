using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbertKoczyScraper
{
    internal class Parser
    {

        public static readonly string[] quirkyTags = { "script", "style" };
        public static readonly string[] selfClosingTags = { "br", "hr", "img", "input", "link", "meta" };

        private Queue<Token> tokens;
        public Parser(Queue<Token> t)
        {
            tokens = t;
        }

        public Node.RootNode Parse()
        {
            var n = new Node.RootNode();
            var c = n.children;
            parseChildren(ref c);
            n.children = c;
            return n;
        }

        private void parseChildren(ref List<Node> children, string? elementName = null)
        {
            children = new List<Node>();
            while (tokens.Count > 0)
            {
                try
                {


                    var tok = tokens.Dequeue();
                    switch (tok)
                    {
                        case Token.LeftChevron chevron:
                            swallowWhitespace();
                            if (tokens.Peek() is Token.Slash)
                            {
                                tokens.Dequeue();
                                swallowWhitespace();
                                var endingName = swallowText();
                                if (tokens.Peek() is Token.RightChevron)
                                {
                                    tokens.Dequeue();

                                }
                                if (elementName != null && elementName == endingName)
                                {
                                    return;
                                }
                                if (elementName != null)
                                {
                                    Console.WriteLine("UNKNOWN ENDING TAG INSIDE OF " + elementName + ": " + endingName);
                                }


                                break;
                            }
                            var nodeName = swallowText();
                            var node = new Node.ElementNode(nodeName);
                            node.attributes = parseAttributes();
                            var tagEnding = tokens.Dequeue();
                            if (tagEnding is Token.Slash)
                            {
                                swallowWhitespace();
                                if (tokens.Peek() is Token.RightChevron)
                                {
                                    tokens.Dequeue();

                                }

                            }
                            else
                            {
                                if (tokens.Peek() is Token.RightChevron)
                                {
                                    tokens.Dequeue();

                                }
                                swallowWhitespace();
                                if (selfClosingTags.Contains(nodeName))
                                {
                                    node.children = new List<Node>();
                                }
                                else if (quirkyTags.Contains(nodeName))
                                {
                                    node.children = ParseQuirkyTag(nodeName);
                                }
                                else
                                {
                                    var c = node.children;
                                    parseChildren(ref c, nodeName);
                                    node.children = c;
                                }
                            }
                            children.Add(node);

                            break;
                        default:
                            var text = tok.lexeme;
                            if(children.Count != 0 && children.Last() is Node.TextNode tn)
                            {
                                tn.text += text;
                            }
                            else
                            {
                                children.Add(new Node.TextNode(text));
                            }
                            
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    
                    // synchronize
                    while (tokens.Count > 0)
                    {
                        var tok = tokens.Peek();
                        if (tok is Token.LeftChevron)
                        {
                            break;
                        }
                        tokens.Dequeue();
                    }
                }
            }
          
           
            
        }

        private Dictionary<string, string> parseAttributes()
        {
            var attributes = new Dictionary<string, string>();
            swallowWhitespace();
            while (tokens.Count > 0)
            {
                if (tokens.Peek() is Token.Slash)
                {
                    return attributes;
                }
                if (tokens.Peek() is Token.RightChevron)
                {
                    return attributes;
                }
                var tok = tokens.Dequeue();
                switch (tok)
                {
                    case Token.Text text:
                        var key = text.lexeme;
                        swallowWhitespace();
                        if (tokens.Peek() is Token.EqualsSign equals)
                        {
                            tokens.Dequeue();
                            swallowWhitespace();
                            attributes.Add(key, parseAttributeValue());
                            swallowWhitespace();
                        }
                        else
                        {
                            attributes.Add(key, "");
                        }

                        break;

                    case Token.RightChevron rightChevron:
                        return attributes;
                }
            }
            return attributes;
        }

        private string parseAttributeValue()
        {
            if (tokens.Peek() is Token.Slash || tokens.Peek() is Token.RightChevron)
            {
                return "";
            }
            var tok = tokens.Dequeue();
            switch (tok)
            {
                case Token.Quote quote:
                    {
                        var v = "";
                        while (tokens.Count > 0)
                        {
                            tok = tokens.Dequeue();
                            switch (tok)
                            {
                                case Token.Quote quote2:
                                    return v;
                                default:
                                    v += tok.lexeme;
                                    break;
                            }
                        }
                        return v;
                    }
                case Token.SingleQuote singleQuote:
                    var value = "";
                    while (tokens.Count > 0)
                    {
                        tok = tokens.Dequeue();
                        switch (tok)
                        {
                            case Token.SingleQuote singleQuote2:
                                return value;
                            default:
                                value += tok.lexeme;
                                break;
                        }
                    }
                    return value;
                default:
                    return tok.lexeme;
            }

        }

        private List<Node> ParseQuirkyTag(string tagName)
        {
            var children = new List<Node>();
            string strVal = "";
            while (tokens.Count > 0)
            {
                var tok = tokens.Dequeue();
                switch (tok)
                {
                    case Token.LeftChevron chevron:
                        swallowWhitespace();
                        if (tokens.Peek() is Token.Slash)
                        {
                            tokens.Dequeue();

                            swallowWhitespace();
                            var endingName = swallowText();
                            swallowWhitespace();
                            if (tokens.Peek() is Token.RightChevron)
                            {
                                tokens.Dequeue();

                            }
                            if (tagName == endingName)
                            {
                                children.Add(new Node.TextNode(strVal));
                                return children;
                            }
                        }
                        break;

                }
                strVal += tok.lexeme;
            }
            children.Add(new Node.TextNode(strVal));
            return children;
        }


        private void swallowWhitespace()
        {
            while (tokens.Count > 0 && typeof(Token.Whitespace).IsInstanceOfType(tokens.Peek()))
            {
                tokens.Dequeue();
            }
        }
        private string swallowText()
        {
            var tok = tokens.Dequeue();
            if (!typeof(Token.Text).IsInstanceOfType(tok))
            {
                throw new Exception("Expected tag name");
            }
            return tok.lexeme;

        }


    }
}
