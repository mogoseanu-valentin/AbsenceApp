using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AbsenteAPP2.Forms
{
    public partial class MainF : Form
    {
        public static MainF PMainF;
        public AbsenteDBEntities AbsenteDB = new AbsenteDBEntities();
        public Image DocMotivareImg;

        public MainF()
        {
            InitializeComponent();
            PMainF = this;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            var x = MessageBox.Show("Doriti sa parasiti aplicatia?", "Parasire aplicatie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (x == DialogResult.Yes)
            {
                AbsenteDB.SaveChanges();
                AbsenteDB.Dispose();
                Application.Exit();
            }
        }

        private void MainF_Load(object sender, EventArgs e)
        {
            RefreshControlsData();
            cmbxOperatii.SelectedIndex = 0;
            RefreshFiltreSortare();
            RefreshAbsenteTable();
            RefreshDocs();
            ImgDocMot.ImageLayout = DataGridViewImageCellLayout.Zoom;
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            var x = MessageBox.Show("Aceasta optiune este doar pentru diriginti. Doriti sa continuati?", "Configurare", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (x == DialogResult.Yes)
            {
                ConfigP.BringToFront();
                btnConfig.Enabled = false;
                btnView.Enabled = true;
                btnStats.Enabled = true;
                btnDocs.Enabled = true;
                RefreshDataTable();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string message = "Sunteti pe cale de a reseta baza de date! (Pierderea tuturor datelor din aplicatie.) Doriti salvarea datelor?";
            var x = MessageBox.Show(message, "Resetare date aplicatie", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (x == DialogResult.Yes)
            {
                SaveAsF saveAsF = new SaveAsF();
                saveAsF.ShowDialog();

                if (saveAsF.PSaveAsF.IsDataSaved)
                {
                    DeleteAllData();
                    MessageBox.Show("Date sterse cu succes!", "Stergere date", default, MessageBoxIcon.Information);
                }
                else
                { 
                    MessageBox.Show("Datele nu au fost sterse!", "Stergere date", default, MessageBoxIcon.Information);
                }
            }
            else
            {
                var y = MessageBox.Show("Sigur doriti stergerea datelor?", "Stergere date", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (y == DialogResult.Yes)
                {
                    DeleteAllData();
                    MessageBox.Show("Date sterse cu succes!", "Stergere date", default, MessageBoxIcon.Information);
                }
            }
        }

        private void cekbFiltre_CheckedChanged(object sender, EventArgs e)
        {
            if (cekbFiltre.Checked)
            {
                groupBoxFiltre.Enabled = false;
                RefreshAbsenteTable();
            }
            else
            {
                groupBoxFiltre.Enabled = true;
                RefreshAbsenteTable();
            }
        }

        private void btnAdauga_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            AddAbsF addAbsF = new AddAbsF();
            addAbsF.ShowDialog();
        }

        private void btnGoBack_Click(object sender, EventArgs e)
        {
            ViewP.BringToFront();
            btnConfig.Enabled = true;
            btnView.Enabled = false;
            RefreshAbsenteTable();
        }

        private void rdbElevi_CheckedChanged(object sender, EventArgs e)
        {
            lbltext.Text = "Nume";
            dgvSemestre.Visible = false;
            dgvMaterii.Visible = false;
            dgvElevi.Visible = true;
            ShowHideControls();
        }

        private void rdbMaterii_CheckedChanged(object sender, EventArgs e)
        {
            lbltext.Text = "Materia";
            dgvSemestre.Visible = false;
            dgvMaterii.Visible = true;
            dgvElevi.Visible = false;
            ShowHideControls();
        }

        private void rdbSemestre_CheckedChanged(object sender, EventArgs e)
        {
            lbltext.Text = "Semestru";
            dgvSemestre.Visible = true;
            dgvMaterii.Visible = false;
            dgvElevi.Visible = false;
            ShowHideControls();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Tools tools = new Tools();
            switch (cmbxOperatii.SelectedIndex)
            {
                case 0:
                    if (rdbElevi.Checked)
                    {
                        if (tbxText.Text != string.Empty)
                        {
                            tools.AddNewElev(tbxText.Text);
                            tbxText.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Nu ati introdus nici un nume", "Adaugare elevi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    if (rdbMaterii.Checked)
                    {
                        if (tbxText.Text != string.Empty)
                        {
                            tools.AddNewMaterie(tbxText.Text);
                            tbxText.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Nu ati introdus nici o materie", "Adaugare materii", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    if (rdbSemestre.Checked)
                    {
                        if (string.IsNullOrWhiteSpace(tbxText.Text) || string.IsNullOrWhiteSpace(tbxAn.Text))
                        {
                            MessageBox.Show("Nu ati introdus semestrul/anul", "Adaugare semestre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            tools.AddNewSemestru(tbxText.Text, tbxAn.Text, dtpStart.Value, dtpSfarsit.Value);
                            tbxText.Clear();
                            tbxAn.Clear();
                        }
                    }
                    break;

                case 1:
                    if (rdbElevi.Checked && dgvElevi.Rows.Count != 0)
                    {
                        string oldName = dgvElevi.CurrentRow.Cells[0].Value.ToString();
                        var elev = AbsenteDB.EleviT.FirstOrDefault(x => x.Fullname == oldName);
                        if (elev != null)
                        {
                            elev.Fullname = tbxText.Text;
                            AbsenteDB.EleviT.Attach(elev);
                            AbsenteDB.Entry(elev).State = EntityState.Modified;
                            AbsenteDB.SaveChanges();
                        }
                    }
                    if (rdbMaterii.Checked && dgvMaterii.Rows.Count != 0)
                    {
                        string oldMaterie = dgvMaterii.CurrentRow.Cells[0].Value.ToString();
                        var materie = AbsenteDB.MateriiT.FirstOrDefault(x => x.Materia == oldMaterie);
                        if (materie != null)
                        {
                            materie.Materia = tbxText.Text;
                            AbsenteDB.MateriiT.Attach(materie);
                            AbsenteDB.Entry(materie).State = EntityState.Modified;
                            AbsenteDB.SaveChanges();
                        }
                    }
                    if (rdbSemestre.Checked && dgvSemestre.Rows.Count != 0)
                    {
                        string oldSem = dgvSemestre.CurrentRow.Cells[0].Value.ToString();
                        string oldAn = dgvSemestre.CurrentRow.Cells[1].Value.ToString();
                        DateTime oldStart = DateTime.Parse(dgvSemestre.CurrentRow.Cells[2].Value.ToString());
                        DateTime oldStop = DateTime.Parse(dgvSemestre.CurrentRow.Cells[3].Value.ToString());
                        var sem = AbsenteDB.SemestreT.FirstOrDefault(x => x.Semestrul == oldSem && x.Anul == oldAn && x.DataStart == oldStart && x.DataStop == oldStop);
                        if (sem != null)
                        {
                            sem.Semestrul = tbxText.Text;
                            sem.Anul = tbxAn.Text;
                            sem.DataStart = dtpStart.Value;
                            sem.DataStop = dtpSfarsit.Value;
                            AbsenteDB.SemestreT.Attach(sem);
                            AbsenteDB.Entry(sem).State = EntityState.Modified;
                            AbsenteDB.SaveChanges();
                        }
                    }
                    break;

                case 2:
                    if (rdbElevi.Checked && dgvElevi.Rows.Count != 0)
                    {
                        tools.DeleteElev(dgvElevi.CurrentRow.Cells[0].Value.ToString());
                    }
                    if (rdbMaterii.Checked && dgvMaterii.Rows.Count != 0)
                    {
                        tools.DeleteMaterie(dgvMaterii.CurrentRow.Cells[0].Value.ToString());
                    }
                    if (rdbSemestre.Checked && dgvSemestre.Rows.Count != 0)
                    {
                        string sem = dgvSemestre.CurrentRow.Cells[0].Value.ToString();
                        string an = dgvSemestre.CurrentRow.Cells[1].Value.ToString();
                        DateTime start = DateTime.Parse(dgvSemestre.CurrentRow.Cells[2].Value.ToString());
                        DateTime stop = DateTime.Parse(dgvSemestre.CurrentRow.Cells[3].Value.ToString());
                        tools.DeleteSemestru(sem, an, start, stop);
                    }
                    break;

                default:
                    break;
            }

            RefreshDataTable();
            RefreshControlsData();
            tbxText.Focus();
        }

        private void cmbxOperatii_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHideControls();
        }

        private void ShowHideControls()
        {
            if (rdbElevi.Checked)
            {
                if (cmbxOperatii.SelectedIndex == 0)
                {
                    tbxText.Enabled = true;
                    tbxAn.Enabled = false;
                    dtpStart.Enabled = false;
                    dtpSfarsit.Enabled = false;
                    SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                }
                if (cmbxOperatii.SelectedIndex == 1)
                {
                    tbxText.Enabled = true;
                    tbxAn.Enabled = false;
                    dtpStart.Enabled = false;
                    dtpSfarsit.Enabled = false;
                    if (dgvElevi.Rows.Count != 0)
                    {
                        SetControlsValue(dgvElevi.CurrentRow.Cells[0].Value.ToString(), string.Empty, DateTime.Now, DateTime.Now);
                    }
                    else
                    {
                        SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                    }
                }
                if (cmbxOperatii.SelectedIndex == 2)
                {
                    tbxText.Enabled = false;
                    tbxAn.Enabled = false;
                    dtpStart.Enabled = false;
                    dtpSfarsit.Enabled = false;
                    SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                }
            }

            if (rdbMaterii.Checked)
            {
                if (cmbxOperatii.SelectedIndex == 0)
                {
                    tbxText.Enabled = true;
                    tbxAn.Enabled = false;
                    dtpStart.Enabled = false;
                    dtpSfarsit.Enabled = false;
                    SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                }
                if (cmbxOperatii.SelectedIndex == 1)
                {
                    tbxText.Enabled = true;
                    tbxAn.Enabled = false;
                    dtpStart.Enabled = false;
                    dtpSfarsit.Enabled = false;
                    if (dgvMaterii.Rows.Count != 0)
                    {
                        SetControlsValue(dgvMaterii.CurrentRow.Cells[0].Value.ToString(), string.Empty, DateTime.Now, DateTime.Now);
                    }
                    else
                    {
                        SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                    }
                }
                if (cmbxOperatii.SelectedIndex == 2)
                {
                    tbxText.Enabled = false;
                    tbxAn.Enabled = false;
                    dtpStart.Enabled = false;
                    dtpSfarsit.Enabled = false;
                    SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                }
            }

            if (rdbSemestre.Checked)
            {
                if (cmbxOperatii.SelectedIndex == 0)
                {
                    tbxText.Enabled = true;
                    tbxAn.Enabled = true;
                    dtpStart.Enabled = true;
                    dtpSfarsit.Enabled = true;
                    SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                }
                if (cmbxOperatii.SelectedIndex == 1)
                {
                    tbxText.Enabled = true;
                    tbxAn.Enabled = true;
                    dtpStart.Enabled = true;
                    dtpSfarsit.Enabled = true;
                    if (dgvSemestre.Rows.Count != 0)
                    {
                        string sem = dgvSemestre.CurrentRow.Cells[0].Value.ToString();
                        string an = dgvSemestre.CurrentRow.Cells[1].Value.ToString();
                        DateTime start = DateTime.Parse(dgvSemestre.CurrentRow.Cells[2].Value.ToString());
                        DateTime stop = DateTime.Parse(dgvSemestre.CurrentRow.Cells[3].Value.ToString());
                        SetControlsValue(sem, an, start, stop);
                    }
                    else
                    {
                        SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                    }
                }
                if (cmbxOperatii.SelectedIndex == 2)
                {
                    tbxText.Enabled = false;
                    tbxAn.Enabled = false;
                    dtpStart.Enabled = false;
                    dtpSfarsit.Enabled = false;
                    SetControlsValue(string.Empty, string.Empty, DateTime.Now, DateTime.Now);
                }
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            SaveAsF saveAsF = new SaveAsF();
            saveAsF.ShowDialog();
        }

        private void RefreshDataTable()
        {
            dgvElevi.Rows.Clear();
            AbsenteDB.EleviT.ToList().ForEach(x => dgvElevi.Rows.Add(x.Fullname));

            dgvMaterii.Rows.Clear();
            AbsenteDB.MateriiT.ToList().ForEach(x => dgvMaterii.Rows.Add(x.Materia));

            dgvSemestre.Rows.Clear();
            AbsenteDB.SemestreT.ToList().ForEach(x => dgvSemestre.Rows.Add(x.Semestrul, x.Anul, x.DataStart.ToShortDateString(), x.DataStop.ToShortDateString()));

            dgvElevi.ClearSelection();
            dgvMaterii.ClearSelection();
            dgvSemestre.ClearSelection();
        }

        private void SetControlsValue(string text, string an, DateTime start, DateTime stop)
        {
            tbxText.Text = text;
            tbxText.Text = text;
            tbxAn.Text = an;
            dtpStart.Value = start.Date;
            dtpSfarsit.Value = stop.Date;
        }

        private void CurrentDGV_SelectionChanged(object sender, EventArgs e)
        {
            if (cmbxOperatii.SelectedIndex == 1)
            {
                if (rdbElevi.Checked && dgvElevi.Rows.Count != 0)
                {
                    SetControlsValue(dgvElevi.CurrentRow.Cells[0].Value.ToString(), string.Empty, DateTime.Now, DateTime.Now);
                }
                if (rdbMaterii.Checked && dgvMaterii.Rows.Count != 0)
                {
                    SetControlsValue(dgvMaterii.CurrentRow.Cells[0].Value.ToString(), string.Empty, DateTime.Now, DateTime.Now);
                }
                if (rdbSemestre.Checked && dgvSemestre.Rows.Count != 0)
                {
                    string sem = dgvSemestre.CurrentRow.Cells[0].Value.ToString();
                    string an = dgvSemestre.CurrentRow.Cells[1].Value.ToString();
                    DateTime start = DateTime.Parse(dgvSemestre.CurrentRow.Cells[2].Value.ToString());
                    DateTime stop = DateTime.Parse(dgvSemestre.CurrentRow.Cells[3].Value.ToString());
                    SetControlsValue(sem, an, start, stop);
                }
            }
        }

        public void RefreshAbsenteTable()
        {
            dgvAbsente.ClearSelection();
            dgvAbsente.Rows.Clear();
            var absente = AbsenteDB.AbsenteT.ToList();
            List<AbsenteT> cAbsente = absente;

            if (cekbFiltre.Checked)
            {
                absente.ForEach(x => dgvAbsente.Rows.Add(x.EleviT.Fullname, x.MateriiT.Materia, x.Motivata, x.Data.ToShortDateString(), x.SemestreT.Semestrul));
            }
            else
            {

                if (rdbToate.Checked)
                {
                    FilteredAbs().ForEach(x => dgvAbsente.Rows.Add(x.EleviT.Fullname, x.MateriiT.Materia, x.Motivata, x.Data.ToShortDateString(), x.SemestreT.Semestrul));
                }
                if (rdbMotivate.Checked)
                {
                    List<AbsenteT> abs = FilteredAbs();
                    abs.RemoveAll(x => x.Motivata == false);
                    abs.ForEach(x => dgvAbsente.Rows.Add(x.EleviT.Fullname, x.MateriiT.Materia, x.Motivata, x.Data.ToShortDateString(), x.SemestreT.Semestrul));
                }
                if (rdbNemotivate.Checked)
                {
                    List<AbsenteT> abs = FilteredAbs();
                    abs.RemoveAll(x => x.Motivata == true);
                    abs.ForEach(x => dgvAbsente.Rows.Add(x.EleviT.Fullname, x.MateriiT.Materia, x.Motivata, x.Data.ToShortDateString(), x.SemestreT.Semestrul));
                }
            }
        }

        private void btnSterge_Click(object sender, EventArgs e)
        {
            if (dgvAbsente.Rows.Count != 0)
            {
                var absenta = GetSelectedAbs();
                var mydr = MessageBox.Show($"Stergi absenta lui {absenta.EleviT.Fullname} la disciplina {absenta.MateriiT.Materia} din data {absenta.Data.ToShortDateString()}?", "Stergere absemta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mydr == DialogResult.Yes)
                {
                    if (absenta != null)
                    {
                        AbsenteDB.AbsenteT.Remove(absenta);
                        AbsenteDB.SaveChanges();
                        RefreshAbsenteTable();
                    }
                }
            }
        }

        private void btnDocNotivare_Click(object sender, EventArgs e)
        {
            DocsP.BringToFront();
            btnConfig.Enabled = true;
            btnView.Enabled = true;
            btnStats.Enabled = true;
            btnDocs.Enabled = false;
            cmbxElevDM.Items.Clear();
            RefreshControlsData();
            if (cmbxElevDM.Items.Count!=0)
            {
                cmbxElevDM.SelectedIndex = 0;
            }
            RefreshDocs();
        }

        private void btnMotiveaza_Click(object sender, EventArgs e)
        {
            var absenta = GetSelectedAbs();

            if (absenta != null && absenta.Motivata == false)
            {
                string msg = $"Doriti sa motivati absenta lui {absenta.EleviT.Fullname} la disciplina {absenta.MateriiT.Materia} din data {absenta.Data.ToShortDateString()}?";
                var dialog = MessageBox.Show(msg, "Motivare absenta", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    absenta.Motivata = true;
                    AbsenteDB.AbsenteT.Attach(absenta);
                    AbsenteDB.Entry(absenta).State = EntityState.Modified;
                    AbsenteDB.SaveChanges();
                    RefreshAbsenteTable();
                }
            }
            else
            {
                MessageBox.Show("Nu exista absente de motivat!", "Motivare absenta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private AbsenteT GetSelectedAbs()
        {
            if (dgvAbsente.Rows.Count != 0)
            {
                string nume = dgvAbsente.CurrentRow.Cells[0].Value.ToString();
                string materia = dgvAbsente.CurrentRow.Cells[1].Value.ToString();
                bool eMotivata = bool.Parse(dgvAbsente.CurrentRow.Cells[2].Value.ToString());
                DateTime data = DateTime.Parse(dgvAbsente.CurrentRow.Cells[3].Value.ToString());
                return AbsenteDB.AbsenteT.FirstOrDefault(x => x.EleviT.Fullname == nume && x.MateriiT.Materia == materia && x.Motivata == eMotivata && x.Data == data);
            }
            else
            {
                return null;
            }
        }

        private void CMBX_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAbsenteTable();
        }

        public void RefreshControlsData()
        {
            cmbxElevDM.Items.Clear();
            cmbxMaterii.Items.Clear();
            cmbxNume.Items.Clear();
            cmbxSemestre.Items.Clear();
            AbsenteDB.EleviT.ToList().ForEach(x => cmbxNume.Items.Add(x.Fullname));
            AbsenteDB.MateriiT.ToList().ForEach(x => cmbxMaterii.Items.Add(x.Materia));
            AbsenteDB.SemestreT.ToList().ForEach(x => cmbxSemestre.Items.Add(x.Semestrul));
            AbsenteDB.EleviT.ToList().ForEach(x => cmbxElevDM.Items.Add(x.Fullname));
        }

        public List<AbsenteT> FilteredAbs()
        {
            List<AbsenteT> absente = AbsenteDB.AbsenteT.ToList();
            absente.RemoveAll(x => x.EleviT.Fullname != cmbxNume.Text);
            absente.RemoveAll(x => x.MateriiT.Materia != cmbxMaterii.Text);
            absente.RemoveAll(x => x.SemestreT.Semestrul != cmbxSemestre.Text);
            return absente;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            ViewP.BringToFront();
            btnDocs.Enabled = true;
            btnConfig.Enabled = true;
            btnStats.Enabled = true;
            btnView.Enabled = false;
            RefreshAbsenteTable();
            RefreshControlsData();
            RefreshFiltreSortare();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG|*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG|All files (*.*)|*.*";
            var x = openFile.ShowDialog();
            tbxPath.Text = openFile.FileName;
        }

        private void btnFinalizare_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxPath.Text))
            {
                MessageBox.Show("Trebuie sa alegeti o imagine a unui document de motivare", "Document motivare", default, MessageBoxIcon.Information);
            }
            else
            {
                DocsMotivareT docMot = new DocsMotivareT();
                docMot.ElevFK = AbsenteDB.EleviT.FirstOrDefault(x => x.Fullname == cmbxElevDM.Text).ElevID;
                docMot.DocMotivare = ImageToByteArray(Image.FromFile(tbxPath.Text));
                docMot.DataStart = dtpStartDM.Value.Date;
                docMot.DataStop = dtpStopDM.Value.Date;


                if (Tools.FindDocument(docMot, AbsenteDB))
                {
                    MessageBox.Show("Document deja existent", "Documente motivare", default, MessageBoxIcon.Information);
                }
                else
                {
                    if (OverlapDate(docMot))
                    {
                        MessageBox.Show("Periaoda selectata nu este valida!", "Documente motivare", default, MessageBoxIcon.Information);
                    }
                    else
                    {
                        AbsenteDB.DocsMotivareT.Add(docMot);
                        AbsenteDB.SaveChanges();
                        RefreshDocs();
                        var abs = AbsenteDB.AbsenteT.ToList();

                        foreach (var item in abs)
                        {
                            if (item.ElevFK == docMot.ElevFK)
                            {
                                if (item.Data >= docMot.DataStart && item.Data <= docMot.DataStop)
                                {
                                    item.Motivata = true;
                                    AbsenteDB.AbsenteT.Attach(item);
                                    AbsenteDB.Entry(item).State = EntityState.Modified;
                                    AbsenteDB.SaveChanges();
                                    RefreshAbsenteTable();
                                }
                            }
                        }

                        string msg = $"Toate absentele nemotivate ale lui {docMot.EleviT.Fullname}" +
                            $" de la data {docMot.DataStart.ToShortDateString()}" +
                            $" pana la data de {docMot.DataStop.ToShortDateString()} au fost motivate";

                        MessageBox.Show(msg, "Motivare absente", default, MessageBoxIcon.Information);
                    }
                }
                cmbxElevDM.SelectedIndex = 0;
                dtpStartDM.Value = DateTime.Today;
                dtpStopDM.Value = DateTime.Today;
                tbxPath.Clear();
            }
        }

        public byte[] ImageToByteArray(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        public void RefreshDocs()
        {
            dgvDocs.Rows.Clear();
            AbsenteDB.DocsMotivareT.ToList().ForEach(x => dgvDocs.Rows.Add(x.EleviT.Fullname, ByteArrayToImage(x.DocMotivare), x.DataStart.ToShortDateString(), x.DataStop.ToShortDateString()));
            dgvDocs.ClearSelection();
        }

        public Image ByteArrayToImage(byte[] bytesArray)
        {
            Image returnImage;
            if (bytesArray != null && bytesArray.Length != 0)
            {
                MemoryStream ms = new MemoryStream(bytesArray);
                returnImage = Image.FromStream(ms);
                return returnImage;
            }
            else
            {
                return null;
            }
        }

        private void btnViewImage_Click(object sender, EventArgs e)
        {
            if (cekbViewMode.Checked)
            {
                if (dgvDocs.Rows.Count != 0)
                {
                    ViewImgDocF viewImg = new ViewImgDocF();
                    viewImg.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Nu exista documente de motivare", "Vizualizare document motivare", default, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(tbxPath.Text))
                {
                    MessageBox.Show("Nu ati selectat o imagine", "Vizualizare imagine", default, MessageBoxIcon.Information);
                }
                else
                {
                    DocMotivareImg = Image.FromFile(tbxPath.Text);
                    ViewImgDocF viewImg = new ViewImgDocF();
                    viewImg.ShowDialog();
                }
            }
        }

        private void cekbViewMode_CheckedChanged(object sender, EventArgs e)
        {
            if (cekbViewMode.Checked)
            {
                gbxAdaugare.Enabled = false;
                btnDelDM.Enabled = true;
            }
            else
            {
                gbxAdaugare.Enabled = true;
                btnDelDM.Enabled = false;
            }
        }

        private void btnDelDM_Click(object sender, EventArgs e)
        {
            if (dgvDocs.Rows.Count!=0)
            {
                string name = dgvDocs.CurrentRow.Cells[0].Value.ToString();
                DateTime start = DateTime.Parse(dgvDocs.CurrentRow.Cells[2].Value.ToString());
                DateTime stop = DateTime.Parse(dgvDocs.CurrentRow.Cells[3].Value.ToString());

                var docMot = AbsenteDB.DocsMotivareT.FirstOrDefault(x => x.EleviT.Fullname == name && x.DataStart == start && x.DataStop == stop);

                if (docMot != null)
                {
                    AbsenteDB.DocsMotivareT.Remove(docMot);
                    AbsenteDB.SaveChanges();
                    RefreshDocs();
                }
            }
            else
            {
                MessageBox.Show("Nu exista documente de motivare!", "Documente motivare", default, MessageBoxIcon.Information);
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            string msg = "Nume aplicatie: Mangement absente la nivel de clasa \n" +
                "Autor: Mogoseanu Valentin Alexandru \n" +
                "Versiune: 1.2.0.1 \n" +
                "Drepturi de autor rezervate!";
            MessageBox.Show(msg, "About", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void DeleteAllData()
        {
            cmbxElevDM.Items.Clear();
            cmbxMaterii.Items.Clear();
            cmbxNume.Items.Clear();
            cmbxSemestre.Items.Clear();
            dgvMaterii.Rows.Clear();
            dgvElevi.Rows.Clear();
            dgvSemestre.Rows.Clear();
            dgvAbsente.Rows.Clear();
            dgvDocs.Rows.Clear();
            AbsenteDB.AbsenteT.ToList().ForEach(x => AbsenteDB.AbsenteT.Remove(x));
            AbsenteDB.DocsMotivareT.ToList().ForEach(x => AbsenteDB.DocsMotivareT.Remove(x));
            AbsenteDB.EleviT.ToList().ForEach(x => AbsenteDB.EleviT.Remove(x));
            AbsenteDB.MateriiT.ToList().ForEach(x => AbsenteDB.MateriiT.Remove(x));
            AbsenteDB.SemestreT.ToList().ForEach(x => AbsenteDB.SemestreT.Remove(x));
            AbsenteDB.SaveChanges();
        }

        private void RefreshFiltreSortare()
        {
            if (cmbxNume.Items.Count != 0)
            {
                cmbxNume.SelectedIndex = 0;
            }
            if (cmbxMaterii.Items.Count != 0)
            {
                cmbxMaterii.SelectedIndex = 0;
            }
            
            if (cmbxElevDM.Items.Count != 0)
            {
                cmbxElevDM.SelectedIndex = 0;
            }

            if (cmbxSemestre.Items.Count!=0)
            {
                var sem = AbsenteDB.SemestreT.FirstOrDefault(x => x.DataStart <= DateTime.Today && x.DataStop >= DateTime.Today);
                if (sem != null)
                {
                    cmbxSemestre.SelectedItem = sem.Semestrul;
                }
                else
                {
                    cmbxSemestre.SelectedIndex = 0;
                }
            }
        }

        private void dgvDocs_SelectionChanged(object sender, EventArgs e)
        {
            DocMotivareImg = GetImageFromDGV(dgvDocs, 1);
        }

        public Image GetImageFromDGV(DataGridView dgv,int columnIndex)
        {
            if (dgv.Rows.Count != 0)
            {
                Byte[] bytes = ImageToByteArray((Image)dgv.CurrentRow.Cells[1].Value);
                MemoryStream ms = new MemoryStream(bytes);
                Image img = Image.FromStream(ms);
                return img;
            }
            else
            {
                return null;
            }
        }

        public bool OverlapDate(DocsMotivareT doc)
        {
            var docs = AbsenteDB.DocsMotivareT.ToList();
            docs.RemoveAll(x => x.ElevFK != doc.ElevFK);

            foreach (var item in docs)
            {
                int r1 = DateTime.Compare(doc.DataStop, item.DataStart);
                int r2 = DateTime.Compare(doc.DataStart, item.DataStop);

                if (r1 >= 0 && r2 <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnStats_Click(object sender, EventArgs e)
        {
            StatsP.BringToFront();
            AbsenteDB.EleviT.ToList().ForEach(x => cmbxElevST.Items.Add(x.Fullname));
            if (cmbxElevST.Items.Count!=0)
            {
                cmbxElevST.SelectedIndex = 0;
            }

            lblNrElevi.Text = $"Nr. Elevi: {AbsenteDB.EleviT.Count()}";
            lblNrMaterii.Text = $"Nr. Materii: {AbsenteDB.MateriiT.Count()}";
            RefreshDGVStats();

            btnConfig.Enabled = true;
            btnView.Enabled = true;
            btnDocs.Enabled = true;
            btnStats.Enabled = false;
        }

        private void RefreshDGVStats()
        {
            dgvStats.Rows.Clear();
            int nrAbs = 0;
            int nrAbsNem = 0;
            int nrAbsMot = 0;

            if (cekbxAllElevs.Checked)
            {
                foreach (var item in AbsenteDB.EleviT)
                {
                    nrAbs = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == item.Fullname).Count();
                    nrAbsMot = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == item.Fullname && x.Motivata == true).Count();
                    nrAbsNem = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == item.Fullname && x.Motivata == false).Count();
                    dgvStats.Rows.Add(item.Fullname, nrAbsNem, nrAbsMot, nrAbs);
                    nrAbs = 0;
                    nrAbsMot = 0;
                    nrAbsNem = 0;
                }
            }
            else
            {
                if (cmbxElevST.Items.Count!=0)
                {
                    nrAbs = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == cmbxElevST.Text).Count();
                    nrAbsMot = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == cmbxElevST.Text && x.Motivata == true).Count();
                    nrAbsNem = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == cmbxElevST.Text && x.Motivata == false).Count();
                    dgvStats.Rows.Clear();
                    dgvStats.Rows.Add(cmbxElevST.Text, nrAbsNem, nrAbsMot, nrAbs);
                }
            }
        }

        private void cekbxAllElevs_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDGVStats();
            if (cekbxAllElevs.Checked)
            {
                cmbxElevST.Enabled = false;
            }
            else
            {
                cmbxElevST.Enabled = true;
            }
        }

        private void btnBrowseST_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            tbxPathST.Text = folderBrowser.SelectedPath;
        }

        private void btnSaveAsST_Click(object sender, EventArgs e)
        {
            try
            {
                string fullpath = tbxPathST.Text + @"\" + "StatisticaAbsente.txt";

                if (File.Exists(fullpath))
                {
                    File.WriteAllText(fullpath, String.Empty);
                }

                var myFile = File.Create(fullpath);
                myFile.Close();

                using (StreamWriter sw = new StreamWriter(fullpath))
                {
                    foreach (var item in AbsenteDB.EleviT)
                    {
                        int nrAbs = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == item.Fullname).Count();
                        int nrAbsMot = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == item.Fullname && x.Motivata == true).Count();
                        int nrAbsNem = AbsenteDB.AbsenteT.Where(x => x.EleviT.Fullname == item.Fullname && x.Motivata == false).Count();
                        sw.WriteLine($"Elevul {item.Fullname}; Absente: {nrAbs}; Nemotivate: {nrAbsNem}; Motivate: {nrAbsMot}");
                    }
                }
                tbxPath.Clear();
                MessageBox.Show("Statistici absente elevi salvate cu succes!", "Salvare statistici", default, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", default, MessageBoxIcon.Error);
            }
            finally
            {
                tbxPathST.Clear();
            }
        }

        private void cmbxElevST_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDGVStats();
        }
    }
}