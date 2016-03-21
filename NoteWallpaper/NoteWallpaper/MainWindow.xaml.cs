using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Markup;
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
            BackgroundColor = System.Drawing.Color.FromArgb(64, 64, 64);
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
            bmp = new Bitmap(Convert.ToInt32(txtWidth.Text), Convert.ToInt32(txtHeight.Text), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(bmp);
            init = true;
        }

        private static Bitmap bmp;
        private static Graphics g;
        private void btnSaveWallpaper_Click(object sender, RoutedEventArgs e)
        {
            string formattedEmail = XamlWriter.Save(rtbPad.Document);
            //SaveXML();
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
                if(string.IsNullOrWhiteSpace(content))
                {
                    content = "";
                }
                bmp = new Bitmap(defaultWidth, defaultHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                g = Graphics.FromImage(bmp);

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

                float imageHeight = vSizeF.Height;
                foreach(var image in images)
                {
                    int imageFitHeight = image.Height, imageFitWidth = image.Width;
                    if (imageFitHeight > defaultHeight)
                    {
                        imageFitWidth = Convert.ToInt32(imageFitWidth * 1.0 * defaultHeight / imageFitHeight);
                        imageFitHeight = defaultHeight;                        
                    }

                    if (imageFitWidth > defaultWidth)
                    {
                        imageFitHeight = Convert.ToInt32(imageFitHeight * 1.0 * defaultWidth / imageFitWidth);
                        imageFitWidth = defaultWidth;
                    }

                    g.DrawImage(image, defaultWidth - imageFitWidth, imageHeight, imageFitWidth, imageFitHeight);
                    imageHeight += imageFitHeight;
                }
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



        private void SaveXML()
        {
            TextRange documentTextRange = new TextRange(rtbPad.Document.ContentStart, rtbPad.Document.ContentEnd);
            FlowDocument flowDocument = rtbPad.Document;

            string s = GetImagesXML(flowDocument);//temp
            LoadImagesIntoXML(s);

            using (StringWriter stringwriter = new StringWriter())
            {
                using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(stringwriter))
                {
                    XamlWriter.Save(flowDocument, writer);//Throws error here
                }

            }
        }

        private string GetImagesXML(FlowDocument flowDocument)
        {
            string s = "";

            using (StringWriter stringwriter = new StringWriter())
            {


                Type inlineType;
                InlineUIContainer uic;
                System.Windows.Controls.Image replacementImage;
                byte[] bytes;
                BitmapImage bi;

                //loop through replacing images in the flowdoc with the byte versions
                foreach (Block b in flowDocument.Blocks)
                {
                    Paragraph paragraph = b as Paragraph;
                    if (paragraph == null) continue;
                    foreach (Inline i in ((Paragraph)b).Inlines)
                    {
                        inlineType = i.GetType();

                        if (inlineType == typeof(Run))
                        {
                            //The inline is TEXT!!!
                        }
                        else if (inlineType == typeof(InlineUIContainer))
                        {
                            //The inline has an object, likely an IMAGE!!!
                            uic = ((InlineUIContainer)i);

                            //if it is an image
                            if (uic.Child.GetType() == typeof(System.Windows.Controls.Image))
                            {
                                //grab the image
                                replacementImage = (System.Windows.Controls.Image)uic.Child;
                                bi = (BitmapImage)replacementImage.Source;

                                //get its byte array
                                bytes = GetImageByteArray(bi);

                                s = Convert.ToBase64String(bytes);//temp
                            }
                        }
                    }
                }

                return s;
            }
        }

        private byte[] GetImageByteArray(BitmapImage src)
        {
            MemoryStream stream = new MemoryStream();
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)src));
            encoder.Save(stream);
            stream.Flush();
            return stream.ToArray();
        }

        private static List<System.Drawing.Image> images = new List<System.Drawing.Image>();
        private void LoadImagesIntoXML(string xml)
        {


            byte[] imageArr = Convert.FromBase64String(xml);
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            MemoryStream stream = new MemoryStream(imageArr);
            BmpBitmapDecoder decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            img.Source = decoder.Frames[0];
            img.Stretch = Stretch.None;

            Paragraph p = new Paragraph();
            p.Inlines.Add(img);
            rtbPad.Document.Blocks.Add(p);
        }

        private void rtbPad_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (System.Windows.Forms.Clipboard.ContainsImage())
                {
                    System.Drawing.Image source = System.Windows.Forms.Clipboard.GetImage();
                    images.Add(source);
                }
                if(System.Windows.Forms.Clipboard.ContainsFileDropList())
                {
                    StringCollection strcollect = System.Windows.Forms.Clipboard.GetFileDropList();
                    if(strcollect[0].ToLower().EndsWith(".png") || 
                        strcollect[0].ToLower().EndsWith(".jpg") ||
                        strcollect[0].ToLower().EndsWith(".jpeg") ||
                        strcollect[0].ToLower().EndsWith(".bmp"))
                    {
                        System.Windows.Forms.DataFormats.Format df = System.Windows.Forms.DataFormats.GetFormat(System.Windows.Forms.DataFormats.Bitmap);
                        System.Drawing.Image image = System.Drawing.Image.FromFile(strcollect[0]);
                        System.Windows.Forms.Clipboard.SetImage(image);
                        images.Add(image);
                    }

                }
                rtbPad.Paste();
                e.Handled = true;
            }
        }
    }
}
