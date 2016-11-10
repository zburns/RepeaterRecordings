using System;
using System.Collections.Generic;
using System.Text;

namespace RepeaterRecordings
{
    class Program
    {
        static private string SaveDirectory;
        static private string FeedNumber;
        static private System.DateTime StartDate;
        static private System.DateTime EndDate;

        static void Main(string[] args)
        {
            int hoursfound = 0;

            SaveDirectory = "C:\\Users\\Zachary Burns\\Desktop\\testdir";
            FeedNumber = "11405";

            Console.Write("Enter Start Date: ");
            //StartDate = System.DateTime.Parse(Console.ReadLine());
            StartDate = System.DateTime.Parse("04/21/2012");
            Console.Write("Enter End Date: ");
            //EndDate = System.DateTime.Parse(Console.ReadLine());
            EndDate = System.DateTime.Now;
            Console.WriteLine();
            
            //BEGIN THE SEARCH
            if (System.IO.Directory.Exists(SaveDirectory))
            {
                while (StartDate < EndDate)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("Searching: " + StartDate.ToString("dddd - MMM dd, yyyy"));
                    string[] FileListing = System.IO.Directory.GetFiles(SaveDirectory,StartDate.ToString("yyyyMMdd") + "*-*-" + FeedNumber + ".mp3",System.IO.SearchOption.TopDirectoryOnly);
                    if (FileListing.Length != 0)
                    {
                        for (int Hour = 0; Hour <= 23; Hour++)
                        {
                            hoursfound = 0;
                            foreach (string File in FileListing)
                            {
                                if (File.Substring(File.LastIndexOf('\\') + 9, 2) == Hour.ToString().PadLeft(2, '0'))
                                {
                                    hoursfound++;
                                    Console.WriteLine();
                                    for (int Minute = 0; Minute <= 59; Minute++)
                                    {
                                        if (File.Substring(File.LastIndexOf('\\') + 11, 2) == Minute.ToString().PadLeft(2, '0'))
                                        {
                                            Console.ForegroundColor = ConsoleColor.White;
                                            Console.BackgroundColor = ConsoleColor.Green;
                                            Console.Write(Hour.ToString().PadLeft(2, '0') + ":" + Minute.ToString().PadLeft(2, '0'));
                                            Console.ForegroundColor = ConsoleColor.White;
                                            Console.BackgroundColor = ConsoleColor.Black;
                                            Console.Write("\t");
                                            try
                                            {
                                                System.IO.File.Move(File, File.Substring(0, File.LastIndexOf('\\') + 1) + StartDate.ToString("MM-dd-yyyy-") + Hour.ToString().PadLeft(2, '0') + Minute.ToString().PadLeft(2, '0') + ".mp3");
                                            }
                                            catch
                                            {
                                                Console.ForegroundColor = ConsoleColor.Cyan;
                                                Console.BackgroundColor = ConsoleColor.Black;
                                                Console.WriteLine("Duplicate File.");
                                            }
                                        }
                                    }
                                }
                            }
                            if (hoursfound < 2) //radio reference records 2 30 minute recordings
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.WriteLine("Missing a Recording!");
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("Missing Entire Day!");
                    }
                    //Go To Next Day
                    StartDate = StartDate.AddDays(1);
                }
            }
            else
            {
                Console.WriteLine("Missing Save Directory");
            }
            Console.ReadLine();
        }
    }
}
