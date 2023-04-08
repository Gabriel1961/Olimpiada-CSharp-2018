using Olimpiada_CSharp_2018.resurse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Olimpiada_CSharp_2018
{
    public partial class eLearning1918_start : Form
    {
        bool manualMode;
        private int cidx = 0;
        public eLearning1918_start()
        {
            InitializeComponent();
            LoadImg();
            DisplayAutoMode();
            progressBar1.Maximum = imgs.Count;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        
        List<Image> imgs = new List<Image>();
        private void LoadImg()
        {
            foreach (string file in Directory.GetFiles("../../resurse/imaginislideshow"))
            {
                imgs.Add(new Bitmap(file)); 
            }
        }

        async void DisplayAutoMode()
        {
            inainteBut.Enabled = false;
            inapoiBut.Enabled = false;
            cidx = 0;
            manualMode = false;
            manualBut.Text = "Automat";
            for (; cidx < imgs.Count; cidx++)
            {
                progressBar1.Value = cidx+1;
                pictureBox1.Image = imgs[cidx];
                await Task.Delay(2000);
                if (manualMode)
                    return;
            }
        }

        void DisplayManualMode()
        {
            manualMode = true;
            manualBut.Text = "Manual";
            inainteBut.Enabled = true;
            inapoiBut.Enabled = true;
            if (cidx == imgs.Count - 1)
                inainteBut.Enabled = false;
            if (cidx == 0)
                inapoiBut.Enabled = false;
            progressBar1.Value = cidx + 1;
            pictureBox1.Image = imgs[cidx];
        }

        private void logIn_Click(object sender, EventArgs e)
        {
            var cm = new SqlCommand($"SELECT IdUtilizator,NumePrenumeUtilizator,ParolaUtilizator FROM Utilizatori WHERE EmailUtilizator=\'{emailBox.Text}\'", Program.con);
            var res = cm.ExecuteReader();
            bool noWork = false;
            if (res.HasRows)
            {
                res.Read();
                var pass = res.GetString(2);
                if (passwordBox.Text != pass.ToString())
                    noWork = true;
                else
                {
                    this.Hide();
                    int elevId = res.GetInt32(0);
                    string numePrenume = res.GetString(1);
                    res.Close();
                    eLearning1918_Elev win = new eLearning1918_Elev(elevId,numePrenume);
                    win.Show();
                    win.FormClosed += (s,ev) => { this.Visible = true; };
                }
            }
            else
                noWork = true;

            if (noWork)
            {
                emailBox.Clear();
                passwordBox.Clear();
                erMsg.Text = "Eroare de autentificare!";
            }
            else
                erMsg.Text = "";
            res.Close();
        }

        private void manualBut_Click(object sender, EventArgs e)
        {
            if (manualMode)
                DisplayAutoMode();
            else
                DisplayManualMode();
        }

        private void inapoiBut_Click(object sender, EventArgs e)
        {
            cidx--;
            DisplayManualMode();
        }

        private void inainteBut_Click(object sender, EventArgs e)
        {
            cidx++;
            DisplayManualMode();
        }
    }
}
