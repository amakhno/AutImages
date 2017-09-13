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
			input = new Bitmap("image.png");
			Aut a = new Aut(64, input, pictureBox2);
			pictureBox1.Image = (Image) a.GetBlock(4, 4);
			pictureBox1.Refresh();
			a.GetAverL();
			pictureBox3.Image = (Image) a.GetLightBlocks();
		}
	}
}
