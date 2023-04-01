using System;
using System.Linq;
using System.Windows.Forms;

namespace AbsenteAPP2.Forms
{
    public partial class AddAbsF : Form
    {
        AbsenteDBEntities AbsenteDB = new AbsenteDBEntities();
        public AddAbsF()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainF.PMainF.Enabled = true;
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            if (cmbxMateria.Items.Count!=0 && cmbxNume.Items.Count!=0)
            {
                AbsenteT absenta = new AbsenteT();
                absenta.ElevFK = AbsenteDB.EleviT.FirstOrDefault(x => x.Fullname == cmbxNume.Text).ElevID;
                absenta.MateriaFK = AbsenteDB.MateriiT.FirstOrDefault(x => x.Materia == cmbxMateria.Text).MaterieID;
                absenta.Motivata = false;
                absenta.Data = dtpData.Value;

                var sem = AbsenteDB.SemestreT.FirstOrDefault(x => x.DataStart <= absenta.Data && x.DataStop >= absenta.Data);

                if (sem != null)
                {
                    absenta.SemestrulFK = sem.SemestruID;
                    AbsenteDB.AbsenteT.Add(absenta);
                    AbsenteDB.SaveChanges();
                    MainF.PMainF.RefreshAbsenteTable();
                    MainF.PMainF.Enabled = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Data introdusa nu este valida!\nSAU\nSemestrele nu sunt inregistrate!", "Adaugare absenta", default, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Nu exista sufieciente materii sau elevi!", "Adaugare absenta", default, MessageBoxIcon.Error);
            }
        }

        private void AddAbsF_Load(object sender, EventArgs e)
        {
            AbsenteDB.EleviT.ToList().ForEach(x => cmbxNume.Items.Add(x.Fullname));
            AbsenteDB.MateriiT.ToList().ForEach(x => cmbxMateria.Items.Add(x.Materia));

            if (cmbxMateria.Items.Count!=0)
            {
                cmbxMateria.SelectedIndex = 0;
            }
            if (cmbxNume.Items.Count!=0)
            {
                cmbxNume.SelectedIndex = 0;
            }
        }
    }
}
