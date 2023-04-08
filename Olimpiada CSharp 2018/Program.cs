using Olimpiada_CSharp_2018.resurse;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Olimpiada_CSharp_2018
{
    internal static class Program
    {
        public static SqlConnection con;
        public static void ExecNQ(string q)
        {
            SqlCommand cmdSql = new SqlCommand(q, con);
            cmdSql.ExecuteNonQuery();
        }
        static void ReadToDb()
        {
            string txt = File.ReadAllText("../../resurse/date.txt");
            string[] lines = txt.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            // util
            int idx = 0;
            idx++;
            ExecNQ("TRUNCATE TABLE Utilizatori");
            do
            {
                string[] args = lines[idx].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                ExecNQ($"INSERT INTO Utilizatori (NumePrenumeUtilizator,ParolaUtilizator,EmailUtilizator,ClasaUtilizator) VALUES (\'{args[0]}\',\'{args[1]}\',\'{args[2]}\',\'{args[3]}\');");
                idx++;
            } while (lines[idx].StartsWith("Itemi") == false);
            idx++;
            ExecNQ("TRUNCATE TABLE Itemi");
            do
            {
                string[] args = lines[idx].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                ExecNQ($"INSERT INTO Itemi (TipItem,EnuntItem,Raspuns1Item,Raspuns2Item,Raspuns3Item,Raspuns4Item,RaspunsCorectItem) VALUES (\'{args[0]}\',\'{args[1]}\',\'{args[2]}\',\'{args[3]}\','{args[4]}','{args[5]}','{args[6]}');");
                idx++;
            } while (lines[idx].StartsWith("Evaluari") == false);
            idx++;
            ExecNQ("TRUNCATE TABLE Evaluari");
            do
            {
                string[] args = lines[idx].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                ExecNQ($"INSERT INTO Evaluari (IdElev,DataEvaluare,NotaEvaluare) VALUES (\'{args[0]}\',\'{args[1]}\',\'{args[2]}\');");
                idx++;
            } while (idx < lines.Length);

        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""|DataDirectory|\eLearning1918.mdf"";Integrated Security=True;Connect Timeout=30");
            con.Open();
            ReadToDb();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new eLearning1918_start());
        }
    }
}
