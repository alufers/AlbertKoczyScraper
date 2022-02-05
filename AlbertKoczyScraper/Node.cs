using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbertKoczyScraper
{
    internal abstract class Node
    {

        public abstract void Print(int indentLevel);
        interface NodeWithChildren
        {
            public List<Node> children { get; set; }

            public List<string> GetLinks();



        }

        public class RootNode : Node, NodeWithChildren
        {
            public List<Node> _children = new List<Node>();

            public List<Node> children { get => _children; set => _children = value; }

            public override void Print(int indentLevel)
            {
                Console.WriteLine(new string(' ', indentLevel) + "RootNode");
                foreach (var child in children)
                {
                    child.Print(indentLevel + 2);
                }
            }
            public List<string> GetLinks()
            {
                List<string> links = new List<string>();
                foreach (Node child in children)
                {
                    if (child is NodeWithChildren)
                    {
                        links.AddRange((child as NodeWithChildren)!.GetLinks());
                    }
                }
                return links;
            }
        }

        public class TextNode : Node
        {
            public string text;
            public TextNode(string t)
            {
                text = t;
            }


            public override void Print(int indentLevel)
            {
                Console.WriteLine(new string(' ', indentLevel) + "|- TextNode: " + text?.Replace("\n", "\\n").Replace("\r", "\\r"));
            }
        }

        public class ElementNode : Node, NodeWithChildren
        {

            public string tagName;
            public List<Node> _children = new List<Node>();

            public List<Node> children { get => _children; set => _children = value; }

            public Dictionary<string, string> attributes = new Dictionary<string, string>();

            public ElementNode(string tagName)
            {
                this.tagName = tagName;
            }
            public override void Print(int indentLevel)
            {
                Console.WriteLine(new string(' ', indentLevel) + "|- ElementNode: " + tagName + "(" + attributes.Aggregate("", (s, kvp) => s + kvp.Key + "=" + kvp.Value + " ") + ")");
                foreach (var child in children)
                {
                    child.Print(indentLevel + 2);
                }
            }

            public List<string> GetLinks()
            {
                
                List<string> links = new List<string>();
                if (tagName == "a")
                {
                    if (attributes.ContainsKey("href"))
                    {
                        links.Add(attributes["href"]);
                    }
                }
                foreach (Node child in children)
                {
                    if (child is NodeWithChildren)
                    {
                        links.AddRange((child as NodeWithChildren)!.GetLinks());
                    }
                }
                return links;
            }
        }


    }
}
