using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Windows.Compatibility;

namespace SistemaLogistica.Services
{
    public class FormEscanerQR : Form
    {
        private VideoCaptureDevice? videoSource;
        private FilterInfoCollection? videoDevices;
        private PictureBox pictureBox;
        private Label lblEstado;
        private Button btnCancelar;
        private System.Windows.Forms.Timer timerEscaneo;
        private BarcodeReader reader;

        public string? CodigoQRDetectado { get; private set; }
        public bool EscaneoExitoso { get; private set; }

        public FormEscanerQR()
        {
            InitializeComponent();
            InicializarCamara();

            // Crear el lector de códigos QR
            reader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new[] { BarcodeFormat.QR_CODE }
                }
            };

            EscaneoExitoso = false;
        }

        private void InitializeComponent()
        {
            this.Text = "Escanear Código QR - Acerque el QR a la cámara";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            // PictureBox para mostrar video
            pictureBox = new PictureBox
            {
                Size = new Size(640, 480),
                Location = new Point(80, 20),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBox);

            // Label de estado
            lblEstado = new Label
            {
                Text = "📸 Buscando código QR...",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 520),
                Size = new Size(640, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblEstado);

            // Botón cancelar
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 12),
                Size = new Size(150, 40),
                Location = new Point(325, 560),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;
            this.Controls.Add(btnCancelar);

            // Timer para escaneo continuo
            timerEscaneo = new System.Windows.Forms.Timer
            {
                Interval = 300 // Escanear cada 300ms
            };
            timerEscaneo.Tick += TimerEscaneo_Tick;
            timerEscaneo.Start();

            this.FormClosing += FormEscanerQR_FormClosing;
        }

        private void InicializarCamara()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("No se detectó ninguna cámara web.\n\nPor favor conecte una cámara e intente nuevamente.",
                        "Error - Sin cámara", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    EscaneoExitoso = false;
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                // Usar la primera cámara disponible
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();

                lblEstado.Text = "📸 Cámara iniciada - Acerque el QR";
                lblEstado.ForeColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar cámara:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                EscaneoExitoso = false;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

                if (pictureBox.InvokeRequired)
                {
                    pictureBox.Invoke(new Action(() =>
                    {
                        if (pictureBox.Image != null)
                        {
                            var oldImage = pictureBox.Image;
                            pictureBox.Image = frame;
                            oldImage.Dispose();
                        }
                        else
                        {
                            pictureBox.Image = frame;
                        }
                    }));
                }
                else
                {
                    if (pictureBox.Image != null)
                    {
                        var oldImage = pictureBox.Image;
                        pictureBox.Image = frame;
                        oldImage.Dispose();
                    }
                    else
                    {
                        pictureBox.Image = frame;
                    }
                }
            }
            catch { }
        }

        private void TimerEscaneo_Tick(object? sender, EventArgs e)
        {
            try
            {
                if (pictureBox.Image != null)
                {
                    Bitmap bitmap = (Bitmap)pictureBox.Image.Clone();

                    // Decodificar usando el método correcto
                    var result = reader.Decode(bitmap);

                    bitmap.Dispose();

                    if (result != null && !string.IsNullOrEmpty(result.Text))
                    {
                        CodigoQRDetectado = result.Text;
                        EscaneoExitoso = true;

                        lblEstado.Text = $"✅ QR DETECTADO: {CodigoQRDetectado}";
                        lblEstado.ForeColor = Color.LightGreen;

                        // Reproducir sonido de éxito
                        System.Media.SystemSounds.Beep.Play();

                        // Detener escaneo
                        timerEscaneo.Stop();

                        // Esperar un momento para que el usuario vea el mensaje
                        Task.Delay(1500).ContinueWith(_ =>
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() => this.Close()));
                            }
                            else
                            {
                                this.Close();
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            EscaneoExitoso = false;
            CodigoQRDetectado = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormEscanerQR_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (timerEscaneo != null)
            {
                timerEscaneo.Stop();
                timerEscaneo.Dispose();
            }

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource = null;
            }

            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timerEscaneo?.Dispose();
                pictureBox?.Image?.Dispose();
                pictureBox?.Dispose();
                lblEstado?.Dispose();
                btnCancelar?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}