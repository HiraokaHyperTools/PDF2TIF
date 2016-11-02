using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using FreeImageAPI;

namespace PDF2TIF {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 2) {
                Console.Error.WriteLine("PDF2TIF input.pdf output.tif");
                Environment.Exit(1);
            }
            String fppdf = args[0];
            String fptiff = args[1];
            String fpprefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            String fppng = fpprefix + ".png";
            String fppbm = fpprefix + ".pbm";
            String fppgm = fpprefix + ".pgm";
            int bpp;
            if (!int.TryParse("" + Environment.GetEnvironmentVariable("PDF2TIF_BPP"), out bpp)) {
                bpp = 24;
            }
            bool mono = bpp == 1;
            bool gray = bpp == 8;
            {
                for (int y = 1; ; y++) {
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPL\\pdftoppm.exe")
                            , " -f " + y + " -singlefile -r 300 " + (mono ? "-mono" : gray ? "-gray" : "-png") + " \"" + fppdf + "\" \"" + fpprefix + "\" ");
                        psi.UseShellExecute = false;
                        psi.RedirectStandardError = true;
                        Process p = Process.Start(psi);
                        System.Threading.ThreadPool.QueueUserWorkItem(delegate { p.StandardError.ReadToEnd(); });
                        p.WaitForExit();
                        if (p.ExitCode == 99) break;
                    }

                    String fpinTif = fpprefix + ".tif";
                    FIBITMAP dib = mono
                        ? FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PBM, fppbm, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
                        : gray
                            ? FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PGM, fppgm, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
                            : FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, fppng, FREE_IMAGE_LOAD_FLAGS.DEFAULT)
                        ;
                    File.Delete(mono ? fppbm : gray ? fppgm : fppng);
                    try {
                        FIMULTIBITMAP tif = FreeImage.OpenMultiBitmap(FREE_IMAGE_FORMAT.FIF_TIFF, fpinTif, true, false, true, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
                        try {
                            FreeImage.SetResolutionX(dib, 300);
                            FreeImage.SetResolutionY(dib, 300);
                            FreeImage.AppendPage(tif, dib);
                        }
                        finally {
                            FreeImage.CloseMultiBitmap(tif, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
                        }
                    }
                    finally {
                        FreeImage.Unload(dib);
                    }

                    {
                        ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GPL\\tiffcp.exe")
                            , " " + ((y != 1) ? "-a" : "") + " \"" + fpinTif + "\" \"" + fptiff + "\" ");
                        psi.UseShellExecute = false;
                        Process p = Process.Start(psi);
                        p.WaitForExit();
                        if (p.ExitCode != 0) {
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
