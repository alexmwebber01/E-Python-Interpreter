using System;
using System.IO;
using System.Text;


namespace proj
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: allow for args?
            string path = "../../../test/python_test_code.py";
            string[] keywords = { "while", "for", "if" };


            // Open the stream and read it back.
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                //while (fs.Read(b, 0, b.Length) > 0)
                //{
                //    Console.WriteLine(temp.GetString(b));
                //}
            }
            

            string[] pythonText;

            //List<Variable> localVariables = new List<Variable>();
            if (File.Exists(path))
            {
                pythonText = File.ReadAllLines(path);
                foreach (string text in pythonText)
                {
                    Console.WriteLine(text);
                }
            }
            else
            {
                Console.WriteLine("File not found.");
            }


            //pythonText is used to pass into while,for,if statements
            /*
            int readLine(string[] pythonText, int textIndex, int indents, List<Variable> globalVariable)
            {
                List<Variable> localVariables = new List<Variable>();
                int lineIndex = 0;

                //Check for the correct level of indentation
                for (int i = 4; i <= indents * 4; i += 4)
                {
                    if (lineIndex < pythonText[textIndex].Length)
                    {
                        if (pythonText[textIndex].Substring(0, i).Equals("    "))
                        {
                            lineIndex += 4;
                        }
                        else
                        {
                            return textIndex + 1;
                        }
                    }
                }
                // startIndex is beginning of text
                int startIndex = lineIndex;
                while (lineIndex < pythonText[textIndex].Length)
                {
                    // increment lineIndex until a comment, space,parentheses, colon is found
                    switch (pythonText[textIndex][lineIndex])
                    {
                        case '#':
                            return 1;
                        case ' ':
                        case '(':
                        case ':':
                            switch (pythonText[textIndex].Substring(startIndex, lineIndex))
                            {
                                case "while":
                                    // condition = send to parse while condition
                                    // while(condition)
                                    // readline(pythonText, textIndex+1, indents+1, localVariable + globalVariable)
                                    break;
                                case "for":
                                    break;
                                case "if":
                                    break;
                            }
                            break;
                        default:
                            lineIndex++;
                            break;
                    }
                }

                return textIndex + 1;
            }
            */
        }
    }
}
