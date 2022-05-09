using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using offis = Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.ToolTips;
using GMap.NET.WindowsForms;
using VisioForge.Types.OutputFormat;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using CefSharp;
using CefSharp.WinForms;
using VisioForge.Types;

namespace WindowsFormsApp54
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            videoCapture1.OnError += OnError;
        }

        Thread thr;
        float x = 0, y = 0, z = 0;
        string hedef_veriler;
        String servo_pwm;
        int b = 0;
        string data;
        string[] veriler;
        int i, j;
        
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portlar = SerialPort.GetPortNames();
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);

            textBox7.Text = "192.168.137.194/upload"; 
            chrome = new ChromiumWebBrowser(textBox7.Text);
            this.panel2.Controls.Add(chrome);
            chrome.Dock = DockStyle.Fill;
            chrome.AddressChanged += chrome_AddressChanged;


            GL.ClearColor(Color.Black);

            double ilk_enlem = 37.037237;
            double ilk_boylam = 37.311915;
            map.DragButton = MouseButtons.Left;
            map.MapProvider = GMapProviders.GoogleSatelliteMap;
            map.MinZoom = 5;
            map.MaxZoom = 100;
            map.Zoom = 15;
            map.Position = new PointLatLng(ilk_enlem, ilk_boylam);
            map.DragButton = MouseButtons.Left;


            Control.CheckForIllegalCrossThreadCalls = false;
            dataGridView1.ColumnCount = 17;
            dataGridView1.RowCount = 10000;
            dataGridView1.Columns[0].Name = "TAKIM NO";
            dataGridView1.Columns[1].Name = "PAKET NO";
            dataGridView1.Columns[2].Name = "GÖNDERME ZAMANI";
            dataGridView1.Columns[3].Name = "BASINÇ";
            dataGridView1.Columns[4].Name = "YÜKSEKLİK";
            dataGridView1.Columns[5].Name = "İNİŞ HIZI";
            dataGridView1.Columns[6].Name = "SICAKLIK";
            dataGridView1.Columns[7].Name = "PİL GERİLİMİ";
            dataGridView1.Columns[8].Name = "GPS LATITUDE";
            dataGridView1.Columns[9].Name = "GPS LONGITUDE";
            dataGridView1.Columns[10].Name = "GPS ALTITUDE";
            dataGridView1.Columns[11].Name = "UYDU STATÜSÜ";
            dataGridView1.Columns[12].Name = "PITCH";
            dataGridView1.Columns[13].Name = "ROLL";
            dataGridView1.Columns[14].Name = "YAW";
            dataGridView1.Columns[15].Name = "DONÜŞ SAYISI";
            dataGridView1.Columns[16].Name = "Video Aktarım Bilgisi";


            foreach (string port in portlar)
            {
                comboBox1.Items.Add(port);
                comboBox1.SelectedIndex = 0;


            }

            comboBox2.Items.Add("300");
            comboBox2.Items.Add("600");
            comboBox2.Items.Add("1200");
            comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("115200");
            comboBox2.SelectedIndex = 7;

            foreach (var device in videoCapture1.Video_CaptureDevicesInfo)
            {
                // comboBox3.Items.Add(device.Name);
            }
        }

        private void chrome_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                textBox7.Text = e.Address;

            }));
        }

        ChromiumWebBrowser chrome;
        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            chrome.Load(textBox7.Text);
        }
        private void copyAlltoClipboard()
        {
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void gunaCircleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                copyAlltoClipboard();
                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlexcel = new Excel.Application();
                xlexcel.Visible = true;
                xlWorkBook = xlexcel.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                Excel.Range CR = (Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            }
            catch (Exception hata)
            {
                MessageBox.Show(hata.StackTrace);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (textBox7.Text == "http://192.168.137.194/fupload")
                {
                    serialPort1.Write("E");
                }


              
                if (veriler[0] == "55324")
                    {

                    map.MapProvider = GMapProviders.GoogleSatelliteMap;     //GPS
                    map.Overlays.Clear();
                    string lat = veriler[13];
                    double lat_d = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture); //Convert.ToDouble(lat); // * 1000000;
                    String longt = veriler[14];
                    double longt_d = double.Parse(longt, System.Globalization.CultureInfo.InvariantCulture); // Convert.ToDouble(longt); //* 1000000;

                    GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat_d, longt_d), GMarkerGoogleType.red);

                    map.Position = new PointLatLng(lat_d, longt_d);
                    data = "";

                    x = Convert.ToInt16(veriler[18]);
                    y = Convert.ToInt16(veriler[19]);
                    z = Convert.ToInt16(veriler[17]);
                    glControl1.Invalidate();

                    string a = veriler[1];
                    int a_1 = int.Parse(a, System.Globalization.CultureInfo.InvariantCulture);


                    if (b < a_1)

                    {
                        dataGridView1.Rows[i].Cells[j].Value = veriler[0]; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[1]; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[2] + "/" + veriler[3] + "/" + veriler[4] + "," + veriler[5] + ":" + veriler[6] + ":" + veriler[7]; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[8] + " hPa"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[9] + " m"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[10] + " m/s"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[11] + " Cᵒ"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[12] + " V"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[13] + " ᵒ"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[14] + " ᵒ"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[15] + " m"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[16]; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[17] + " ᵒ"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[18] + " ᵒ"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[19] + " ᵒ"; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[20]; j++;
                        dataGridView1.Rows[i].Cells[j].Value = veriler[21]; j++;

                        i++;
                        j = 0;

                        b = a_1;
                    }

                    try
                    {


                        this.chart1.Series["BASINÇ"].Points.AddXY(DateTime.Now.ToLongTimeString(), veriler[8]);
                        this.chart2.Series["SICAKLIK"].Points.AddXY(DateTime.Now.ToLongTimeString(), veriler[11]);
                        this.chart3.Series["İNİŞ HIZI"].Points.AddXY(DateTime.Now.ToLongTimeString(), veriler[10]);
                        this.chart5.Series["PİL GERİLİMİ"].Points.AddXY(DateTime.Now.ToLongTimeString(), veriler[12]);
                        this.chart4.Series["YÜKSEKLİK"].Points.AddXY(DateTime.Now.ToLongTimeString(), veriler[9]);



                    }


                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }


                    int l = i - 9;

                    dataGridView1.FirstDisplayedScrollingRowIndex = l;
                }
            }
            catch (Exception)
            {

            }

        }



        private void job1()
        {
            try
            {            
            if (serialPort1.IsOpen == false)
            {

                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = 115200;

                /*
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = 115200;
                serialPort1.Parity = Parity.Even;
                serialPort1.StopBits = StopBits.One;
                serialPort1.DataBits = 8;*/

                try
                {
                    serialPort1.Open();                   
                }
                catch (Exception)
                {
                    MessageBox.Show("Port Bağlantısı Yapılamadı");                  
                }
            }
            
            for (; ;)
            {
                    //Thread.Sleep(10);
                    try
                    {
                        data = serialPort1.ReadLine();
                        veriler = data.Split('_');
                        int deger = data.Length;
                        textBox1.Text = deger + "";
                        Console.WriteLine(data.Length);
                        Console.ReadLine();


                        Thread.Sleep(300);
                    }
                    catch (Exception)
                    {
                        if (!serialPort1.IsOpen == true)
                        {
                            textBox1.Text = "0";
                            b = 0;
                            // MessageBox.Show("BAĞLANTI KESİLDİ.");
                            timer1.Stop();
                            videoCapture1.Stop();
                            thr.Abort();

                            serialPort1.Close();
                        }
                        
                    }
                   
                       
            }
            }
            catch 
            {
              

            }

        }

        private void gunaCircleButton1_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")

                return;
            thr = new Thread(job1);
            thr.Start();
            timer1.Start();          
        }

        private void gunaCircleButton2_Click_1(object sender, EventArgs e)
        {
            
         if (comboBox1.Text == "")

                  return;

            timer1.Stop();
            videoCapture1.Stop();
            thr.Abort();

            serialPort1.Close();


            MessageBox.Show("BAĞLANTI KESİLDİ");
            
        }

        private void gunaCircleButton3_Click_1(object sender, EventArgs e)
        {
            if (textBox3.Text == "")

                return;

            hedef_veriler += servo_pwm;
            serialPort1.Write("S"+textBox3.Text.ToString());
            hedef_veriler = "";
         // motor_pwm = "";
            servo_pwm = "";
        }
        private void gunaCircleButton5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")

                return;
            serialPort1.Write("M"+textBox2.Text.ToString());
        }

        private void gunaGradientButton1_Click_1(object sender, EventArgs e)
        {
            
            videoCapture1.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = "http://192.168.137.35/", Type = VisioForge.Types.VFIPSource.HTTP_MJPEG_LowLatency };
            videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
            videoCapture1.Output_Filename = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\Yer_İstasyonu_Kayıt.mp4";
            videoCapture1.Output_Format = new VFWMVOutput();
            videoCapture1.Mode = VisioForge.Types.VFVideoCaptureMode.IPCapture;
            
            videoCapture1.Start();

        }

        private void gunaGradientButton2_Click_1(object sender, EventArgs e)
        {
            videoCapture1.Stop();
        }
        private void OnError(object sender, ErrorsEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }


       
