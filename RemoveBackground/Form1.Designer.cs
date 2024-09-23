namespace RemoveBackground
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            PictureBox_Input = new PictureBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            PictureBox_Output = new PictureBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            Button_LoadImage = new Button();
            flowLayoutPanel2 = new FlowLayoutPanel();
            label1 = new Label();
            TrackBar_Threshold = new TrackBar();
            openFileDialog1 = new OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)PictureBox_Input).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox_Output).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBar_Threshold).BeginInit();
            SuspendLayout();
            // 
            // PictureBox_Input
            // 
            PictureBox_Input.BorderStyle = BorderStyle.Fixed3D;
            PictureBox_Input.Cursor = Cursors.Cross;
            PictureBox_Input.Dock = DockStyle.Fill;
            PictureBox_Input.Image = (Image)resources.GetObject("PictureBox_Input.Image");
            PictureBox_Input.Location = new Point(3, 103);
            PictureBox_Input.Name = "PictureBox_Input";
            PictureBox_Input.Size = new Size(394, 244);
            PictureBox_Input.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox_Input.TabIndex = 0;
            PictureBox_Input.TabStop = false;
            PictureBox_Input.Paint += PictureBox_Input_Paint;
            PictureBox_Input.MouseClick += PictureBox_Input_MouseClick;
            PictureBox_Input.Resize += PictureBox_Input_Resize;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(PictureBox_Input, 0, 1);
            tableLayoutPanel1.Controls.Add(PictureBox_Output, 1, 1);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 0);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel2, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tableLayoutPanel1.Size = new Size(800, 450);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // PictureBox_Output
            // 
            PictureBox_Output.BorderStyle = BorderStyle.Fixed3D;
            PictureBox_Output.Dock = DockStyle.Fill;
            PictureBox_Output.Image = (Image)resources.GetObject("PictureBox_Output.Image");
            PictureBox_Output.Location = new Point(403, 103);
            PictureBox_Output.Name = "PictureBox_Output";
            PictureBox_Output.Size = new Size(394, 244);
            PictureBox_Output.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox_Output.TabIndex = 2;
            PictureBox_Output.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 2);
            flowLayoutPanel1.Controls.Add(Button_LoadImage);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(794, 94);
            flowLayoutPanel1.TabIndex = 3;
            // 
            // Button_LoadImage
            // 
            Button_LoadImage.Location = new Point(3, 3);
            Button_LoadImage.Name = "Button_LoadImage";
            Button_LoadImage.Size = new Size(112, 34);
            Button_LoadImage.TabIndex = 0;
            Button_LoadImage.Text = "From File...";
            Button_LoadImage.UseVisualStyleBackColor = true;
            Button_LoadImage.Click += Button_LoadImage_Click;
            // 
            // flowLayoutPanel2
            // 
            tableLayoutPanel1.SetColumnSpan(flowLayoutPanel2, 2);
            flowLayoutPanel2.Controls.Add(label1);
            flowLayoutPanel2.Controls.Add(TrackBar_Threshold);
            flowLayoutPanel2.Dock = DockStyle.Fill;
            flowLayoutPanel2.Location = new Point(3, 353);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(794, 94);
            flowLayoutPanel2.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(94, 25);
            label1.TabIndex = 2;
            label1.Text = "Threshold:";
            // 
            // TrackBar_Threshold
            // 
            TrackBar_Threshold.Location = new Point(103, 3);
            TrackBar_Threshold.Maximum = 100;
            TrackBar_Threshold.Name = "TrackBar_Threshold";
            TrackBar_Threshold.Size = new Size(156, 69);
            TrackBar_Threshold.TabIndex = 1;
            TrackBar_Threshold.ValueChanged += TrackBar_Threshold_ValueChanged;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Title = "Choose image";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Text = "Remove Background";
            ((System.ComponentModel.ISupportInitialize)PictureBox_Input).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PictureBox_Output).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBar_Threshold).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox PictureBox_Input;
        private TableLayoutPanel tableLayoutPanel1;
        private PictureBox PictureBox_Output;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button Button_LoadImage;
        private FlowLayoutPanel flowLayoutPanel2;
        private OpenFileDialog openFileDialog1;
        private TrackBar TrackBar_Threshold;
        private Label label1;
    }
}
