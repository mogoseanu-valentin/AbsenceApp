using System;
using System.Linq;
using AbsenteAPP2.Forms;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AbsenteAPP2
{
    public class Tools
    {

        AbsenteDBEntities AbsenteDB = MainF.PMainF.AbsenteDB;

        public void AddNewElev(string nume)
        {
            if (AbsenteDB.EleviT.FirstOrDefault(x => x.Fullname == nume) != null)
            {
                MessageBox.Show("Elevul exista deja", "Adaugare elev", default, MessageBoxIcon.Information);
            }
            else
            {
                EleviT elev = new EleviT(nume);
                AbsenteDB.EleviT.Add(elev);
                AbsenteDB.SaveChanges();
            }
        }

        public void AddNewMaterie(string materie)
        {
            if (AbsenteDB.MateriiT.FirstOrDefault(x => x.Materia == materie) != null)
            {
                MessageBox.Show("Materia exista deja", "Adaugare materie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MateriiT mat = mat = new MateriiT(materie);
                AbsenteDB.MateriiT.Add(mat);
                AbsenteDB.SaveChanges();
            }
        }

        public void AddNewSemestru(string semestru, string anul, DateTime inceput, DateTime sfarsit)
        {
            if (AbsenteDB.SemestreT.FirstOrDefault(x => x.Semestrul == semestru || x.DataStart == inceput || x.DataStop == sfarsit) != null)
            {
                MessageBox.Show("Semestrul exista deja", "Adaugare semestru", default, MessageBoxIcon.Information);
            }
            else
            {
                SemestreT sem = sem = new SemestreT(semestru, anul, inceput, sfarsit);
                AbsenteDB.SemestreT.Add(sem);
                AbsenteDB.SaveChanges();
            }
        }

        public void DeleteElev(string name)
        {
            var elev = AbsenteDB.EleviT.FirstOrDefault(x => x.Fullname == name);
            if (elev != null)
            {
                AbsenteDB.EleviT.Remove(elev);
                AbsenteDB.SaveChanges();
            }
        }

        public void DeleteMaterie(string materie)
        {
            var mat = AbsenteDB.MateriiT.FirstOrDefault(x => x.Materia == materie);
            if (mat != null)
            {
                AbsenteDB.MateriiT.Remove(mat);
                AbsenteDB.SaveChanges();
            }
        }

        public void DeleteSemestru(string semestru, string an, DateTime start, DateTime stop)
        {
            var sem = AbsenteDB.SemestreT.FirstOrDefault(x => x.Semestrul == semestru && x.Anul == an && x.DataStart == start && x.DataStop == stop);
            if (sem != null)
            {
                AbsenteDB.SemestreT.Remove(sem);
                AbsenteDB.SaveChanges();
            }
        }

        static public bool CheckEquality<T>(T[] first, T[] second)
        {
            return Enumerable.SequenceEqual(first, second);
        }

        static public bool FindDocument(DocsMotivareT doc, AbsenteDBEntities db)
        {
            foreach (var item in db.DocsMotivareT)
            {
                if (CheckEquality<byte>(doc.DocMotivare, item.DocMotivare))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
