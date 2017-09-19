/*
 * Created by SharpDevelop.
 * User: mahno_a_a
 * Date: 13.09.2017
 * Time: 18:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ImageAut
{
	/// <summary>
	/// Description of Aut.
	/// </summary>
	public class Aut
	{
		int blockSize;
		Bitmap bmp;
		int[,] AvgL;
		Bitmap[,] BitmapBlocks;
		Bitmap[,] LightBlocks;
        ulong[,] pHashes;

        public Aut(int blockSize, Bitmap bmp, PictureBox pic)
		{
			this.blockSize = blockSize;
			this.bmp = bmp;
			int sizeX = bmp.Width;
			int sizeY = bmp.Height;
			int blockCountX = bmp.Width / blockSize;
			int blockCountY = bmp.Height / blockSize;
			BitmapBlocks = new Bitmap[blockCountX, blockCountY];
            

            for (int i = 0; i < blockCountX; i++) 
			{
				for (int j = 0; j < blockCountY; j++) 
				{
					Rectangle cloneRect = new Rectangle(blockSize * i, blockSize * j, blockSize, blockSize);
					
					System.Drawing.Imaging.PixelFormat format =
						bmp.PixelFormat;
					Bitmap cloneBitmap = bmp.Clone(cloneRect, format);
					
					BitmapBlocks[i, j] = Helpers.ResizeImage((Image)cloneBitmap, 8, 8);
				}
			}
			
			pic.Image = bmp.Clone(new Rectangle(blockSize * 4, blockSize * 4, blockSize, blockSize), bmp.PixelFormat);
		}
		
		public Bitmap GetBlock(int i, int j)
		{
			return BitmapBlocks[i, j];
		}
		
		public void GetAverL()
		{
			AvgL = new int[BitmapBlocks.GetLength(0), BitmapBlocks.GetLength(1)];
			for (int i = 0; i<AvgL.GetLength(0); i++)
			{
				for (int j = 0; j<AvgL.GetLength(1); j++)
				{
					double sum = 0;
					for(int k = 0; k<8; k++)//(Color color in BitmapBlocks[i, j])
					{
						for(int z = 0; z<8; z++)//(Color color in BitmapBlocks[i, j])
						{
							Color color = BitmapBlocks[i, j].GetPixel(k, z);
							sum += Helpers.GetLight(color);
						}
					}
					sum /= 64;
					AvgL[i, j] = (int)sum;
				}
			}
		}
		
		public Bitmap GetLightBlocks()
		{
			LightBlocks = new Bitmap[BitmapBlocks.GetLength(0), BitmapBlocks.GetLength(1)];
			for (int i = 0; i<BitmapBlocks.GetLength(0); i++)
			{
				for (int j = 0; j<BitmapBlocks.GetLength(1); j++)
				{
					LightBlocks[i, j] = new Bitmap(8 ,8);
					for(int k = 0; k<8; k++)
					{
						for(int z = 0; z<8; z++)
						{
							if (i == 4 && j == 4)
							{
								;
							}
							if (Helpers.GetLight(BitmapBlocks[i, j].GetPixel(k, z)) > AvgL[i, j])
							{
								LightBlocks[i, j].SetPixel(k, z, Color.Black);
							}
							else
							{
								LightBlocks[i, j].SetPixel(k, z, Color.White);
							}							
						}
					}
				}
			}
			return LightBlocks[4, 4];
		}
		
		public ulong[,] GetPHashesFromLight()
        {
            pHashes = Helpers.GetPHashesFromLight(LightBlocks);
            return pHashes;
        }

	    public Bitmap InsertPHashIntoImageKutter(int sigma, double lambda)
        {
            Bitmap newImage = new Bitmap(bmp.Width, bmp.Height);
            newImage = bmp.Clone(new Rectangle(0, 0, newImage.Width, newImage.Height), bmp.PixelFormat);
            int countBlockX = BitmapBlocks.GetLength(0), 
                countBlockY = BitmapBlocks.GetLength(1);

            int[] key = Helpers.BadKeyGeneration(sigma, blockSize);

            for (int i = 0; i < countBlockX; i++)
            {
                for (int j = 0; j < countBlockY; j++)
                {
                    string hashString = Helpers.GetByteStringByHash(pHashes[i, j]);
                    for(int keyIndex = 0; keyIndex < key.Length; keyIndex++)
                    {
                        int[] currenPixel = Helpers.GetPixel(blockSize, key[keyIndex], sigma, i, j);
                        Color oldColor = newImage.GetPixel(i * blockSize + currenPixel[0], j * blockSize + currenPixel[1]);
                        double L = Helpers.GetLight(newImage.GetPixel(i * blockSize + currenPixel[0], j * blockSize + currenPixel[1]));
                        if (hashString[keyIndex] == '0')
                        {
                            //0
                            newImage.SetPixel(i * blockSize + currenPixel[0], j * blockSize + currenPixel[1], Color.FromArgb(oldColor.R, oldColor.G, Helpers.BlueOrNull(oldColor.B - lambda * L)));
                        }
                        else
                        {
                            //1
                            newImage.SetPixel(i * blockSize + currenPixel[0], j * blockSize + currenPixel[1], Color.FromArgb(oldColor.R, oldColor.G, Helpers.BlueOrNull(oldColor.B + lambda * L)));
                        }
                    }
                }
            }

            newImage.Save("image2.png");
            return newImage;
        }

        public ulong[,] ExtractFromImageKutter(int sigma)
        {
            int countBlockX = BitmapBlocks.GetLength(0),
                countBlockY = BitmapBlocks.GetLength(1);

            int[] key = Helpers.BadKeyGeneration(sigma, blockSize);

            ulong[,] result = new ulong[countBlockX, countBlockY];

            string s = "";

            for (int i = 0; i < countBlockX; i++)
            {
                for (int j = 0; j < countBlockY; j++)
                {
                    s = "";
                    ulong hash = 0;
                    for (int keyIndex = 0; keyIndex < key.Length; keyIndex++)
                    {
                        int[] currenPixel = Helpers.GetPixel(blockSize, key[keyIndex], sigma, i, j);

                        int oldColor = bmp.GetPixel(i * blockSize + currenPixel[0], j * blockSize + currenPixel[1]).B;

                        int sum = 0;
                        for(int k = 1; k<sigma+1; k++)
                        {
                            sum += bmp.GetPixel(i * blockSize + currenPixel[0]    , j * blockSize + currenPixel[1] - k).B;
                            sum += bmp.GetPixel(i * blockSize + currenPixel[0]    , j * blockSize + currenPixel[1] + k).B;
                            sum += bmp.GetPixel(i * blockSize + currenPixel[0] - k, j * blockSize + currenPixel[1]    ).B;
                            sum += bmp.GetPixel(i * blockSize + currenPixel[0] + k, j * blockSize + currenPixel[1]    ).B;
                        }
                        sum = (int)((sum)/(double)(4 * sigma));

                        if (oldColor > sum)
                        {
                            hash |= 0x01;
                            s += "1";
                        }
                        else
                        {
                            s += "0";
                        }
                        if (keyIndex != key.Length - 1)
                        {
                            hash = hash << 1;
                        }
                        else
                        {
                            ;
                        }
                    }
                    result[i, j] = hash;
                }
            }

            return result;
        }
    }
}
