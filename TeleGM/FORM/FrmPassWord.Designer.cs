namespace GmTelematics
{
    partial class FrmPassWord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPassWord));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblMsg = new System.Windows.Forms.Label();
            this.secLlb1 = new AxisAnalogLibrary.AxiLabelX();
            this.secLlb2 = new AxisAnalogLibrary.AxiLabelX();
            this.btnOnScreenKeyBoard = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.secLlb1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.secLlb2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(302, 182);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(105, 29);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(191, 182);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 29);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(75, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Please, Input PassWord";
            this.label1.DoubleClick += new System.EventHandler(this.label1_DoubleClick);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(112, 92);
            this.txtPassword.MaxLength = 20;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(195, 29);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.ForeColor = System.Drawing.Color.Crimson;
            this.lblMsg.Location = new System.Drawing.Point(136, 133);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 16);
            this.lblMsg.TabIndex = 6;
            this.lblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // secLlb1
            // 
            this.secLlb1.Enabled = true;
            this.secLlb1.Location = new System.Drawing.Point(365, 1);
            this.secLlb1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.secLlb1.Name = "secLlb1";
            this.secLlb1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("secLlb1.OcxState")));
            this.secLlb1.Size = new System.Drawing.Size(25, 19);
            this.secLlb1.TabIndex = 70;
            this.secLlb1.OnMouseUp += new AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEventHandler(this.secLlb1_OnMouseUp);
            // 
            // secLlb2
            // 
            this.secLlb2.Enabled = true;
            this.secLlb2.Location = new System.Drawing.Point(393, 1);
            this.secLlb2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.secLlb2.Name = "secLlb2";
            this.secLlb2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("secLlb2.OcxState")));
            this.secLlb2.Size = new System.Drawing.Size(25, 19);
            this.secLlb2.TabIndex = 69;
            this.secLlb2.OnMouseUp += new AxisAnalogLibrary.IiLabelXEvents_OnMouseUpEventHandler(this.secLlb2_OnMouseUp);
            // 
            // btnOnScreenKeyBoard
            // 
            this.btnOnScreenKeyBoard.Location = new System.Drawing.Point(7, 182);
            this.btnOnScreenKeyBoard.Name = "btnOnScreenKeyBoard";
            this.btnOnScreenKeyBoard.Size = new System.Drawing.Size(109, 29);
            this.btnOnScreenKeyBoard.TabIndex = 71;
            this.btnOnScreenKeyBoard.Text = "Open KeyBoard";
            this.btnOnScreenKeyBoard.UseVisualStyleBackColor = true;
            this.btnOnScreenKeyBoard.Click += new System.EventHandler(this.btnOnScreenKeyBoard_Click);
            // 
            // FrmPassWord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 223);
            this.ControlBox = false;
            this.Controls.Add(this.btnOnScreenKeyBoard);
            this.Controls.Add(this.secLlb1);
            this.Controls.Add(this.secLlb2);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Courier New", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FrmPassWord";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPassWord_Load);
            this.Shown += new System.EventHandler(this.FrmPassWord_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.secLlb1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.secLlb2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblMsg;
        private AxisAnalogLibrary.AxiLabelX secLlb1;
        private AxisAnalogLibrary.AxiLabelX secLlb2;
        private System.Windows.Forms.Button btnOnScreenKeyBoard;
    }
}