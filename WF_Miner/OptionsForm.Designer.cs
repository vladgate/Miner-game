namespace WF_Miner
{
    partial class OptionsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtBoxWidth = new System.Windows.Forms.TextBox();
            this.txtBoxHeight = new System.Windows.Forms.TextBox();
            this.txtBoxAmountMines = new System.Windows.Forms.TextBox();
            this.radioBtnCustom = new System.Windows.Forms.RadioButton();
            this.radioBtnEasy = new System.Windows.Forms.RadioButton();
            this.radioBtnMedium = new System.Windows.Forms.RadioButton();
            this.radioBtnHard = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(205, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mines (10-777):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(205, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Height (9-50):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(205, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Width (9-65):";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(111, 118);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(62, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(202, 118);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtBoxWidth
            // 
            this.txtBoxWidth.Location = new System.Drawing.Point(300, 56);
            this.txtBoxWidth.Name = "txtBoxWidth";
            this.txtBoxWidth.Size = new System.Drawing.Size(46, 20);
            this.txtBoxWidth.TabIndex = 3;
            this.txtBoxWidth.Text = "9";
            this.txtBoxWidth.Enter += new System.EventHandler(this.txtBoxWidth_Enter);
            this.txtBoxWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtBox_KeyPress);
            this.txtBoxWidth.Leave += new System.EventHandler(this.txtBoxWidth_Leave);
            // 
            // txtBoxHeight
            // 
            this.txtBoxHeight.Location = new System.Drawing.Point(300, 83);
            this.txtBoxHeight.Name = "txtBoxHeight";
            this.txtBoxHeight.Size = new System.Drawing.Size(46, 20);
            this.txtBoxHeight.TabIndex = 2;
            this.txtBoxHeight.Text = "9";
            this.txtBoxHeight.Enter += new System.EventHandler(this.txtBoxHeight_Enter);
            this.txtBoxHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtBox_KeyPress);
            this.txtBoxHeight.Leave += new System.EventHandler(this.txtBoxHeight_Leave);
            // 
            // txtBoxAmountMines
            // 
            this.txtBoxAmountMines.Location = new System.Drawing.Point(300, 30);
            this.txtBoxAmountMines.Name = "txtBoxAmountMines";
            this.txtBoxAmountMines.Size = new System.Drawing.Size(46, 20);
            this.txtBoxAmountMines.TabIndex = 1;
            this.txtBoxAmountMines.Text = "10";
            this.txtBoxAmountMines.Enter += new System.EventHandler(this.txtBoxAmountMines_Enter);
            this.txtBoxAmountMines.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtBox_KeyPress);
            this.txtBoxAmountMines.Leave += new System.EventHandler(this.txtBoxAmountMines_Leave);
            // 
            // radioBtnCustom
            // 
            this.radioBtnCustom.AutoSize = true;
            this.radioBtnCustom.Location = new System.Drawing.Point(208, 13);
            this.radioBtnCustom.Name = "radioBtnCustom";
            this.radioBtnCustom.Size = new System.Drawing.Size(60, 17);
            this.radioBtnCustom.TabIndex = 6;
            this.radioBtnCustom.Text = "Custom";
            this.radioBtnCustom.UseVisualStyleBackColor = true;
            this.radioBtnCustom.Click += new System.EventHandler(this.RadioBtnCustom_Click);
            // 
            // radioBtnEasy
            // 
            this.radioBtnEasy.AutoSize = true;
            this.radioBtnEasy.Checked = true;
            this.radioBtnEasy.Location = new System.Drawing.Point(14, 12);
            this.radioBtnEasy.Name = "radioBtnEasy";
            this.radioBtnEasy.Size = new System.Drawing.Size(122, 17);
            this.radioBtnEasy.TabIndex = 6;
            this.radioBtnEasy.TabStop = true;
            this.radioBtnEasy.Text = "Easy (9x9, 10 mines)";
            this.radioBtnEasy.UseVisualStyleBackColor = true;
            this.radioBtnEasy.Click += new System.EventHandler(this.RadioBtNotCustom_Click);
            // 
            // radioBtnMedium
            // 
            this.radioBtnMedium.AutoSize = true;
            this.radioBtnMedium.Location = new System.Drawing.Point(14, 48);
            this.radioBtnMedium.Name = "radioBtnMedium";
            this.radioBtnMedium.Size = new System.Drawing.Size(148, 17);
            this.radioBtnMedium.TabIndex = 6;
            this.radioBtnMedium.Text = "Medium (16x16, 40 mines)";
            this.radioBtnMedium.UseVisualStyleBackColor = true;
            this.radioBtnMedium.Click += new System.EventHandler(this.RadioBtNotCustom_Click);
            // 
            // radioBtnHard
            // 
            this.radioBtnHard.AutoSize = true;
            this.radioBtnHard.Location = new System.Drawing.Point(14, 86);
            this.radioBtnHard.Name = "radioBtnHard";
            this.radioBtnHard.Size = new System.Drawing.Size(134, 17);
            this.radioBtnHard.TabIndex = 6;
            this.radioBtnHard.Text = "Hard (16x30, 99 mines)";
            this.radioBtnHard.UseVisualStyleBackColor = true;
            this.radioBtnHard.Click += new System.EventHandler(this.RadioBtNotCustom_Click);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 151);
            this.Controls.Add(this.radioBtnHard);
            this.Controls.Add(this.radioBtnMedium);
            this.Controls.Add(this.radioBtnEasy);
            this.Controls.Add(this.radioBtnCustom);
            this.Controls.Add(this.txtBoxWidth);
            this.Controls.Add(this.txtBoxHeight);
            this.Controls.Add(this.txtBoxAmountMines);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.TextBox txtBoxHeight;
        internal System.Windows.Forms.TextBox txtBoxWidth;
        internal System.Windows.Forms.TextBox txtBoxAmountMines;
        private System.Windows.Forms.RadioButton radioBtnCustom;
        private System.Windows.Forms.RadioButton radioBtnEasy;
        private System.Windows.Forms.RadioButton radioBtnMedium;
        private System.Windows.Forms.RadioButton radioBtnHard;
    }
}