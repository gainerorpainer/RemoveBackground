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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            PictureBox_Input = new PictureBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            PictureBox_Output = new PictureBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            Button_LoadImage = new Button();
            Button_FromClipboard = new Button();
            label1 = new Label();
            TrackBar_Threshold = new TrackBar();
            Label_Threshold = new Label();
            flowLayoutPanel2 = new FlowLayoutPanel();
            Label_ComputeTime = new Label();
            Button_SaveFile = new Button();
            Button_ToClipboard = new Button();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            Timer_CheckClipboard = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)PictureBox_Input).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox_Output).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBar_Threshold).BeginInit();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // PictureBox_Input
            // 
            PictureBox_Input.BackgroundImage = (Image)resources.GetObject("PictureBox_Input.BackgroundImage");
            PictureBox_Input.BorderStyle = BorderStyle.Fixed3D;
            PictureBox_Input.Cursor = Cursors.Cross;
            PictureBox_Input.Dock = DockStyle.Fill;
            PictureBox_Input.Image = (Image)resources.GetObject("PictureBox_Input.Image");
            PictureBox_Input.Location = new Point(3, 103);
            PictureBox_Input.Name = "PictureBox_Input";
            PictureBox_Input.Size = new Size(547, 540);
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
            tableLayoutPanel1.Size = new Size(1106, 746);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // PictureBox_Output
            // 
            PictureBox_Output.BackgroundImage = (Image)resources.GetObject("PictureBox_Output.BackgroundImage");
            PictureBox_Output.BorderStyle = BorderStyle.Fixed3D;
            PictureBox_Output.Dock = DockStyle.Fill;
            PictureBox_Output.Image = (Image)resources.GetObject("PictureBox_Output.Image");
            PictureBox_Output.Location = new Point(556, 103);
            PictureBox_Output.Name = "PictureBox_Output";
            PictureBox_Output.Size = new Size(547, 540);
            PictureBox_Output.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox_Output.TabIndex = 2;
            PictureBox_Output.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 2);
            flowLayoutPanel1.Controls.Add(Button_LoadImage);
            flowLayoutPanel1.Controls.Add(Button_FromClipboard);
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Controls.Add(TrackBar_Threshold);
            flowLayoutPanel1.Controls.Add(Label_Threshold);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1100, 94);
            flowLayoutPanel1.TabIndex = 3;
            // 
            // Button_LoadImage
            // 
            Button_LoadImage.AutoSize = true;
            Button_LoadImage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Button_LoadImage.Location = new Point(3, 3);
            Button_LoadImage.Name = "Button_LoadImage";
            Button_LoadImage.Size = new Size(107, 35);
            Button_LoadImage.TabIndex = 0;
            Button_LoadImage.Text = "From File...";
            Button_LoadImage.UseVisualStyleBackColor = true;
            Button_LoadImage.Click += Button_LoadImage_Click;
            // 
            // Button_FromClipboard
            // 
            Button_FromClipboard.AutoSize = true;
            Button_FromClipboard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Button_FromClipboard.Enabled = false;
            Button_FromClipboard.Location = new Point(116, 3);
            Button_FromClipboard.Name = "Button_FromClipboard";
            Button_FromClipboard.Size = new Size(193, 35);
            Button_FromClipboard.TabIndex = 4;
            Button_FromClipboard.Text = "Insert from Clipboard";
            Button_FromClipboard.UseVisualStyleBackColor = true;
            Button_FromClipboard.Click += Button_FromClipboard_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(315, 0);
            label1.Name = "label1";
            label1.Size = new Size(94, 25);
            label1.TabIndex = 2;
            label1.Text = "Threshold:";
            // 
            // TrackBar_Threshold
            // 
            TrackBar_Threshold.Location = new Point(415, 3);
            TrackBar_Threshold.Maximum = 100;
            TrackBar_Threshold.Name = "TrackBar_Threshold";
            TrackBar_Threshold.Size = new Size(156, 69);
            TrackBar_Threshold.TabIndex = 1;
            TrackBar_Threshold.Value = 20;
            TrackBar_Threshold.ValueChanged += TrackBar_Threshold_ValueChanged;
            // 
            // Label_Threshold
            // 
            Label_Threshold.AutoSize = true;
            Label_Threshold.Location = new Point(577, 0);
            Label_Threshold.Name = "Label_Threshold";
            Label_Threshold.Size = new Size(147, 25);
            Label_Threshold.TabIndex = 3;
            Label_Threshold.Text = "Threshold = 20%";
            // 
            // flowLayoutPanel2
            // 
            tableLayoutPanel1.SetColumnSpan(flowLayoutPanel2, 2);
            flowLayoutPanel2.Controls.Add(Label_ComputeTime);
            flowLayoutPanel2.Controls.Add(Button_SaveFile);
            flowLayoutPanel2.Controls.Add(Button_ToClipboard);
            flowLayoutPanel2.Dock = DockStyle.Fill;
            flowLayoutPanel2.Location = new Point(3, 649);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(1100, 94);
            flowLayoutPanel2.TabIndex = 4;
            // 
            // Label_ComputeTime
            // 
            Label_ComputeTime.AutoSize = true;
            Label_ComputeTime.Location = new Point(3, 0);
            Label_ComputeTime.Name = "Label_ComputeTime";
            Label_ComputeTime.Size = new Size(194, 25);
            Label_ComputeTime.TabIndex = 0;
            Label_ComputeTime.Text = "Magic wand took 0 ms";
            // 
            // Button_SaveFile
            // 
            Button_SaveFile.AutoSize = true;
            Button_SaveFile.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Button_SaveFile.Location = new Point(203, 3);
            Button_SaveFile.Name = "Button_SaveFile";
            Button_SaveFile.Size = new Size(124, 35);
            Button_SaveFile.TabIndex = 1;
            Button_SaveFile.Text = "Save to File...";
            Button_SaveFile.UseVisualStyleBackColor = true;
            Button_SaveFile.Click += Button_SaveFile_Click;
            // 
            // Button_ToClipboard
            // 
            Button_ToClipboard.AutoSize = true;
            Button_ToClipboard.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Button_ToClipboard.Location = new Point(333, 3);
            Button_ToClipboard.Name = "Button_ToClipboard";
            Button_ToClipboard.Size = new Size(169, 35);
            Button_ToClipboard.TabIndex = 2;
            Button_ToClipboard.Text = "Copy to Clipboard";
            Button_ToClipboard.UseVisualStyleBackColor = true;
            Button_ToClipboard.Click += Button_ToClipboard_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Title = "Choose image";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.DefaultExt = "png";
            saveFileDialog1.Filter = "Png|*.png|JPEG|*.jpeg";
            // 
            // Timer_CheckClipboard
            // 
            Timer_CheckClipboard.Enabled = true;
            Timer_CheckClipboard.Tick += Timer_CheckClipboard_Tick;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1106, 746);
            Controls.Add(tableLayoutPanel1);
            KeyPreview = true;
            Name = "Form1";
            Text = "Remove Background";
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            ((System.ComponentModel.ISupportInitialize)PictureBox_Input).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PictureBox_Output).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TrackBar_Threshold).EndInit();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
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
        private Label Label_Threshold;
        private Label Label_ComputeTime;
        private Button Button_SaveFile;
        private SaveFileDialog saveFileDialog1;
        private Button Button_FromClipboard;
        private System.Windows.Forms.Timer Timer_CheckClipboard;
        private Button Button_ToClipboard;
    }
}
