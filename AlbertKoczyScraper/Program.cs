using System;

namespace AlbertKoczyScraper // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            int limit = 20;
            Stack<string> urlStack = new Stack<string>();
            SortedList<string, bool> visited = new SortedList<string, bool>();
          

            urlStack.Push("https://wikipedia.org");

            while (limit > 0 && urlStack.Count > 0)
            {
                string url = urlStack.Pop();
                Console.WriteLine("ZAPYTANIE DO: " + url);
                string stringContents = "";
                visited.Add(url, true);
                try
                {
                    stringContents = await WebUtil.MakeGETRequest(url);
                }
                catch (Exception e)
                {
                    Console.WriteLine("BLAD SIECI: " + e.Message);
                }
                 Console.WriteLine("ZAPYTANIE OK: " + url);
                var lexer = new Lexer(stringContents);
                var scannedTokens = lexer.Scan();
                var parser = new Parser(scannedTokens);
                var rootNode = parser.Parse();
                rootNode.Print(0);
                Console.WriteLine("PARSKOWANIE OK: " + url);
                var links = rootNode.GetLinks();
                foreach (var link in links)
                {
                    if (!visited.ContainsKey(link))
                    {
                        urlStack.Push(link);
                    }
                    Console.WriteLine("LINK: " + link);
                    
                }
                Console.WriteLine("\n\n");
                limit--;

            }



        }
    }
}
