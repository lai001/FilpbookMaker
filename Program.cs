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

            [Option('c', "column", Required = false, HelpText = "", Default = 5)]
            public int column { get; set; }

            [Option('r', "row", Required = false, HelpText = "", Default = 5)]
            public int row { get; set; }

            [Option('f', "offset", Required = false, HelpText = "", Default = 0)]
            public int offset { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                Trace.Assert(o.column > 0);
                Trace.Assert(o.row > 0);
                Trace.Assert(o.inputFolderPath.Count() > 0);
                Trace.Assert(o.outputPath.Count() > 0);

                int nums = o.column * o.row;
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
                            Image image = Image.FromFile(file);
                            if (images.Count > 0)
                            {
                                Trace.Assert(images.Last().Size.Width == image.Size.Width);
                                Trace.Assert(images.Last().Size.Height == image.Size.Height);
                            }
                            images.Add(image);
                            Trace.Assert(images.Last().PixelFormat == PixelFormat.Format32bppArgb);
                        }
                        if (images.Count >= nums)
                        {
                            break;
                        }
                    }
                }
                Trace.Assert(images.Count == nums);
                Bitmap canvas = new Bitmap(images[0].Size.Width * o.column, images[0].Size.Height * o.row, PixelFormat.Format32bppArgb);
                Graphics graphics = Graphics.FromImage(canvas);

                for (int r = 0; r < o.row; r++)
                {
                    for (int c = 0; c < o.column; c++)
                    {
                        Image image = images[0];
                        int i = o.column * r + c;
                        int x = c * image.Size.Width;
                        int y = r * image.Size.Height;
                        graphics.DrawImage(images[i], x, y, image.Size.Width, image.Size.Width);
                    }
                }

                canvas.Save(o.outputPath);
                Process.Start(new FileInfo(o.outputPath).DirectoryName);
            });
        }
    }
}
