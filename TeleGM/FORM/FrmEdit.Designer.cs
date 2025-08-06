namespace GmTelematics
{
    partial class FrmEdit
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridEdit = new System.Windows.Forms.DataGridView();
            this.cbJobFiles = new System.Windows.Forms.ComboBox();
            this.Fsave = new System.Windows.Forms.Button();
            this.Fdelete = new System.Windows.Forms.Button();
            this.Fnew = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.toolTipText = new System.Windows.Forms.ToolTip(this.components);
            this.FSaveAs = new System.Windows.Forms.Button();
            this.FSaveCsv = new System.Windows.Forms.Button();
            this.pannelTextFinder = new System.Windows.Forms.Panel();
            this.btnFinderClose = new System.Windows.Forms.Button();
            this.txtTargetFind = new System.Windows.Forms.TextBox();
            this.lblFind = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEdit)).BeginInit();
            this.pannelTextFinder.SuspendLayout();
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
            this.dataGridEdit.Location = new System.Drawing.Point(4, 44);
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
            this.dataGridEdit.Size = new System.Drawing.Size(1329, 318);
            this.dataGridEdit.TabIndex = 2;
            this.dataGridEdit.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridEdit_CellClick);
            this.dataGridEdit.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridEdit_CellEnter);
            this.dataGridEdit.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridEdit_CellMouseDown);
            this.dataGridEdit.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridEdit_CellMouseUp);
            this.dataGridEdit.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridEdit_CellValueChanged);
            this.dataGridEdit.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridEdit_RowHeaderMouseClick);
            this.dataGridEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridEdit_KeyDown);
            // 
            // cbJobFiles
            // 
            this.cbJobFiles.BackColor = System.Drawing.Color.PeachPuff;
            this.cbJobFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbJobFiles.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbJobFiles.FormattingEnabled = true;
            this.cbJobFiles.Location = new System.Drawing.Point(4, 5);
            this.cbJobFiles.Name = "cbJobFiles";
            this.cbJobFiles.Size = new System.Drawing.Size(536, 26);
            this.cbJobFiles.TabIndex = 3;
            this.cbJobFiles.SelectedIndexChanged += new System.EventHandler(this.cbJobFiles_SelectedIndexChanged);
            // 
            // Fsave
            // 
            this.Fsave.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fsave.Location = new System.Drawing.Point(541, 4);
            this.Fsave.Name = "Fsave";
            this.Fsave.Size = new System.Drawing.Size(75, 37);
            this.Fsave.TabIndex = 4;
            this.Fsave.Text = "SAVE";
            this.Fsave.UseVisualStyleBackColor = true;
            this.Fsave.Click += new System.EventHandler(this.Fsave_Click);
            // 
            // Fdelete
            // 
            this.Fdelete.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fdelete.Location = new System.Drawing.Point(615, 4);
            this.Fdelete.Name = "Fdelete";
            this.Fdelete.Size = new System.Drawing.Size(75, 37);
            this.Fdelete.TabIndex = 5;
            this.Fdelete.Text = "DELETE";
            this.Fdelete.UseVisualStyleBackColor = true;
            this.Fdelete.Click += new System.EventHandler(this.Fdelete_Click);
            // 
            // Fnew
            // 
            this.Fnew.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fnew.Location = new System.Drawing.Point(689, 4);
            this.Fnew.Name = "Fnew";
            this.Fnew.Size = new System.Drawing.Size(75, 37);
            this.Fnew.TabIndex = 6;
            this.Fnew.Text = "NEW";
            this.Fnew.UseVisualStyleBackColor = true;
            this.Fnew.Click += new System.EventHandler(this.Fnew_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(1253, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(80, 37);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "CLOSE";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // FSaveAs
            // 
            this.FSaveAs.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FSaveAs.Location = new System.Drawing.Point(763, 4);
            this.FSaveAs.Name = "FSaveAs";
            this.FSaveAs.Size = new System.Drawing.Size(75, 37);
            this.FSaveAs.TabIndex = 9;
            this.FSaveAs.Text = "SAVE As";
            this.FSaveAs.UseVisualStyleBackColor = true;
            this.FSaveAs.Click += new System.EventHandler(this.FSaveAs_Click);
            // 
            // FSaveCsv
            // 
            this.FSaveCsv.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FSaveCsv.Location = new System.Drawing.Point(837, 4);
            this.FSaveCsv.Name = "FSaveCsv";
            this.FSaveCsv.Size = new System.Drawing.Size(99, 37);
            this.FSaveCsv.TabIndex = 10;
            this.FSaveCsv.Text = "Copy Excel";
            this.FSaveCsv.UseVisualStyleBackColor = true;
            this.FSaveCsv.Click += new System.EventHandler(this.FSaveCsv_Click);
            // 
            // pannelTextFinder
            // 
            this.pannelTextFinder.BackColor = System.Drawing.SystemColors.Control;
            this.pannelTextFinder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pannelTextFinder.Controls.Add(this.btnFinderClose);
            this.pannelTextFinder.Controls.Add(this.txtTargetFind);
            this.pannelTextFinder.Controls.Add(this.lblFind);
            this.pannelTextFinder.Location = new System.Drawing.Point(4, 368);
            this.pannelTextFinder.Name = "pannelTextFinder";
            this.pannelTextFinder.Size = new System.Drawing.Size(450, 37);
            this.pannelTextFinder.TabIndex = 13;
            this.pannelTextFinder.Visible = false;
            // 
            // btnFinderClose
            // 
            this.btnFinderClose.Location = new System.Drawing.Point(390, 3);
            this.btnFinderClose.Name = "btnFinderClose";
            this.btnFinderClose.Size = new System.Drawing.Size(55, 29);
            this.btnFinderClose.TabIndex = 2;
            this.btnFinderClose.Text = "Close";
            this.btnFinderClose.UseVisualStyleBackColor = true;
            this.btnFinderClose.Click += new System.EventHandler(this.btnFinderClose_Click);
            // 
            // txtTargetFind
            // 
            this.txtTargetFind.BackColor = System.Drawing.Color.Yellow;
            this.txtTargetFind.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetFind.Location = new System.Drawing.Point(86, 4);
            this.txtTargetFind.Name = "txtTargetFind";
            this.txtTargetFind.Size = new System.Drawing.Size(300, 26);
            this.txtTargetFind.TabIndex = 1;
            this.txtTargetFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTargetFind_KeyDown);
            // 
            // lblFind
            // 
            this.lblFind.BackColor = System.Drawing.Color.Black;
            this.lblFind.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFind.ForeColor = System.Drawing.Color.White;
            this.lblFind.Location = new System.Drawing.Point(1, 1);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(448, 33);
            this.lblFind.TabIndex = 0;
            this.lblFind.Text = "Find Text";
            this.lblFind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FrmEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1341, 581);
            this.ControlBox = false;
            this.Controls.Add(this.pannelTextFinder);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.FSaveCsv);
            this.Controls.Add(this.FSaveAs);
            this.Controls.Add(this.Fnew);
            this.Controls.Add(this.Fdelete);
            this.Controls.Add(this.Fsave);
            this.Controls.Add(this.cbJobFiles);
            this.Controls.Add(this.dataGridEdit);
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "FrmEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JOB FILE EDITOR";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmEdit_Load);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FrmEdit_MouseDoubleClick);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridEdit)).EndInit();
            this.pannelTextFinder.ResumeLayout(false);
            this.pannelTextFinder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridEdit;
        private System.Windows.Forms.ComboBox cbJobFiles;
        private System.Windows.Forms.Button Fsave;
        private System.Windows.Forms.Button Fdelete;
        private System.Windows.Forms.Button Fnew;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ToolTip toolTipText;
        private System.Windows.Forms.Button FSaveAs;
        private System.Windows.Forms.Button FSaveCsv;
        private System.Windows.Forms.Panel pannelTextFinder;
        private System.Windows.Forms.Button btnFinderClose;
        private System.Windows.Forms.TextBox txtTargetFind;
        private System.Windows.Forms.Label lblFind;
    }
}