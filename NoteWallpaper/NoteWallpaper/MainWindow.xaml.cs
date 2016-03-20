using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NoteWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static System.Drawing.Color BackgroundColor;
        private static System.Drawing.Color FontColor;
        private static Font FontDefault;
        private bool init = false;
        public MainWindow()
        {
            InitializeComponent();
            BackgroundColor = new System.Drawing.Color();
            BackgroundColor = System.Drawing.Color.FromArgb(192, 192, 192);
            FontColor = new System.Drawing.Color();
            FontColor = System.Drawing.Color.FromArgb(0, 0, 255);
            txtWidth.Text = Screen.PrimaryScreen.Bounds.Width.ToString();
            txtHeight.Text = Screen.PrimaryScreen.Bounds.Height.ToString();
            FontDefault = new Font("宋体", 24);
            btnFontColor.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(FontColor.A, FontColor.R, FontColor.G, FontColor.B));
            btnBGColorPicker.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(BackgroundColor.A, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
            this.Width = Screen.PrimaryScreen.Bounds.Width * 0.7;
            this.Height = Screen.PrimaryScreen.Bounds.Height * 0.7;
            this.rtbPad.Background = btnBGColorPicker.Background;
            this.rtbPad.Foreground = btnFontColor.Foreground;
            this.rtbPad.FontFamily = new System.Windows.Media.FontFamily(FontDefault.FontFamily.Name);
            this.rtbPad.FontSize = FontDefault.Size;
            this.rtbPad.Height = this.Height - btnBGColorPicker.Height - 50;
            this.rtbPad.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.rtbPad.Document.PageWidth = 10000;
            init = true;
        }

        private void btnSaveWallpaper_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fileSave = new System.Windows.Forms.SaveFileDialog();
            fileSave.Filter = "图片文件|*.png";
            fileSave.FileName = "WallPaper" + DateTime.Now.ToString("yyyyMMdd") + ".png";
            if (fileSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string currentFile = fileSave.FileName;
                if (string.IsNullOrEmpty(currentFile))
                {
                    System.Windows.Forms.MessageBox.Show("未选择保存路径！", "保存失败");
                    return;
                }
                int defaultWidth = Convert.ToInt32(txtWidth.Text), defaultHeight = Convert.ToInt32(txtHeight.Text);
                var textRange = new TextRange(rtbPad.Document.ContentStart, rtbPad.Document.ContentEnd);
                string content = textRange.Text;
                Bitmap bmp = new Bitmap(defaultWidth, defaultHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);

                SolidBrush sbrush = new SolidBrush(BackgroundColor);
                SolidBrush fontbrush = new SolidBrush(FontColor);
                g.FillRectangle(sbrush, 0, 0, defaultWidth, defaultHeight);
                SizeF vSizeF = g.MeasureString(content, FontDefault);
                int dStrLength = Convert.ToInt32(Math.Ceiling(vSizeF.Width));
                if (vSizeF.Height > defaultHeight)
                {
                    defaultWidth = Convert.ToInt32(defaultWidth / (defaultHeight * 1.0) * vSizeF.Height);
                    defaultHeight = Convert.ToInt32(vSizeF.Height);
                    bmp = new Bitmap(defaultWidth, defaultHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    g = Graphics.FromImage(bmp);
                    g.FillRectangle(sbrush, 0, 0, defaultWidth, defaultHeight);
                }
                g.DrawString(content, FontDefault, fontbrush, new PointF(defaultWidth - dStrLength - 20, 20));
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                using (FileStream file = new FileStream(currentFile, FileMode.Create))
                {
                    bmp.Save(file, System.Drawing.Imaging.ImageFormat.Png);
                }
                g.Dispose();
                bmp.Dispose();

            }

        }

        private void btnBGColorPicker_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BackgroundColor = System.Drawing.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                btnBGColorPicker.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(BackgroundColor.A, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
                SetRTB();
            }
        }

        private void btnFontColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FontColor = System.Drawing.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                btnFontColor.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(FontColor.A, FontColor.R, FontColor.G, FontColor.B));
                SetRTB();
            }
        }

        private void SetRTB()
        {
            this.rtbPad.Background = btnBGColorPicker.Background;
            this.rtbPad.Foreground = btnFontColor.Foreground;
            this.rtbPad.FontFamily = new System.Windows.Media.FontFamily(FontDefault.FontFamily.Name);
            this.rtbPad.FontSize = FontDefault.Size;
        }

        private void btnFont_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FontDefault = fontDialog.Font;//将当前选定的文字改变字体 
                SetRTB();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.rtbPad.Height = e.NewSize.Height - btnBGColorPicker.Height - 50;
        }

        private void txtWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            int width = 0;
            if (chkFixRatio.IsChecked == true && Int32.TryParse(txtWidth.Text, out width))
            {
                txtHeight.Text = Convert.ToInt32(Screen.PrimaryScreen.Bounds.Height * width * 1.0 / Screen.PrimaryScreen.Bounds.Width).ToString();
            }
        }

        private void txtHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            int height = 0;
            if (chkFixRatio.IsChecked == true && Int32.TryParse(txtHeight.Text, out height))
            {
                txtWidth.Text = Convert.ToInt32(Screen.PrimaryScreen.Bounds.Width * height * 1.0 / Screen.PrimaryScreen.Bounds.Height).ToString();
            }
        }
    }
}
