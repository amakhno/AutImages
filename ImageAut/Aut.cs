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
		public int blockSize;
		public Bitmap bmp;
		public Color[,] colors;
		public int[,] AvgL;
		public Bitmap[,] BitmapBlocks;
		public Bitmap[,] LightBlocks;
		
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
					
					BitmapBlocks[i, j] = ResizeImage((Image)cloneBitmap, 8, 8);
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
							sum += GetLight(color);
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
							if (GetLight(BitmapBlocks[i, j].GetPixel(k, z)) > AvgL[i, j])
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
		
		private static double GetLight(Color color)
		{
			return 0.3 * color.R + 0.6 * color.G + 0.1 * color.B;
		}
		
		private static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage)) {
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes()) {
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
	
		
	
	
	}
}
