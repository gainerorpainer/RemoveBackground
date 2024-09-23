namespace RemoveBackground
{
    public partial class Form1 : Form
    {
        private struct SelectedPoint(Point pictureBoxCoords, Point? imageCoords)
        {
            /// <summary>
            /// Relative to its size
            /// </summary>
            public Point PictureboxCoords { get; set; } = pictureBoxCoords;
            public Point? ImageCoords { get; set; } = imageCoords;
        }

        private SelectedPoint? LastSelectedPoint { get; set; } = null;

        private Size LastPictureBoxInputSize { get; set; }

        public Form1()
        {
            InitializeComponent();

            LastPictureBoxInputSize = PictureBox_Input.Size;
        }

        private void Button_LoadImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            PictureBox_Input.Image = Image.FromFile(openFileDialog1.FileName);
        }

        private void PictureBox_Input_MouseClick(object sender, MouseEventArgs e)
        {
            // convert mouse to picture coords
            LastSelectedPoint = new SelectedPoint(e.Location, ToPictureCoords(e.Location));
            if (LastSelectedPoint.Value.ImageCoords is null)
                return;

            PictureBox_Input.Invalidate();

            MagicWand();
        }

        private void PictureBox_Input_Paint(object sender, PaintEventArgs e)
        {
            if (LastSelectedPoint is null)
                return;

            int x = LastSelectedPoint.Value.PictureboxCoords.X;
            int y = LastSelectedPoint.Value.PictureboxCoords.Y;

            const int LINE_LENGTH = 20; // Length of crosshair lines
            const int LINE_THICKNESS = 2; // Thickness of crosshair lines

            // Create a pen to draw the crosshair
            Pen pen = new(Color.Red, LINE_THICKNESS);
            // Draw the horizontal line
            e.Graphics.DrawLine(pen, x - LINE_LENGTH, y, x + LINE_LENGTH, y);
            // Draw the vertical line 
            e.Graphics.DrawLine(pen, x, y - LINE_LENGTH, x, y + LINE_LENGTH);
        }

        private void TrackBar_Threshold_ValueChanged(object sender, EventArgs e)
        {
            if (LastSelectedPoint is null)
                return;

            MagicWand();
        }

        private void PictureBox_Input_Resize(object sender, EventArgs e)
        {
            // transform picturebox point after resize
            if (LastSelectedPoint is null)
                return;

            float scaleX = PictureBox_Input.Width / LastPictureBoxInputSize.Width;
            float scaleY = PictureBox_Input.Height / LastPictureBoxInputSize.Height;

            LastSelectedPoint.PictureboxCoords = new Point();

            LastPictureBoxInputSize = PictureBox_Input.Size;
            PictureBox_Input.Refresh();
        }

        private Point? ToPictureCoords(Point mouseLocation)
        {
            // Get PictureBox and image dimensions
            int pbWidth = PictureBox_Input.Width;
            int pbHeight = PictureBox_Input.Height;
            int imgWidth = PictureBox_Input.Image.Width;
            int imgHeight = PictureBox_Input.Image.Height;

            // Calculate aspect ratios
            double pbAspectRatio = (double)pbWidth / pbHeight;
            double imgAspectRatio = (double)imgWidth / imgHeight;

            int newImgWidth, newImgHeight;
            int offsetX = 0, offsetY = 0;

            // Calculate the size of the image as it appears in the PictureBox
            if (imgAspectRatio > pbAspectRatio) // Image is wider than PictureBox
            {
                newImgWidth = pbWidth;
                newImgHeight = (int)(pbWidth / imgAspectRatio);
                offsetY = (pbHeight - newImgHeight) / 2; // Center vertically
            }
            else // Image is taller than PictureBox
            {
                newImgHeight = pbHeight;
                newImgWidth = (int)(pbHeight * imgAspectRatio);
                offsetX = (pbWidth - newImgWidth) / 2; // Center horizontally
            }

            // Check if the click is inside the image area
            if (mouseLocation.X < offsetX || mouseLocation.X > offsetX + newImgWidth
                || mouseLocation.Y < offsetY || mouseLocation.Y > offsetY + newImgHeight)
                // The click is outside the image
                return null;

            // Translate mouse coordinates to image coordinates
            int imageX = (int)((float)(mouseLocation.X - offsetX) / newImgWidth * imgWidth);
            int imageY = (int)((float)(mouseLocation.Y - offsetY) / newImgHeight * imgHeight);

            return new Point(imageX, imageY);
        }

        private void MagicWand()
        {
            if (LastSelectedPoint is null || LastSelectedPoint.Value.ImageCoords is null)
                throw new InvalidOperationException($"Must first set {nameof(LastSelectedPoint)} property");

            var alphaMaskedResult = FloodFill.MagicWand((Bitmap)PictureBox_Input.Image, (Point)LastSelectedPoint.Value.ImageCoords, TrackBar_Threshold.Value / 100.0f);
            PictureBox_Output.Image = alphaMaskedResult.Bitmap;
        }
    }
}
