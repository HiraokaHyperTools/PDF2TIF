using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using PDF2TIF.Utils;

namespace PDF2TIF
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("PDF2TIF input.pdf output.tif");
                Console.Error.WriteLine("PDF2TIF input.pdf output{}.tif");
                Console.Error.WriteLine("PDF2TIF input.pdf output{}.png");
                Console.Error.WriteLine("PDF2TIF input.pdf output{}.jpg");
                Console.Error.WriteLine("PDF2TIF input.pdf output{}.ppm");
                return 1;
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

            bool usePageNumPlaceHolder = fpOutTiff.Contains("{}");

            string fileFormatOption = "-tiff";

            if (fpOutTiff.ToLowerInvariant().EndsWith(".png"))
            {
                fileFormatOption = "-png";

                if (!usePageNumPlaceHolder)
                {
                    Console.Error.WriteLine("png output needs \"{}\" place holder included!");
                    return 1;
                }
            }
            else if (fpOutTiff.ToLowerInvariant().EndsWith(".jpg"))
            {
                fileFormatOption = "-jpeg";

                if (!usePageNumPlaceHolder)
                {
                    Console.Error.WriteLine("jpeg output needs \"{}\" place holder included!");
                    return 1;
                }
            }
            else if (false
                || fpOutTiff.ToLowerInvariant().EndsWith(".ppm")
                || fpOutTiff.ToLowerInvariant().EndsWith(".pgm")
                || fpOutTiff.ToLowerInvariant().EndsWith(".pbm")
            )
            {
                fileFormatOption = "";

                if (!usePageNumPlaceHolder)
                {
                    Console.Error.WriteLine("ppm/pgm/pbm outputs need \"{}\" place holder included!");
                    return 1;
                }
            }

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
                            string stdErr = "";
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate { stdErr = p.StandardError.ReadToEnd(); });
                            p.WaitForExit();
                            if (p.ExitCode == 99) break;
                            if (p.ExitCode != 0)
                            {
                                Console.Error.WriteLine("pdftoppm.exe failed {0}", p.ExitCode);
                                Console.Error.WriteLine(stdErr);

                                FileUtils.DeleteFileIfExists(fpOutTiff);

                                return 1;
                            }
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
                            string stdErr = "";
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate { stdErr = p.StandardError.ReadToEnd(); });
                            p.WaitForExit();
                            if (p.ExitCode == 99) break;
                            if (p.ExitCode != 0)
                            {
                                Console.Error.WriteLine("pdftoppm.exe exited on error code {0}", p.ExitCode);
                                Console.Error.WriteLine(stdErr);

                                FileUtils.DeleteFileIfExists(fpOutTiff);

                                return 1;
                            }
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
                            psi.RedirectStandardError = true;
                            Process p = Process.Start(psi);
                            string stdErr = "";
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate { stdErr = p.StandardError.ReadToEnd(); });
                            p.WaitForExit();
                            if (p.ExitCode != 0)
                            {
                                Console.Error.WriteLine("tiffcp.exe exited on error code {0}", p.ExitCode);
                                Console.Error.WriteLine(stdErr);

                                FileUtils.DeleteFileIfExists(fpinTif);
                                FileUtils.DeleteFileIfExists(fpOutTiff);

                                return 1;
                            }
                        }

                        File.Delete(fpinTif);
                    }
                }
            }

            return 0;
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
