using System;
using System.Text;
using System.Xml;

namespace TestDisplay
{
    public static class MainClass
    {
        static void Main(string[] args)
        {
            XmlReader reader = XmlReader.Create(args[0]);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch(reader.Name)
                    {
                    case "test-run":
                        {
                            var total = reader.GetAttribute("total");
                            var passed = reader.GetAttribute("passed");
                            var failed = reader.GetAttribute("failed");

                            Console.WriteLine("Test Results: passed: " + passed + "/" + total + "  failed: " + failed + "/" + total);
                        }
                        break;
                    case "test-suite":
                        {
                            var type = reader.GetAttribute("type");
                            var name = reader.GetAttribute("name");
                            var result = reader.GetAttribute("result");

                            if (type == "TestSuite") Console.Write(name + " result: ");
                            else if (type == "TestFixture") Console.Write("\t" + name + " result: ");

                            if (result.ToLower() == "passed") Console.ForegroundColor = ConsoleColor.Green;
                            else Console.ForegroundColor = ConsoleColor.Red;
                            
                            Console.WriteLine(result);
                            Console.ResetColor();

                        }
                        break;
                    case "test-case":
                        {   
                            var name   = reader.GetAttribute("name");
                            var result = reader.GetAttribute("result");

                            Console.Write("\t\t" + name + " result: ");

                            if (result.ToLower() == "passed") Console.ForegroundColor = ConsoleColor.Green;
                            else Console.ForegroundColor = ConsoleColor.Red;
                            
                            Console.WriteLine(result);
                            Console.ResetColor();

                            while (reader.Read())
                            {
                                if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "test-case")) break;
                                else if (reader.NodeType == XmlNodeType.CDATA) Console.WriteLine("\t\t\t" + reader.Value.Replace(System.Environment.NewLine, ""));
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
