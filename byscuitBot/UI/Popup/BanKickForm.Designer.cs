namespace byscuitBot.UI.Popup
{
    partial class BanKickForm
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
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.reasonTxt = new MetroFramework.Controls.MetroTextBox();
            this.kickBtn = new MetroFramework.Controls.MetroButton();
            this.banCBox = new MetroFramework.Controls.MetroComboBox();
            this.kickUsrLbl = new MetroFramework.Controls.MetroLabel();
            this.eraseMsgCBox = new MetroFramework.Controls.MetroComboBox();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(24, 64);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(54, 19);
            this.metroLabel1.TabIndex = 0;
            this.metroLabel1.Text = "Reason:";
            // 
            // reasonTxt
            // 
            this.reasonTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reasonTxt.Location = new System.Drawing.Point(24, 86);
            this.reasonTxt.Name = "reasonTxt";
            this.reasonTxt.Size = new System.Drawing.Size(285, 23);
            this.reasonTxt.TabIndex = 1;
            // 
            // kickBtn
            // 
            this.kickBtn.Location = new System.Drawing.Point(24, 116);
            this.kickBtn.Name = "kickBtn";
            this.kickBtn.Size = new System.Drawing.Size(80, 23);
            this.kickBtn.TabIndex = 2;
            this.kickBtn.Text = "Kick";
            this.kickBtn.Click += new System.EventHandler(this.KickBtn_Click);
            // 
            // banCBox
            // 
            this.banCBox.FormattingEnabled = true;
            this.banCBox.ItemHeight = 23;
            this.banCBox.Location = new System.Drawing.Point(24, 146);
            this.banCBox.Name = "banCBox";
            this.banCBox.Size = new System.Drawing.Size(285, 29);
            this.banCBox.TabIndex = 3;
            this.banCBox.SelectedIndexChanged += new System.EventHandler(this.BanCBox_SelectedIndexChanged);
            // 
            // kickUsrLbl
            // 
            this.kickUsrLbl.AutoSize = true;
            this.kickUsrLbl.Dock = System.Windows.Forms.DockStyle.Right;
            this.kickUsrLbl.Location = new System.Drawing.Point(163, 60);
            this.kickUsrLbl.Name = "kickUsrLbl";
            this.kickUsrLbl.Size = new System.Drawing.Size(147, 19);
            this.kickUsrLbl.TabIndex = 4;
            this.kickUsrLbl.Text = "user#0000(0000000000)";
            // 
            // eraseMsgCBox
            // 
            this.eraseMsgCBox.FormattingEnabled = true;
            this.eraseMsgCBox.ItemHeight = 23;
            this.eraseMsgCBox.Location = new System.Drawing.Point(22, 181);
            this.eraseMsgCBox.Name = "eraseMsgCBox";
            this.eraseMsgCBox.Size = new System.Drawing.Size(285, 29);
            this.eraseMsgCBox.TabIndex = 5;
            this.eraseMsgCBox.Visible = false;
            // 
            // BanKickForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 227);
            this.Controls.Add(this.eraseMsgCBox);
            this.Controls.Add(this.kickUsrLbl);
            this.Controls.Add(this.banCBox);
            this.Controls.Add(this.kickBtn);
            this.Controls.Add(this.reasonTxt);
            this.Controls.Add(this.metroLabel1);
            this.Name = "BanKickForm";
            this.Text = "Ban/Kick User";
            this.Load += new System.EventHandler(this.BanKickForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTextBox reasonTxt;
        private MetroFramework.Controls.MetroButton kickBtn;
        private MetroFramework.Controls.MetroComboBox banCBox;
        private MetroFramework.Controls.MetroLabel kickUsrLbl;
        private MetroFramework.Controls.MetroComboBox eraseMsgCBox;
    }
}