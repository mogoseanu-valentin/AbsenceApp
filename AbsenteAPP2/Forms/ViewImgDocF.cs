using System;
using System.Windows.Forms;

namespace AbsenteAPP2.Forms
{
    public partial class ViewImgDocF : Form
    {
        public ViewImgDocF()
        {
            InitializeComponent();
        }

        private void ViewImgDocF_Load(object sender, EventArgs e)
        {
            pbxImg.Image = MainF.PMainF.DocMotivareImg;

            this.Width = MainF.PMainF.DocMotivareImg.Width;
            this.Height = MainF.PMainF.DocMotivareImg.Height;
        }
    }
}