//simulation
        private void silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(PrimitiveType.Quads);
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(Color.White);
                else if (step < 90)
                    GL.Color3(Color.FromArgb(253, 225, 0));
                else if (step < 135)
                    GL.Color3(Color.White);
                else if (step < 180)
                    GL.Color3(Color.FromArgb(253, 225, 0));
                else if (step < 225)
                    GL.Color3(Color.White);
                else if (step < 270)
                    GL.Color3(Color.FromArgb(253, 225, 0));
                else if (step < 315)
                    GL.Color3(Color.White);
                else if (step < 360)
                    GL.Color3(Color.FromArgb(253, 225, 0));

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 2) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 2) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)
            {
                if (step < 45)
                    GL.Color3(Color.White);
                else if (step < 90)
                    GL.Color3(Color.FromArgb(253, 225, 0));
                else if (step < 135)
                    GL.Color3(Color.White);
                else if (step < 180)
                    GL.Color3(Color.FromArgb(253, 225, 0));
                else if (step < 225)
                    GL.Color3(Color.White);
                else if (step < 270)
                    GL.Color3(Color.FromArgb(253, 225, 0));
                else if (step < 315)
                    GL.Color3(Color.White);
                else if (step < 360)
                    GL.Color3(Color.FromArgb(253, 225, 0));

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey1, ciz1_y);
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            while (step <= 180)//ALT KAPAK
            {
                if (step < 45)
                    GL.Color3(Color.White);
                else if (step < 90)
                    GL.Color3(Color.White);
                else if (step < 135)
                    GL.Color3(Color.White);
                else if (step < 180)
                    GL.Color3(Color.White);
                else if (step < 225)
                    GL.Color3(Color.White);
                else if (step < 270)
                    GL.Color3(Color.White);
                else if (step < 315)
                    GL.Color3(Color.White);
                else if (step < 360)
                    GL.Color3(Color.White);

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);


                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);





                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                step += topla;
                topla = step;
            }
            GL.End();

        }


        private void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 1.0f;
            GL.Begin(PrimitiveType.Lines);
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(Color.White);
                else if (step < 90)
                    GL.Color3(Color.White);
                else if (step < 135)
                    GL.Color3(Color.White);
                else if (step < 180)
                    GL.Color3(Color.White);
                else if (step < 225)
                    GL.Color3(Color.White);
                else if (step < 270)
                    GL.Color3(Color.White);
                else if (step < 315)
                    GL.Color3(Color.White);
                else if (step < 360)
                    GL.Color3(Color.White);

                float ciz1_x = (float)(radius1 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius1 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            step = eski_step;
            topla = step;
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(Color.White);
                else if (step < 90)
                    GL.Color3(Color.White);
                else if (step < 135)
                    GL.Color3(Color.White);
                else if (step < 180)
                    GL.Color3(Color.White);
                else if (step < 225)
                    GL.Color3(Color.White);
                else if (step < 270)
                    GL.Color3(Color.White);
                else if (step < 315)
                    GL.Color3(Color.White);
                else if (step < 360)
                    GL.Color3(Color.White);


                float ciz1_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            GL.End();
        }


        private void Pervane(float yukseklik, float uzunluk, float kalinlik, float egiklik)
        {
            //float radius = 8, angle = 45.0f;
            //GL.Begin(PrimitiveType.Quads);

            //GL.Color3(Color.White);
            //GL.Vertex3(uzunluk, yukseklik, kalinlik);
            //GL.Vertex3(uzunluk, yukseklik + egiklik, -kalinlik);
            //GL.Vertex3(0.0, yukseklik + egiklik, -kalinlik);
            //GL.Vertex3(0.0, yukseklik, kalinlik);

            //GL.Color3(Color.White);
            //GL.Vertex3(-uzunluk, yukseklik + egiklik, kalinlik);
            //GL.Vertex3(-uzunluk, yukseklik, -kalinlik);
            //GL.Vertex3(0.0, yukseklik, -kalinlik);
            //GL.Vertex3(0.0, yukseklik + egiklik, kalinlik);


            //GL.Color3(Color.FromArgb(253, 225, 0));
            //GL.Vertex3(kalinlik, yukseklik, -uzunluk);
            //GL.Vertex3(-kalinlik, yukseklik + egiklik, -uzunluk);
            //GL.Vertex3(-kalinlik, yukseklik + egiklik, 0.0);//+
            //GL.Vertex3(kalinlik, yukseklik, 0.0);//-

            //GL.Color3(Color.FromArgb(253, 225, 0));
            //GL.Vertex3(kalinlik, yukseklik + egiklik, +uzunluk);
            //GL.Vertex3(-kalinlik, yukseklik, +uzunluk);
            //GL.Vertex3(-kalinlik, yukseklik, 0.0);
            //GL.Vertex3(kalinlik, yukseklik + egiklik, 0.0);


            //GL.End();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            float step = 1.0f;
            float topla = step;
            float radius = 3.0f;
            float dikey1 = radius, dikey2 = -radius;

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 4 / 3, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(35, 0, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Rotate(x, 0.2, 0.0, 0.0);//ÖNEMLİ
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);

            silindir(step, topla, radius, 9, -8);
            silindir(0.01f, topla, 3f, 9, 9f);
            silindir(0.01f, topla, 3f, 3, dikey1 + 3);
            koni(0.1f, 0.01f, radius, 3f, 3, 3);
            koni(0.01f, 0.01f, radius, 1f, 9f, 12f);
            Pervane(10.0f, 10.0f, 0.2f, 0.5f);
            //Beginmode
            GL.Begin(PrimitiveType.Lines);

            GL.Color3(Color.White);
            GL.Vertex3(-30.0, 0.0, 0.0);
            GL.Vertex3(30.0, 0.0, 0.0);


            GL.Color3(Color.Transparent);
            GL.Vertex3(0.0, 30.0, 0.0);
            GL.Vertex3(0.0, -30.0, 0.0);

            GL.Color3(Color.Yellow);
            GL.Vertex3(0.0, 0.0, 30.0);
            GL.Vertex3(0.0, 0.0, -30.0);

            GL.End();
            //GraphicsContext.CurrentContext.VSync = true;
            glControl1.SwapBuffers();
        }
        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);//sonradan yazdık

        }
        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }

        private void numericUpDown2_ValueChanged_1(object sender, EventArgs e)
        {
            glControl1.Invalidate();

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }


        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        private void glControl1_Resize(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void gunaCirclePictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void map_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
