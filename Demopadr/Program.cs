using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Demopadr
{
    class Program
    {
        static void Main(string[] args)
        {
            AELogger.Prepare();
            try
            {
                Run(args);
            }
            catch (Exception e)
            {
                AELogger.Print("Exception: " + e.Message);

                AELogger.Log("Exception: " + e.StackTrace);

                int i = 1;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    AELogger.Log("InnerException " + i + ": " + e.Message);

                    AELogger.Log("InnerException " + i + ": " + e.StackTrace);
                    i++;
                }
                Console.WriteLine(e.Message);

                AELogger.Print("Got a big error, if it keeps happening,\nseek out anotak for assistance (and bring the logfile.txt)");
            }

            AELogger.WriteLog();
        } // main

        static void Run(string[] args)
        {
            AELogger.Print("demopadr - a tool by anotak to pad demo lumps");
            AELogger.Print("           for use in vanilla Doom's attract mode");
            AELogger.Print("           sequence. for more information see");
            AELogger.Print("https://doomwiki.org/wiki/Revenant_tracers_desync_internal_demos");
            AELogger.Print("");

            if (args.Length < 2)
            {
                AELogger.Print("\nNo file names specified.");
                AELogger.Print("Correct usage:");
                AELogger.Print("\tdemopadr <input> <output>");
                AELogger.Print("\t(warning: this overwrites the output file!)");
                AELogger.Print("");

                return;
            }

            if (args[0].ToLower() != "-b")
            {
                string in_filename = args[0];
                string out_filename = args[1];

                PadDemo(in_filename, out_filename);
            }
            else
            {
                if (args.Length < 3)
                {
                    AELogger.Print("\nNo directory names specified for batch mode.");
                    AELogger.Print("Correct usage:");
                    AELogger.Print("\tdemopadr -b <input> <output>");
                    AELogger.Print("\t(warning: this overwrites the output files!)");
                    AELogger.Print("");

                    return;
                }

                string dir = args[1];
                string out_dir = args[2];

                BatchDirectory(dir,out_dir);
            }
        } // run(

        static void BatchDirectory(string in_dir, string out_dir)
        {
            if (!Directory.Exists(in_dir))
            {
                AELogger.Print(in_dir + " is not a valid directory, please specify a valid directory");
                return;
            }

            if (!Directory.Exists(out_dir))
            {
                Directory.CreateDirectory(out_dir);
            }

            foreach (string in_filename in Directory.GetFiles(in_dir))
            {
                AELogger.Print("---------------------");
                string out_filename = Path.Combine(out_dir, Path.GetFileName(in_filename));
                AELogger.Print("doing " + in_filename + " to " + out_filename);

                PadDemo(in_filename, out_filename);
            }
        }
        
        static void PadDemo(string in_filename, string out_filename)
        {
            if (!File.Exists(in_filename))
            {
                AELogger.Print("File " + in_filename + " doesn't exist!");
                return;
            }

            AELogger.Print("Loading " + in_filename);

            // demo file structure is
            // 13 byte header
            // 4 bytes per tic
            // single 0x80 byte at end

            // longtics uses 5 bytes per tic


            int filesize;
            int original_num_tics;
            int target_num_tics;
            int tic_size = 4;
            byte[] output;
            using (BinaryReader reader = new BinaryReader(File.OpenRead(in_filename)))
            {
                byte version = reader.ReadByte(); // do not forget this value

                if (version == 111) // is longtics?
                {
                    tic_size = 5;
                    AELogger.Print("longtics demo detected");
                }
                else
                {
                    tic_size = 4;
                }

                filesize = (int)reader.BaseStream.Length;

                byte[] input = reader.ReadBytes(filesize - 1);

                int orig_footer_ptr = 0;

                for (int i = 12; i < filesize - 1; i += tic_size)
                {
                    if (input[i] == 0x80)
                    {
                        orig_footer_ptr = i + 1;
                        AELogger.Log("file ends at " + orig_footer_ptr);
                        break; // for
                    }
                }

                int footer_size = 0;

                if (orig_footer_ptr == 0)
                {
                    AELogger.Print("warning, 0x80 not found, attempting to proceed anyway.");
                    AELogger.Print("this may be an error (?)");
                    orig_footer_ptr = filesize - 1;
                    original_num_tics = (filesize - 14) / tic_size;
                }
                else
                {
                    if (orig_footer_ptr + 1 < filesize)
                    {
                        footer_size = filesize - (orig_footer_ptr + 1);
                        AELogger.Print("preserving footer with size " + footer_size);
                    }
                    original_num_tics = (orig_footer_ptr - 13) / tic_size;
                }

                AELogger.Print("original_num_tics: " + original_num_tics);

                // determine padding
                if (original_num_tics % 4 == 0)
                {
                    target_num_tics = original_num_tics;
                }
                else
                {
                    target_num_tics = (original_num_tics) + (4 - (original_num_tics % 4));
                }

                AELogger.Print("target_num_tics: " + target_num_tics);

                int tgt_footer_ptr = 14 + (target_num_tics * tic_size);

                output = new byte[tgt_footer_ptr + footer_size];

                output[0] = version;

                AELogger.Log("filesize " + filesize + " vs output size " + output.Length);
                AELogger.Log("doing main copy");
                Array.Copy(input, 0, output, 1, orig_footer_ptr-1);
                
                if (footer_size > 0)
                {
                    AELogger.Log("doing footer copy w size " + footer_size);
                    Array.Copy(input, orig_footer_ptr, output, tgt_footer_ptr, footer_size);
                }

                AELogger.Log("writing 0x80 @ " + (tgt_footer_ptr - 1));
                output[tgt_footer_ptr - 1] = 0x80; // final ending byte
            } // using

            AELogger.Print("writing to temp file DEMOPADR.TMP");
            if (File.Exists("DEMOPADR.TMP"))
            {
                File.Delete("DEMOPADR.TMP");
            }

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite("DEMOPADR.TMP")))
            {
                writer.Write(output);
            }

            if (File.Exists(out_filename))
            {
                AELogger.Print("copying temp file over existing " + out_filename);
                File.Delete(out_filename);
            }
            else
            {
                AELogger.Print("writing new " + out_filename);
            }

            File.Copy("DEMOPADR.TMP", out_filename);

            try
            {
                if (File.Exists("DEMOPADR.TMP"))
                {
                    File.Delete("DEMOPADR.TMP");
                }
            }
            catch (Exception e)
            {
                AELogger.Print("warning: there was some kind of error deleting DEMOPADR.TMP");
                AELogger.Print("check Logfile.txt for more information?");

                AELogger.Log("Exception: " + e.Message);

                AELogger.Log("Exception: " + e.StackTrace);

                int i = 1;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    AELogger.Log("InnerException " + i + ": " + e.Message);

                    AELogger.Log("InnerException " + i + ": " + e.StackTrace);
                    i++;

                }
            } 

        } // paddemo(

    } // class
} // ns
