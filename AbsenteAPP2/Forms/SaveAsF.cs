using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace AbsenteAPP2.Forms
{
    public partial class SaveAsF : Form
    {
        public SaveAsF PSaveAsF;
        public bool IsDataSaved = false;

        public SaveAsF()
        {
            InitializeComponent();
            tbxFileName.Text = "Situatie absente";
            PSaveAsF = this;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainF.PMainF.Enabled = true;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdPath = new FolderBrowserDialog();
            var x = fbdPath.ShowDialog();
            if (x==DialogResult.OK)
            {
                tbxPath.Text = fbdPath.SelectedPath;
            }
        }

        private void btnSalvare_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxPath.Text))
            {
                MessageBox.Show("Nu ati selectat o locatie!", "Salvare date", default, MessageBoxIcon.Error);
            }
            else
            {
                string fullpath = tbxPath.Text + @"\" + tbxFileName.Text + ".txt";

                if (File.Exists(fullpath))
                {
                    MessageBox.Show("Exista un fisier cu acelasi nume!", "Salvare situatie absente", default, MessageBoxIcon.Error);
                }
                else
                {
                    var myFile = File.Create(fullpath);
                    myFile.Close();

                    AbsenteDBEntities absenteDB = new AbsenteDBEntities();
                    using (StreamWriter sw = new StreamWriter(fullpath))
                    {
                        absenteDB.AbsenteT.ToList().ForEach(x => sw.WriteLine(x.EleviT.Fullname + " | " + x.MateriiT.Materia + " | Motivata: " + x.Motivata.ToString() + " | " + x.Data.ToShortDateString()));
                    }
                    tbxPath.Clear();
                    MessageBox.Show("Date salvate cu succes!", "Salvare date", default, MessageBoxIcon.Information);
                    IsDataSaved = true;
                    MainF.PMainF.Enabled = true;
                    this.Close();
                }
            }
        }
    }
}
