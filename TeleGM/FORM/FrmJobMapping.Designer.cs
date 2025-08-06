namespace GmTelematics
{
    partial class FrmJobMapping
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridEdit = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.Fsave = new System.Windows.Forms.Button();
            this.Fnew = new System.Windows.Forms.Button();
            this.Fdelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridEdit
            // 
            this.dataGridEdit.AllowUserToAddRows = false;
            this.dataGridEdit.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Beige;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dataGridEdit.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridEdit.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridEdit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.DarkSeaGreen;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridEdit.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridEdit.Location = new System.Drawing.Point(12, 63);
            this.dataGridEdit.Name = "dataGridEdit";
            this.dataGridEdit.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridEdit.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridEdit.RowHeadersWidth = 55;
            this.dataGridEdit.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Beige;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            this.dataGridEdit.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridEdit.RowTemplate.Height = 23;
            this.dataGridEdit.Size = new System.Drawing.Size(947, 412);
            this.dataGridEdit.TabIndex = 3;
            this.dataGridEdit.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridEdit_CellClick);
            this.dataGridEdit.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridEdit_EditingControlShowing);
            this.dataGridEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridEdit_KeyDown);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(794, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(165, 37);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "CLOSE";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // Fsave
            // 
            this.Fsave.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fsave.Location = new System.Drawing.Point(649, 12);
            this.Fsave.Name = "Fsave";
            this.Fsave.Size = new System.Drawing.Size(139, 37);
            this.Fsave.TabIndex = 8;
            this.Fsave.Text = "MAP SAVE";
            this.Fsave.UseVisualStyleBackColor = true;
            this.Fsave.Click += new System.EventHandler(this.Fsave_Click);
            // 
            // Fnew
            // 
            this.Fnew.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fnew.Location = new System.Drawing.Point(12, 12);
            this.Fnew.Name = "Fnew";
            this.Fnew.Size = new System.Drawing.Size(139, 37);
            this.Fnew.TabIndex = 11;
            this.Fnew.Text = "ITEM ADD";
            this.Fnew.UseVisualStyleBackColor = true;
            this.Fnew.Click += new System.EventHandler(this.Fnew_Click);
            // 
            // Fdelete
            // 
            this.Fdelete.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fdelete.Location = new System.Drawing.Point(157, 12);
            this.Fdelete.Name = "Fdelete";
            this.Fdelete.Size = new System.Drawing.Size(139, 37);
            this.Fdelete.TabIndex = 10;
            this.Fdelete.Text = "ITEM DELETE";
            this.Fdelete.UseVisualStyleBackColor = true;
            this.Fdelete.Click += new System.EventHandler(this.Fdelete_Click);
            // 
            // FrmJobMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 487);
            this.ControlBox = false;
            this.Controls.Add(this.Fnew);
            this.Controls.Add(this.Fdelete);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.Fsave);
            this.Controls.Add(this.dataGridEdit);
            this.Name = "FrmJobMapping";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JOB FILE MAPPING";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmJobMapping_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEdit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridEdit;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button Fsave;
        private System.Windows.Forms.Button Fnew;
        private System.Windows.Forms.Button Fdelete;
    }
}