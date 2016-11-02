using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PDF2TIF {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 2) {
                Console.Error.WriteLine("PDF2TIF input.pdf output.tif");
                Environment.Exit(1);
            }
            String fppdf = args[0];
            String fpOutTiff = args[1];
            String fpprefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            int bpp;
            if (!int.TryParse("" + Environment.GetEnvironmentVariable("PDF2TIF_BPP"), out bpp)) {
                bpp = 24;
            }

            int dpi;
            if (!int.TryParse("" + Environment.GetEnvironmentVariable("PDF2TIF_DPI"), out dpi)) {
                dpi = 300;
            }

            bool mono = bpp == 1;
            bool gray = bpp == 8;
            {
                for (int y = 1; ; y++) {
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPL\\pdftoppm.exe"), String.Concat(""
                            , " -tiff"
                            , " -tiffcompression deflate"
                            , " -f ", y.ToString()
                            , " -singlefile"
                            , " -r 300"
                            , " ", (mono ? "-mono" : gray ? "-gray" : "")
                            , " \"", fppdf, "\""
                            , " \"", fpprefix, "\""
                            , " ")
                            );
                        psi.UseShellExecute = false;
                        psi.RedirectStandardError = true;
                        Process p = Process.Start(psi);
                        System.Threading.ThreadPool.QueueUserWorkItem(delegate { p.StandardError.ReadToEnd(); });
                        p.WaitForExit();
                        if (p.ExitCode == 99) break;
                    }

                    String fpinTif = fpprefix + ".tif";

                    {
                        ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPL\\tiffcp.exe")
                            , " " + ((y != 1) ? "-a" : "") + " \"" + fpinTif + "\" \"" + fpOutTiff + "\" ");
                        psi.UseShellExecute = false;
                        Process p = Process.Start(psi);
                        p.WaitForExit();
                        if (p.ExitCode != 0) {
                            if (File.Exists(fpinTif)) {
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
}
