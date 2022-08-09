using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using CommandLine;

namespace FilpbookMaker
{
    class Program
    {
        public class Options
        {
            [Option('i', "input", Required = true, HelpText = "")]
            public string inputFolderPath { get; set; }

            [Option('o', "output", Required = false, HelpText = "", Default = "output.png")]
            public string outputPath { get; set; }

            [Option('s', "slice", Required = false, HelpText = "", Default = 6)]
            public int slice { get; set; }

            [Option('f', "offset", Required = false, HelpText = "", Default = 0)]
            public int offset { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                Trace.Assert(o.slice > 0);
                Trace.Assert(o.inputFolderPath.Count() > 0);
                Trace.Assert(o.outputPath.Count() > 0);

                int slice = o.slice;
                int nums = slice * slice;
                string folderPath = o.inputFolderPath;
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath);
                List<string> allFiles = new List<string>();
                List<Image> images = new List<Image>();
                for (int i = 0; i < files.Count(); i++)
                {
                    if (i >= o.offset)
                    {
                        string file = files.ElementAt(i);
                        string Ext = Path.GetExtension(file);
                        if (Ext == ".png")
                        {
                            Console.WriteLine(file);
                            allFiles.Add(file);
                            images.Add(Image.FromFile(file));
                            Trace.Assert(images.Last().Size.Width == images.Last().Size.Height);
                            Trace.Assert(images.Last().PixelFormat == PixelFormat.Format32bppArgb);
                        }
                        if (images.Count >= nums)
                        {
                            break;
                        }
                    }
                }
                Trace.Assert(images.Count == nums);
                int length = images[0].Size.Width * slice;
                Bitmap canvas = new Bitmap(length, length, PixelFormat.Format32bppArgb);
                Graphics graphics = Graphics.FromImage(canvas);
                for (int i = 0; i < images.Count; i++)
                {
                    Image image = images[0];
                    int x = (i % slice) * image.Size.Width;
                    int y = (i / slice) * image.Size.Width;
                    graphics.DrawImage(images[i], x, y, image.Size.Width, image.Size.Width);
                }
                canvas.Save(o.outputPath);
                Process.Start(new FileInfo(o.outputPath).DirectoryName);
            });
        }
    }
}
