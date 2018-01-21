using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;
namespace Demopadr
{
    public class AELogger
    {
        static List<String> Logfile = new List<string>(2048);

        public static bool bLogging = true;
        public static bool bPrintAll = false;

        private static StringBuilder sbuilder = new StringBuilder(1024);

        private static CultureInfo culture = CultureInfo.InvariantCulture;
        public static Stopwatch stopwatch;

#if DEBUG
        public static bool bPrepared = false;
#endif

        public static void Prepare()
        {
#if DEBUG
            bPrepared = true;
#endif
            stopwatch = new Stopwatch();
            stopwatch.Start();
            Log("logging started @ " + DateTime.Now.ToString(@"HH:mm:ss.fff", culture));
            Log("working directory " + Directory.GetCurrentDirectory());
            Log("build time: " + GetCompileDate());
#if DEBUG
            Log("WARNING: THIS IS A DEBUG BUILD");
#endif
        } // prepare()

        public static void Log(string message, Boolean time = true, Boolean print = false)
        {
#if DEBUG
            if (!bPrepared)
            {
                throw new Exception("AELogger.Prepare() not called yet!!!!!");
            }
#endif
            if (bLogging)
            {
                if (time)
                {
                    // on .NET 4.0 we could
                    //sbuilder.Clear();
                    //sbuilder.Append(stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff", culture));

                    // but instead we must
                    sbuilder.Length = 0;
                    TimeSpan span = stopwatch.Elapsed;
                    sbuilder.Append(
                        String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                            span.TotalHours,
                            span.TotalMinutes % 60.0,
                            span.TotalSeconds % 60.0,
                            span.TotalMilliseconds % 1000.0)
                        );


                    sbuilder.Append(": ");
                    sbuilder.Append(message);

                    Logfile.Add(sbuilder.ToString());

                    if (print || bPrintAll)
                    {
                        Console.WriteLine(sbuilder.ToString());
                    }
                }
                else
                {
                    Logfile.Add(message);
                    if (print || bPrintAll)
                    {
                        Console.WriteLine(message);
                    }
                }

            }
        } // log string version

        // STRINGBUILDER VERSION
        public static void Log(StringBuilder inbuilder, Boolean time = true, Boolean print = false)
        {
#if DEBUG
            if (!bPrepared)
            {
                throw new Exception("AELogger.Prepare() not called yet!!!!!");
            }
#endif
            if (bLogging)
            {
                if (time)
                {
                    // on .NET 4.0 we could
                    //sbuilder.Clear();
                    //sbuilder.Append(stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff", culture));

                    // but instead we must
                    sbuilder.Length = 0;
                    TimeSpan span = stopwatch.Elapsed;
                    sbuilder.Append(
                        String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                            span.TotalHours,
                            span.TotalMinutes % 60.0,
                            span.TotalSeconds % 60.0,
                            span.TotalMilliseconds % 1000.0)
                        );


                    sbuilder.Append(": ");
                    sbuilder.Append(inbuilder);

                    string message = sbuilder.ToString();
                    Logfile.Add(message);

                    if (print || bPrintAll)
                    {
                        Console.WriteLine(message);
                    }
                }
                else
                {
                    string message = inbuilder.ToString();
                    Logfile.Add(message);
                    if (print || bPrintAll)
                    {
                        Console.WriteLine(message);
                    }
                }

            }
        } // log stringbuilder

        public static void Print(string message)
        {
            Log(message, false, true);
        }

        public static string GetCompileDate()
        {
            System.Version MyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return new DateTime(2000, 1, 1).AddDays(MyVersion.Build).AddSeconds(MyVersion.Revision * 2).ToString();
        }

        public static void WriteLog(string filename = "")
        {
            if (bLogging)
            {
                if (filename == "")
                {
                    filename = System.Reflection.Assembly.GetCallingAssembly().GetName().Name + "-logfile.txt";
                }

                using (StreamWriter SW = File.CreateText(filename))
                {
                    int count = Logfile.Count;
                    for (int i = 0; i < count; i++)
                    {
                        SW.WriteLine(Logfile[i]);
                    }
                    SW.WriteLine("Logfile written @ " + DateTime.Now.ToString(@"HH:mm:ss.fff", culture));
                } // closed
            }
        } // writelog

    } // class
} // ns
