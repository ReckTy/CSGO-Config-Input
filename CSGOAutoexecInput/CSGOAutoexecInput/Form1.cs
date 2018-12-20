using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace CSGOConfigInput
{
    public partial class Form1 : Form
    {
        private string dataPath;
        private string dataSteamPath;
        private string dataInputPath;
        private string dataAutorunPath;

        bool inputChanged = false;
        bool autorun = false;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataPath = Truncate(Application.ExecutablePath, Application.ExecutablePath.Length - Application.CompanyName.Length - 4) + "Data";
            dataSteamPath = dataPath + @"\SteamPath.txt";
            dataInputPath = dataPath + @"\Input.txt";
            dataAutorunPath = dataPath + @"\Autorun.txt";

            CreateMissingTextFiles();

            textBox1.Text = File.ReadLines(dataSteamPath).First();

            richTextBox1.Lines = File.ReadAllLines(dataInputPath);
            
            inputChanged = false;

            if (autorun)
            {
                button1_Click(null, null);
                if (autorun) Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] dirs = Directory.GetDirectories(textBox1.Text + @"\userdata");
            bool success = false;

            foreach (string s in dirs)
            {
                string configDir = s + @"\730\local\cfg\config.cfg";
                
                if (File.Exists(configDir))
                {
                    List<string> text = File.ReadAllLines(configDir).ToList();

                    text.Add("\n");

                    foreach (String line in richTextBox1.Lines)
                    {
                        text.Add(line);
                    }

                    File.WriteAllLines(configDir, text);

                    success = true;
                }
            }

            if (success && !autorun)
                MessageBox.Show("Input Inserted!", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            else if (!success)
            {
                MessageBox.Show("Insert Failed!", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                autorun = false;
            }
        }

        private void CreateMissingTextFiles()
        {
            Directory.CreateDirectory(dataPath);

            if (!File.Exists(dataSteamPath))
            {
                using (StreamWriter sw = File.CreateText(dataSteamPath))
                {
                    sw.WriteLine((@"C:\Program Files (x86)\Steam"));
                }
            }
            if (!File.Exists(dataInputPath))
                File.CreateText(dataInputPath);

            if (!File.Exists(dataAutorunPath))
            {
                using (StreamWriter sw = File.CreateText(dataAutorunPath))
                {
                    sw.WriteLine("False");
                }
            }
            else if (File.ReadLines(dataAutorunPath).First() == "True")
            {
                autorun = true;
            }
            else
                File.WriteAllText(dataAutorunPath, "False");
        }

        private string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((textBox1.Text != File.ReadAllText(dataSteamPath) || inputChanged) && MessageBox.Show("Do you want to save data?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                File.WriteAllText(dataSteamPath, textBox1.Text);

                File.WriteAllLines(dataInputPath, richTextBox1.Lines);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            inputChanged = true;
        }

    }
}
