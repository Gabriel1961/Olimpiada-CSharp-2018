using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Schema;

namespace Olimpiada_CSharp_2018.resurse
{
    public partial class eLearning1918_Elev : Form
    {
        class Item
        {
            public int TipItem;
            public string[] RaspunsItem;
            public string EnuntItem, RaspunsCorectItem;
        }
        List<Item> items = new List<Item>();
        List<RadioButton> rbs = new List<RadioButton>();
        List<CheckBox> cbs = new List<CheckBox>();
        class NotaEntry
        {
            public int Nota { get; set; }
            public DateTime Data { get; set; }
        }
        List<NotaEntry> note = new List<NotaEntry>();
        Item citem;

        int punctaj = 1;

        int idElev;
        string numePrenumeElev;
        void GetNote()
        {
            SqlCommand cmd = new SqlCommand($"SELECT NotaEvaluare,DataEvaluare FROM Evaluari WHERE IdElev={idElev}", Program.con);
            var res = cmd.ExecuteReader();
            note.Clear();
            while (res.Read())
            {
                NotaEntry rec = new NotaEntry(){ Nota = res.GetInt32(0), Data = res.GetDateTime(1)};
                note.Add(rec);
            }
            res.Close();
            dataGridView1.DataSource = note;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PlotGraph();
        }

        void PlotGraph()
        {
            var ser = chart1.Series[0];
            ser.Name = "Note";
            ser.ChartType = SeriesChartType.Line;
            ser.BorderWidth = 4;
            ser.Points.Clear();
            note = note.OrderBy((n) => n.Data).ToList();
            foreach (var gr in note)
            {
                ser.Points.AddXY((gr.Data - note[0].Data).Days,gr.Nota);
            }
        }

        public eLearning1918_Elev(int idElev,string numePrenumeElev)
        {
            InitializeComponent();
            cbs = new List<CheckBox>() { checkBox1, checkBox2, checkBox3, checkBox4 };
            rbs = new List<RadioButton>() { radioButton1, radioButton2, radioButton3, radioButton4 };
            HideButtons();
            carnetLab.Text = "Carnetul de note al elevului " + numePrenumeElev;
            raspundeBut.Visible = false;
            this.numePrenumeElev = numePrenumeElev;
            this.idElev = idElev;
            GetNote();
        }

        int cidx = -1;
        Random rand = new Random();

        void HideButtons()
        {

            for (int i = 0; i < 4; i++)
            {
                cbs[i].Visible = false;
                rbs[i].Visible = false;
                cbs[i].Checked = false;
                rbs[i].Checked = false;
                rbs[i].ForeColor = Color.Black;
                cbs[i].ForeColor = Color.Black;
            }

            raspunsLab.Visible = false;
            raspunsTb.Visible = false;
            raspunsTb.Text = "";
        }

