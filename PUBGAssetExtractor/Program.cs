using System;
using System.Diagnostics;
using System.IO;

namespace PUBGAssetExtractor
{
    public class Program
    {
        public static string CurrentDirectory = Directory.GetCurrentDirectory();

        public static string Quickbms = CurrentDirectory + "\\quickbms.exe";

        public static string UE4 = CurrentDirectory + "\\UE4.bms";

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("[+] PUBG Asset extractor");
                
                //Ensure insage is correct
                if (args.Length != 1)
                {
                    Console.WriteLine($"[-] Invalid usage ({args.Length} args passed)");
                    Console.ReadKey();
                    return;
                }
                
                //Ensure input path is correct
                string PUBG = args[0];
                if (!Directory.Exists(PUBG) || !PUBG.EndsWith("\\PUBG"))
                {
                    Console.WriteLine($"[-] Invalid path '{PUBG}'");
                    Console.ReadKey();
                    return;
                }

                string Common = Directory.GetParent(PUBG).FullName;
                string Content = $"{PUBG}\\TslGame\\Content";
                string Paks = $"{Content}\\Paks";
                string Output = $"{Common}\\PUBG Dump";

                Console.WriteLine($"[*] Dumping '{Paks}'");
                Console.WriteLine($"[*] To '{Output}'");

                //Drop and create output directory
                if (Directory.Exists(Output))
                    Directory.Delete(Output, true);
                
                Directory.CreateDirectory(Output);

                //Release quickbms
                if (!File.Exists(Quickbms))
                    File.WriteAllBytes(Quickbms, Properties.Resources.quickbms);

                //Release quickbms script
                if (!File.Exists(UE4))
                    File.WriteAllBytes(UE4, Properties.Resources.UE4);

                //Get all .pak files in the Paks directory
                foreach (string file_path in Directory.GetFiles(Paks, "*.pak", SearchOption.TopDirectoryOnly))
                {
                    //Get the file name
                    string file_name = Path.GetFileName(file_path);
                    Console.WriteLine($"[*] Processing {file_name}");
                    
                    //Create the output directory
                    string file_output = Output + "\\" + file_name;
                    Directory.CreateDirectory(file_output);
                    
                    //Create the process
                    Process p = new Process();
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.FileName = Quickbms;
                    p.StartInfo.Arguments = $"\"{UE4}\" \"{file_path}\" \"{file_output}\"";
                    p.Start();
                    p.WaitForExit();
                    
                    //Check exit code (must be 0 if succeed)
                    if (p.ExitCode != 0)
                        Console.WriteLine($"[-] Extraction failed");
                }

                Console.WriteLine("[+] Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-] Exception: " + ex);
               
            }
            finally
            {
                //Remove quickbms
                if (File.Exists(Quickbms))
                    File.Delete(Quickbms);

                //Remove quickbms script
                if (File.Exists(UE4))
                    File.Delete(UE4);
            }

            Console.ReadLine();
        }
    }
}
