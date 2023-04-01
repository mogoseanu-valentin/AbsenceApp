
namespace AbsenteAPP2.Forms
{
    partial class ViewImgDocF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbxImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxImg)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxImg
            // 
            this.pbxImg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbxImg.Location = new System.Drawing.Point(0, 0);
            this.pbxImg.Margin = new System.Windows.Forms.Padding(0);
            this.pbxImg.Name = "pbxImg";
            this.pbxImg.Size = new System.Drawing.Size(800, 450);
            this.pbxImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbxImg.TabIndex = 0;
            this.pbxImg.TabStop = false;
            // 
            // ViewImgDocF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pbxImg);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewImgDocF";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vizualizare imagine";
            this.Load += new System.EventHandler(this.ViewImgDocF_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxImg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxImg;
    }
}