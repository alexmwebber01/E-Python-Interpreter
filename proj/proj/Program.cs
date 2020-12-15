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


            // Open the stream and read it back.
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    Console.WriteLine(temp.GetString(b));
                }
            }
        }
	}
}
