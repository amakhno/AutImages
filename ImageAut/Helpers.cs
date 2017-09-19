using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageAut
{
    static class Helpers
    {
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static double GetLight(Color color)
        {
            return 0.3 * color.R + 0.6 * color.G + 0.1 * color.B;
        }

        /// <summary>Получает набор pHash
        /// <para>Да да. <see cref="System.Console.WriteLine(System.String)"/> for information about output statements.</para>
        /// <seealso cref="TestClass.Main"/>
        /// </summary>
        public static ulong[,] GetPHashesFromLight(Bitmap[,] LightBlocks)
        {
            int countBlockX = LightBlocks.GetLength(0),
                countBlockY = LightBlocks.GetLength(1);

            ulong[,] pHashes = new ulong[countBlockX, countBlockY];

            for (int i = 0; i < countBlockX; i++)
            {
                for (int j = 0; j < countBlockY; j++)
                {
                    ulong cache = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        for (int z = 0; z < 8; z++)
                        {
                            if (LightBlocks[i, j].GetPixel(k, z) == Color.FromArgb(255, 0, 0, 0))
                            {
                                cache |= 0x01;
                            }
                            if (k != 7 || z != 7)
                            {
                                cache = cache << 1;
                            }
                        }
                    }

                    pHashes[i, j] = cache;
                }
            }

            return pHashes;
        }

        public static string GetByteStringByHash(ulong hash)
        {
            string result = "";
            for (int x = 63; x > -1; x--)
            {
                if ((hash & (1UL << (x))) == 0)
                {
                    result += "0";
                }
                else
                {
                    result += "1";
                }
            }
            return result;
        }

        public static int[] NewKeyGeneration(int sigma, int blockSize)
        {
            //ключ - каждый второй?
            int step = (int)Math.Floor((double)((blockSize - 2 * sigma) * (blockSize - 2 * sigma)) / (double)(8 * 8));

            if (step < 2)
            {
                throw new Exception("Блоки слишком маленького размера");
            }

            int[] key = new int[64];

            key[0] = 0;
            for (int i = 1; i < key.Length; i++)
            {
                key[i] = key[i - 1] + (int)(step);
            }

            return key;
        }

        public static int[] BadKeyGeneration(int sigma, int blockSize)
        {
            //ключ - каждый второй?
            int stepX = (int)Math.Floor((double)((blockSize - 2 * sigma) * (blockSize - 2 * sigma)) / (double)(8 * 8));
            int subStep = 2 * sigma;

            int smallBlockSize = blockSize - 2 * sigma;

            if (stepX < 2)
            {
                throw new Exception("Блоки слишком маленького размера");
            }

            int[] key = new int[64];

            int index = 0;
            for (int i = 0; i < blockSize - 2 * sigma; i+= subStep)
            {
                if (index == 64)
                {
                    break;
                }
                for (int j = 0; j < blockSize - 2 * sigma; j += subStep)
                {
                    if (index == 64)
                    {
                        break;
                    }
                    key[index] = j + (blockSize - 2 * sigma) * i;
                    index++;
                }
            }

            return key;
        }

        internal static int[] GetPixel(int blockSize, int v, int sigma, int i, int j)
        {
            int newBlockSize = blockSize - 2 * sigma;
            int[] a = new int[2];
            a[1] = v / newBlockSize + sigma;
            a[0] = v % newBlockSize + sigma;
            return a;
        }

        internal static int BlueOrNull(double col)
        {
            if (col < 0)
            {
                return 0;
            }
            if (col > 255)
            {
                return 255;
            }
            else
            {
                return (int)col;
            }
        }

        internal static int Difference(ulong a, ulong b)
        {
            ulong temp = a ^ b;
            int count = 0;
            while (temp > 0)
            {
                if ((temp & 0x01) == 1)
                {
                    count++;
                }
                temp = temp >> 1;
            }
            return count;
        }
    }
}