        void NextQ()
        {
            HideButtons();
            punctajLab.Text = $"Punctaj = {punctaj}";
            cidx++;

            if (cidx >= 10)
            {
                enuntTb.Text = "Testul este gata.";
                Program.ExecNQ($"INSERT INTO Evaluari (IdElev,DataEvaluare,NotaEvaluare) VALUES (\'{idElev}\',\'{DateTime.Now}\',{punctaj});");
                raspundeBut.Visible = false;
                GetNote();
                return;
            }

            citem = items[cidx];
            enuntTb.Text = citem.EnuntItem;

            if (citem.TipItem == 1)
            {
                raspunsLab.Visible = true;
                raspunsTb.Visible = true;
            }
            else if(citem.TipItem == 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    rbs[i].Text = citem.RaspunsItem[i];
                    rbs[i].Visible = citem.RaspunsItem[i]!=null;
                }
            }
            else if(citem.TipItem == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    cbs[i].Text = citem.RaspunsItem[i];
                    cbs[i].Visible = citem.RaspunsItem[i] != null;
                }

            }
            else if(citem.TipItem == 4)
            {
                rbs[0].Visible = true;
                rbs[1].Visible = true;
                rbs[0].Text = "Fals";
                rbs[1].Text = "Adevarat";
            }
        }

        private void startTestBut_Click(object sender, EventArgs e)
        {
            punctaj = 1;
            SqlCommand cmd = new SqlCommand("SELECT * FROM Itemi", Program.con);
            var res = cmd.ExecuteReader();
            items.Clear();
            while (res.Read())
            {
                Item it = new Item
                {
                    TipItem = res.GetInt32(1),
                    EnuntItem = res.GetString(2),
                    RaspunsItem = new string[] { res.GetString(3), res.GetString(4), res.GetString(5), res.GetString(6) },
                    RaspunsCorectItem = res.GetString(7)
                };
                items.Add(it);  
            }
            items = items.OrderBy((x) => rand.Next()).ToList();

            cidx = -1;
            raspundeBut.Visible = true;
            res.Close();
            NextQ();
        }

        bool CheckAns()
        {
            if (citem.TipItem == 1)
            {
                string[] cuv = raspunsTb.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] cuv2 = citem.RaspunsCorectItem.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (cuv.Length != cuv2.Length)
                {
                    raspunsTb.Text = citem.RaspunsCorectItem;
                    raspunsTb.ForeColor = Color.Red;
                    return false;
                }

                for (int i = 0; i < cuv.Length; i++)
                {
                    if (string.Equals(cuv[i], cuv2[i], StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        raspunsTb.Text = citem.RaspunsCorectItem;
                        raspunsTb.ForeColor = Color.Red;
                        return false;
                    }
                }
                return true;
            }
            else if (citem.TipItem == 2)
            {
                int ansSz = citem.RaspunsCorectItem.Length;
                int crctNr = 0;
                for (int i = 0; i < 4; i++)
                {
                    if(citem.RaspunsCorectItem.IndexOf((char)('0' + i+1)) != -1)
                    {
                        rbs[i].ForeColor = Color.Green;
                        if (rbs[i].Checked )
                            crctNr++;
                    }
                    else
                    {
                        rbs[i].ForeColor = Color.Red;
                    }
                }
                return crctNr == ansSz;
            }
            else if (citem.TipItem == 3)
            {
                int ansSz = citem.RaspunsCorectItem.Length;
                int crctNr = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (citem.RaspunsCorectItem.IndexOf((char)('0' + i+1)) != -1)
                    {
                        cbs[i].ForeColor = Color.Green;
                        if (cbs[i].Checked)
                            crctNr++;
                    }
                    else
                    {
                        cbs[i].ForeColor = Color.Red;
                        if (cbs[i].Checked)
                            crctNr = -100;
                    }
                }
                return crctNr == ansSz;
            }
            else if (citem.TipItem == 4)
            {

                if (citem.RaspunsCorectItem == "0")
                    radioButton1.ForeColor = Color.Green;
                else
                    radioButton2.ForeColor = Color.Green;

                if (citem.RaspunsCorectItem == "0" && radioButton1.Checked)
                    return true;
                if (citem.RaspunsCorectItem == "1" && radioButton2.Checked)
                    return true;
                return false;
            }
            else
                return false;
        }


        private void raspundeBut_Click(object sender, EventArgs e)
        {
            if(raspundeBut.Text == "Raspunde")
            {
                if (CheckAns())
                {
                    raspunsLab.Text = "Raspunde";
                    punctaj++;
                    NextQ();
                }
                else
                    raspundeBut.Text = "Urmatoarea";
            }
            else
            {
                NextQ();
                raspundeBut.Text = "Raspunde";
            }
        }

        PrintDocument printDocument = new PrintDocument();
        private void print_ButtonClick(object sender, EventArgs e)
        {
            printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            printPreviewDialog1.Location = new System.Drawing.Point(29, 29);
            printDocument.PrintPage  += new PrintPageEventHandler(document_PrintPage);
            printPreviewDialog1.Document = printDocument;
            printPreviewDialog1.Show();
        }


        private void document_PrintPage(object sender,PrintPageEventArgs e)
        {
            string text = "Carnetul de note al elevului " + numePrenumeElev;
            Font printFont = new Font("Arial", 25,FontStyle.Regular);
            e.Graphics.DrawString(text, printFont,Brushes.Black, 120, 50);
            Bitmap bmp = new Bitmap(dataGridView1.Width,dataGridView1.Height);
            dataGridView1.ClearSelection();
            dataGridView1.DrawToBitmap(bmp,new Rectangle(100,100,bmp.Width,bmp.Height));
            e.Graphics.DrawImage(bmp,0,0);
            
        }
    }
}
