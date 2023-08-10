using System;
using System.Collections.Generic;
using System.IO.Filesystem.Ntfs;
using System.IO;
using System.Security.Principal;

public class Program
{ 
    public static void Main()
    {
        Console.Title = "BestDuplicatesRemover | Created by https://github.com/GabryB03/";

        if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator))
        {
            Console.WriteLine("Please, run the program with Administrator privileges.");
            Console.WriteLine("Press the ENTER key in order to exit from the program.");
            Console.ReadLine();
            return;
        }

        string selectedOption = "";

        while (selectedOption != "1" && selectedOption != "2")
        {
            Console.WriteLine("When duplicates files are found, do you want to remove both (1) or only one (2)?");
            selectedOption = Console.ReadLine();

            if (selectedOption != "1" && selectedOption != "2")
            {
                Console.WriteLine("Invalid answer provided. Please, try again");
            }
        }

        bool removeBoth = selectedOption == "1";
        Console.WriteLine("Analyzing all your drives in order to fetch all the files existing in the system, please wait some minutes.");
        List<INode> allNodes = new List<INode>();

        foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
        {
            DriveInfo driveToAnalyze = new DriveInfo(driveInfo.Name[0].ToString());
            NtfsReader ntfsReader = new NtfsReader(driveToAnalyze, RetrieveMode.Minimal);
            IEnumerable<INode> nodes = ntfsReader.GetNodes(driveToAnalyze.Name);
            allNodes.AddRange(nodes);
        }

        Console.WriteLine("Successfully fetched all files from all the drives.");
        Console.WriteLine("Duplicates removing process is now started.");
        Console.WriteLine("Please wait some minutes.");

        foreach (INode node1 in allNodes)
        {
            foreach (INode node2 in allNodes)
            {
                if (node1.NodeIndex != node2.NodeIndex)
                {
                    if (node1.Name.ToLower() == node2.Name.ToLower() && node1.Size == node2.Size)
                    {
                        try
                        {
                            if (removeBoth)
                            {
                                File.Delete(node1.FullName);
                                File.Delete(node2.FullName);
                            }
                            else
                            {
                                File.Delete(node2.FullName);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }

            Console.WriteLine(node1.FullName);
        }

        Console.WriteLine("Duplicates removing process finished successfully.");
        Console.WriteLine("Press the ENTER key in order to exit from the program.");
        Console.ReadLine();
    }
}