/*
 * Created by SharpDevelop.
 * User: mahno_a_a
 * Date: 13.09.2017
 * Time: 18:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ImageAut
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		Bitmap input;

		
		void Button1Click(object sender, EventArgs e)
		{
            int blockSize = 64;
            int sigma = 3;
            double lambda = 0.6;

            #region 1st image
            Bitmap input = new Bitmap("image.png");
			Aut a = new Aut(blockSize, input, pictureBox2);
			pictureBox1.Image = (Image) a.GetBlock(4, 4);
			a.GetAverL();
			pictureBox3.Image = (Image) a.GetLightBlocks();
            a.GetPHashesFromLight();
            pictureBox4.Image = a.InsertPHashIntoImageKutter(sigma, lambda);

            string oldP = Helpers.GetByteStringByHash(a.GetPHashesFromLight()[0, 0]);
            #endregion

            #region 2nd image
            Bitmap input2 = new Bitmap("image2.png");
            Aut b = new Aut(blockSize, input2, pictureBox2);
            //extract
            var variableOld = b.ExtractFromImageKutter(sigma);
            //getpHshes
            b.GetAverL();
            b.GetLightBlocks();
            var variable = b.GetPHashesFromLight();
            #endregion

            string newP = Helpers.GetByteStringByHash(variable[0, 0]);

            var result = Helpers.Difference(a.GetPHashesFromLight()[4, 4], variable[4, 4]);

            int[] key = Helpers.BadKeyGeneration(sigma, blockSize);
            Bitmap keySample = new Bitmap(blockSize, blockSize);
            for (int i = 0; i<key.Length; i++)
            {
                int[] helper = Helpers.GetPixel(blockSize, key[i], sigma, 0, 0);
                keySample.SetPixel(helper[0], helper[1], Color.Black);
            }
            pictureBox5.Image = keySample;
            input.Dispose();
            input2.Dispose();
        }
	}
}
