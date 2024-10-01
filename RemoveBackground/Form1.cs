using RemoveBackground.FloodFill;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Media;

namespace RemoveBackground
{
    public partial class Form1 : Form
    {
        private struct SelectedPoint(PointF relpictureBoxCoords, Point? imageCoords)
        {
            /// <summary>
            /// Relative to its size
            /// </summary>
            public PointF RelPictureboxCoords { get; set; } = relpictureBoxCoords;

            public Point? ImageCoords { get; set; } = imageCoords;
        }

        private SelectedPoint? LastSelectedPoint { get; set; } = null;
        private FloodFillResult? LastFloodFillResult { get; set; } = null;

        public Form1()
        {
            InitializeComponent();

            PictureBox_Input_MouseClick(this, new MouseEventArgs(MouseButtons.Left, 1, PictureBox_Input.Width / 2, PictureBox_Input.Height / 2, 0));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                Button_FromClipboard_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.C))
            {
                Button_ToClipboard_Click(this, new EventArgs());
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Timer_CheckClipboard_Tick(object sender, EventArgs e)
        {
            Button_FromClipboard.Enabled = Clipboard.ContainsImage();
        }

        private void Button_LoadImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                SetImage(Image.FromFile(openFileDialog1.FileName));
            }
            catch (OutOfMemoryException)
            {
                SystemSounds.Beep.Play();
                return;
            }
        }

        private void Button_FromClipboard_Click(object sender, EventArgs e)
        {
            Image? image = Clipboard.GetImage();
            if (image is null)
                return;

            SetImage(image);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data is null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = ((string[])e.Data!.GetData(DataFormats.FileDrop)!).Length > 0 ? DragDropEffects.Move : DragDropEffects.None;
            else if (e.Data.GetDataPresent(DataFormats.Bitmap))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data is null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Image image;
                try
                {
                    image = Image.FromFile(((string[])e.Data!.GetData(DataFormats.FileDrop)!)[0]);
                }
                catch (Exception)
                {
                    return;
                }
                SetImage(image);
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                SetImage((Image)e.Data.GetData(DataFormats.Bitmap)!);
                return;
            }
        }

        private void PictureBox_Input_MouseClick(object sender, MouseEventArgs e)
        {
            // convert mouse to picture coords
            PointF relpictureBoxCoords = new((float)e.Location.X / PictureBox_Input.Width, (float)e.Location.Y / PictureBox_Input.Height);
            Point? imageCoords = ToImageCoords(e.Location);
            LastSelectedPoint = new SelectedPoint(relpictureBoxCoords, imageCoords);
            if (LastSelectedPoint.Value.ImageCoords is null)
                return;

            PictureBox_Input.Refresh();

            MagicWand();
        }

        private void PictureBox_Input_Paint(object sender, PaintEventArgs e)
        {
            if (LastSelectedPoint is null)
                return;

            int x = (int)(LastSelectedPoint.Value.RelPictureboxCoords.X * PictureBox_Input.Width);
            int y = (int)(LastSelectedPoint.Value.RelPictureboxCoords.Y * PictureBox_Input.Height);

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
            Label_Threshold.Text = $"Threshold = {TrackBar_Threshold.Value}%";

            if (LastSelectedPoint is null)
                return;

            MagicWand();
        }

        private void PictureBox_Input_Resize(object sender, EventArgs e)
        {
            // transform picturebox point after resize
            if (LastSelectedPoint is null)
                return;

            PictureBox_Input.Refresh();
        }

        private void Button_SaveFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            PictureBox_Output.Image.Save(saveFileDialog1.FileName);
        }

        private void Button_ToClipboard_Click(object sender, EventArgs e)
        {
            SetClipboardImage((Bitmap)PictureBox_Output.Image);
        }

        private void Button_Invert_Click(object sender, EventArgs e)
        {
            if (LastFloodFillResult is null)
                return;

            PictureBox_Output.Image = InvertAndCrop(LastFloodFillResult.Bitmap);
        }


        #region Function

        private void SetImage(Image image)
        {
            PictureBox_Input.Image = image;
            PictureBox_Output.Image = null;
            LastSelectedPoint = null;

            PictureBox_Input.Refresh();
        }

        private Point? ToImageCoords(Point mouseLocation)
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

            Stopwatch stopwatch = Stopwatch.StartNew();

            LastFloodFillResult = Algorithm.MagicWand((Bitmap)PictureBox_Input.Image, (Point)LastSelectedPoint.Value.ImageCoords, TrackBar_Threshold.Value / 100.0f);
            // crop image
            PictureBox_Output.Image = LastFloodFillResult.Bitmap.Clone(LastFloodFillResult.ROI, format: PixelFormat.Undefined);

            Label_ComputeTime.Text = $"Magic wand took {stopwatch.ElapsedMilliseconds} ms";
        }

        private static void SetClipboardImage(Bitmap image)
        {
            Clipboard.Clear();
            DataObject data = new();
            using MemoryStream pngMemStream = new();
            using MemoryStream dibMemStream = new();
            // As PNG. Gimp will prefer this over the other two.
            image.Save(pngMemStream, ImageFormat.Png);
            data.SetData("PNG", false, pngMemStream);
            // The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
            Clipboard.SetDataObject(data, copy: true);
        }

        private Bitmap InvertAndCrop(Bitmap bitmap)
        {
            var copy = new RawBitmap(bitmap);
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            unsafe
            {
                fixed (uint* pixels = copy.RawData)
                {
                    for (int y = 0; y < copy.Height; y++)
                    {
                        for (int x = 0; x < copy.Width; x++)
                        {
                            int i = x + y * copy.Width;
                            uint alphaInverted = Constants.MAX_ALPHA ^ (pixels[i] & ~Constants.RGB_MASK);
                            pixels[i] = (pixels[i] & Constants.RGB_MASK) | alphaInverted;

                            if (alphaInverted == 0)
                                continue;

                            if (x < minX)
                                minX = x;
                            if (y < minY)
                                minY = y;
                            if (x > maxX)
                                maxX = x;
                            if (y > maxY)
                                maxY = y;
                        }
                    }
                }
            }
            // crop fails if no pixels selected
            if (minX == int.MaxValue)
                return bitmap;
            // crop
            Rectangle roi = new(minX, minY, maxX - minX, maxY - minY);
            return copy.Bitmap.Clone(roi, bitmap.PixelFormat);
        }

        #endregion
    }
}
