namespace DragAndDrop
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            chkStayOnTop = new CheckBox();
            label1 = new Label();
            cmbOutputFormat = new ComboBox();
            label2 = new Label();
            tbQuality = new TrackBar();
            lblQualityValue = new Label();
            chkDeleteOriginal = new CheckBox();
            label3 = new Label();
            chkCustomOutputFolder = new CheckBox();
            txtCustomOutputPath = new TextBox();
            btnBrowseOutputFolder = new Button();
            dropAreaLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)tbQuality).BeginInit();
            SuspendLayout();
            // 
            // chkStayOnTop
            // 
            chkStayOnTop.AutoSize = true;
            chkStayOnTop.Location = new Point(12, 34);
            chkStayOnTop.Margin = new Padding(4);
            chkStayOnTop.Name = "chkStayOnTop";
            chkStayOnTop.Size = new Size(154, 25);
            chkStayOnTop.TabIndex = 0;
            chkStayOnTop.Text = "Поверх всех окон";
            chkStayOnTop.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(211, 9);
            label1.Name = "label1";
            label1.Size = new Size(144, 21);
            label1.TabIndex = 1;
            label1.Text = "Выходной формат:";
            // 
            // cmbOutputFormat
            // 
            cmbOutputFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOutputFormat.FormattingEnabled = true;
            cmbOutputFormat.Location = new Point(220, 33);
            cmbOutputFormat.Name = "cmbOutputFormat";
            cmbOutputFormat.Size = new Size(121, 29);
            cmbOutputFormat.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(220, 83);
            label2.Name = "label2";
            label2.Size = new Size(92, 21);
            label2.TabIndex = 3;
            label2.Text = "Качество %";
            // 
            // tbQuality
            // 
            tbQuality.Location = new Point(220, 115);
            tbQuality.Maximum = 100;
            tbQuality.Minimum = 1;
            tbQuality.Name = "tbQuality";
            tbQuality.Size = new Size(135, 45);
            tbQuality.TabIndex = 4;
            tbQuality.Value = 90;
            tbQuality.Scroll += TbQuality_Scroll;
            // 
            // lblQualityValue
            // 
            lblQualityValue.AutoSize = true;
            lblQualityValue.Location = new Point(318, 83);
            lblQualityValue.Name = "lblQualityValue";
            lblQualityValue.Size = new Size(37, 21);
            lblQualityValue.TabIndex = 5;
            lblQualityValue.Text = "000";
            // 
            // chkDeleteOriginal
            // 
            chkDeleteOriginal.AutoSize = true;
            chkDeleteOriginal.Location = new Point(12, 58);
            chkDeleteOriginal.Margin = new Padding(4);
            chkDeleteOriginal.Name = "chkDeleteOriginal";
            chkDeleteOriginal.Size = new Size(201, 25);
            chkDeleteOriginal.TabIndex = 6;
            chkDeleteOriginal.Text = "Переместить исходный в корзину";
            chkDeleteOriginal.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(74, 9);
            label3.Name = "label3";
            label3.Size = new Size(59, 21);
            label3.TabIndex = 7;
            label3.Text = "Опции";
            // 
            // chkCustomOutputFolder
            // 
            chkCustomOutputFolder.AutoSize = true;
            chkCustomOutputFolder.Location = new Point(12, 83);
            chkCustomOutputFolder.Margin = new Padding(4);
            chkCustomOutputFolder.Name = "chkCustomOutputFolder";
            chkCustomOutputFolder.Size = new Size(208, 25);
            chkCustomOutputFolder.TabIndex = 8;
            chkCustomOutputFolder.Text = "Использовать папку вых.";
            chkCustomOutputFolder.UseVisualStyleBackColor = true;
            chkCustomOutputFolder.CheckedChanged += ChkCustomOutputFolder_CheckedChanged;
            // 
            // txtCustomOutputPath
            // 
            txtCustomOutputPath.Enabled = false;
            txtCustomOutputPath.Location = new Point(12, 115);
            txtCustomOutputPath.Name = "txtCustomOutputPath";
            txtCustomOutputPath.ReadOnly = true;
            txtCustomOutputPath.Size = new Size(201, 29);
            txtCustomOutputPath.TabIndex = 9;
            // 
            // btnBrowseOutputFolder
            // 
            btnBrowseOutputFolder.Enabled = false;
            btnBrowseOutputFolder.Location = new Point(12, 150);
            btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
            btnBrowseOutputFolder.Size = new Size(201, 37);
            btnBrowseOutputFolder.TabIndex = 10;
            btnBrowseOutputFolder.Text = "Обзор...";
            btnBrowseOutputFolder.UseVisualStyleBackColor = true;
            btnBrowseOutputFolder.Click += BtnBrowseOutputFolder_Click;
            // 
            // dropAreaLabel
            // 
            dropAreaLabel.AutoSize = false;
            dropAreaLabel.BorderStyle = BorderStyle.FixedSingle;
            dropAreaLabel.Location = new Point(12, 199);
            dropAreaLabel.Name = "dropAreaLabel";
            dropAreaLabel.Size = new Size(334, 236);
            dropAreaLabel.TabIndex = 11;
            dropAreaLabel.Text = "Перетащите файлы изображений сюда";
            dropAreaLabel.TextAlign = ContentAlignment.TopLeft;
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(356, 447);
            Controls.Add(dropAreaLabel);
            Controls.Add(btnBrowseOutputFolder);
            Controls.Add(txtCustomOutputPath);
            Controls.Add(chkCustomOutputFolder);
            Controls.Add(label3);
            Controls.Add(chkDeleteOriginal);
            Controls.Add(lblQualityValue);
            Controls.Add(tbQuality);
            Controls.Add(label2);
            Controls.Add(cmbOutputFormat);
            Controls.Add(label1);
            Controls.Add(chkStayOnTop);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "Form";
            Text = "ImgConverter2";
            ((System.ComponentModel.ISupportInitialize)tbQuality).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkStayOnTop;
        private Label label1;
        private ComboBox cmbOutputFormat;
        private Label label2;
        private TrackBar tbQuality;
        private Label lblQualityValue;
        private CheckBox chkDeleteOriginal;
        private Label label3;
        private CheckBox chkCustomOutputFolder;
        private TextBox txtCustomOutputPath;
        private Button btnBrowseOutputFolder;
        private Label dropAreaLabel;
    }
}
