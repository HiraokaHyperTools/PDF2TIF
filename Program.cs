using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PDF2TIF
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("PDF2TIF input.pdf output.tif");
                Console.Error.WriteLine("PDF2TIF input.pdf output{}.tif");
                Console.Error.WriteLine("PDF2TIF input.pdf output{}.png");
                Console.Error.WriteLine("PDF2TIF input.pdf output{}.jpg");
                Console.Error.WriteLine("PDF2TIF input.pdf output{}.ppm");
                Environment.Exit(1);
            }
            String fppdf = args[0];
            String fpOutTiff = args[1];
            String fpprefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            int bpp;
            if (!int.TryParse("" + Environment.GetEnvironmentVariable("PDF2TIF_BPP"), out bpp))
            {
                bpp = 24;
            }

            int dpi;
            if (!int.TryParse("" + Environment.GetEnvironmentVariable("PDF2TIF_DPI"), out dpi))
            {
                dpi = 300;
            }

            string fileFormatOption = "-tiff";

            if (fpOutTiff.ToLowerInvariant().EndsWith(".png"))
            {
                fileFormatOption = "-png";
            }
            else if (fpOutTiff.ToLowerInvariant().EndsWith(".jpg"))
            {
                fileFormatOption = "-jpeg";
            }
            else if (fpOutTiff.ToLowerInvariant().EndsWith(".ppm"))
            {
                fileFormatOption = "";
            }

            bool usePageNumPlaceHolder = fpOutTiff.Contains("{}");

            bool mono = bpp == 1;
            bool gray = bpp == 8;
            {
                for (int y = 1; ; y++)
                {
                    if (usePageNumPlaceHolder)
                    {
                        {
                            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPL\\pdftoppm.exe"), String.Concat(""
                                , " ", fileFormatOption
                                , " -f ", y.ToString()
                                , " -singlefile"
                                , " -r " + dpi
                                , " ", (mono ? "-mono" : gray ? "-gray" : "")
                                , " \"", fppdf, "\""
                                , " \"", ExcludeExtension(fpOutTiff).Replace("{}", y.ToString()), "\""
                                , " "
                                ));
                            psi.UseShellExecute = false;
                            psi.RedirectStandardError = true;
                            Process p = Process.Start(psi);
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate { p.StandardError.ReadToEnd(); });
                            p.WaitForExit();
                            if (p.ExitCode == 99) break;
                        }
                    }
                    else
                    {
                        {
                            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPL\\pdftoppm.exe"), String.Concat(""
                                , " ", fileFormatOption
                                , " -f ", y.ToString()
                                , " -singlefile"
                                , " -r " + dpi
                                , " ", (mono ? "-mono" : gray ? "-gray" : "")
                                , " \"", fppdf, "\""
                                , " \"", fpprefix, "\""
                                , " "
                                ));
                            psi.UseShellExecute = false;
                            psi.RedirectStandardError = true;
                            Process p = Process.Start(psi);
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate { p.StandardError.ReadToEnd(); });
                            p.WaitForExit();
                            if (p.ExitCode == 99) break;
                        }

                        String fpinTif = fpprefix + ".tif";

                        {
                            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPL\\tiffcp.exe"), String.Concat(""
                                , " ", ((y != 1) ? "-a" : "")
                                , " -c ", (mono ? "g4" : gray ? "lzw" : "lzw")
                                , " \"" + fpinTif + "\""
                                , " \"" + fpOutTiff + "\" "
                                ));
                            psi.UseShellExecute = false;
                            Process p = Process.Start(psi);
                            p.WaitForExit();
                            if (p.ExitCode != 0)
                            {
                                if (File.Exists(fpinTif))
                                {
                                    File.Delete(fpinTif);
                                }
                                Environment.ExitCode = 1;
                                return;
                            }
                        }

                        File.Delete(fpinTif);
                    }
                }
            }
        }

        static string ExcludeExtension(string filePath)
        {
            int pos = filePath.LastIndexOfAny("\\/.".ToCharArray());
            if (pos >= 0 && filePath[pos] == '.')
            {
                return filePath.Substring(0, pos);
            }
            return filePath;
        }
    }
}
