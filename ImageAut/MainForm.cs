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
        Bitmap input2;


        void Button1Click(object sender, EventArgs e)
		{
            int blockSize = 0;
            int sigma = 0;
            double lambda = 0;
            try
            {
                sigma = Convert.ToInt32(textBox1.Text);
                blockSize  = Convert.ToInt32(textBox2.Text);
                lambda = Convert.ToDouble(textBox3.Text.Replace(".", ","));
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }

            #region 1st image
            input?.Dispose();
            input2?.Dispose();
            input = new Bitmap("image.png");
			Aut a = new Aut(blockSize, input, pictureBox2);
			pictureBox1.Image = (Image) a.GetBlock(3, 3);
			a.GetAverL();
			pictureBox3.Image = (Image) a.GetLightBlocks();
            a.GetPHashesFromLight();
            pictureBox4.Image = a.InsertPHashIntoImageKutter(sigma, lambda);
            #endregion

            #region 2nd image
            input2?.Dispose();
            input2 = new Bitmap("image2.png");
            pictureBox1.Image = input;
            Aut b = new Aut(blockSize, input2, pictureBox2);            
            //extract
            var variableOld = b.ExtractFromImageKutter(sigma);
            //getpHshes
            b.GetAverL();
            b.GetLightBlocks();
            var variable = b.GetPHashesFromLight();
            #endregion

            int[] key = Helpers.BadKeyGeneration(sigma, blockSize);
            Bitmap keySample = new Bitmap(blockSize, blockSize);
            for (int i = 0; i<key.Length; i++)
            {
                int[] helper = Helpers.GetPixel(blockSize, key[i], sigma, 0, 0);
                keySample.SetPixel(helper[0], helper[1], Color.Black);
            }
            pictureBox5.Image = keySample;

            label1.Text = "";
            for(int i= 0; i<variable.GetLength(0); i++)
            {
                for (int j = 0; j < variable.GetLength(1); j++)
                {
                    try
                    {
                        label1.Text += Helpers.Difference(variable[i, j], variableOld[i, j]).ToString() + " " + " ";
                    }
                    catch(Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                        return;
                    }
                }
                label1.Text += "\n\n";
            }

            //input.Dispose();
            //input2.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int blockSize = 0;
            int sigma = 0;
            double lambda = 0;
            try
            {
                sigma = Convert.ToInt32(textBox1.Text);
                blockSize = Convert.ToInt32(textBox2.Text);
                lambda = Convert.ToDouble(textBox3.Text.Replace(".", ","));
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    input?.Dispose();
                    input = new Bitmap(openFileDialog1.FileName);
                }
                catch(Exception exc)
                {
                    MessageBox.Show("Input Error!");
                    return;
                }
            }
            else
            {
                return;
            }
            pictureBox1.Image = input;

            Aut b = new Aut(blockSize, input, pictureBox2);
            //extract
            var variableOld = b.ExtractFromImageKutter(sigma);
            //getpHshes
            b.GetAverL();
            b.GetLightBlocks();
            var variable = b.GetPHashesFromLight();

            int[] key = Helpers.BadKeyGeneration(sigma, blockSize);
            Bitmap keySample = new Bitmap(blockSize, blockSize);
            for (int i = 0; i < key.Length; i++)
            {
                int[] helper = Helpers.GetPixel(blockSize, key[i], sigma, 0, 0);
                keySample.SetPixel(helper[0], helper[1], Color.Black);
            }
            pictureBox5.Image = keySample;

            label1.Text = "";
            for (int i = 0; i < variable.GetLength(0); i++)
            {
                for (int j = 0; j < variable.GetLength(1); j++)
                {
                    try
                    {
                        label1.Text += Helpers.Difference(variable[i, j], variableOld[i, j]).ToString() + " " + " ";
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                        return;
                    }
                }
                label1.Text += "\n\n";
            }
        }
    }
}
