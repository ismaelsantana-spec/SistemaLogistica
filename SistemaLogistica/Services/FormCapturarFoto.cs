using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaLogistica.Services
{
    public class FormCapturarFoto : Form
    {
        private VideoCaptureDevice? videoSource;
        private FilterInfoCollection? videoDevices;
        private PictureBox pictureBox;
        private Label lblInstruccion;
        private Button btnCapturar;
        private Button btnCancelar;
        private Bitmap? fotoCapturada;

        public Bitmap? FotoCapturada { get; private set; }
        public bool CapturaExitosa { get; private set; }

        public FormCapturarFoto()
        {
            InitializeComponent();
            InicializarCamara();
            CapturaExitosa = false;
        }

        private void InitializeComponent()
        {
            this.Text = "Capturar Evidencia Fotográfica";
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            pictureBox = new PictureBox();
            pictureBox.Size = new Size(640, 480);
            pictureBox.Location = new Point(80, 20);
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.BackColor = Color.Black;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(pictureBox);

            lblInstruccion = new Label();
            lblInstruccion.Text = "📸 Posicione la cámara y haga clic en CAPTURAR";
            lblInstruccion.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblInstruccion.ForeColor = Color.White;
            lblInstruccion.Location = new Point(80, 520);
            lblInstruccion.Size = new Size(640, 30);
            lblInstruccion.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblInstruccion);

            btnCapturar = new Button();
            btnCapturar.Text = "📷 CAPTURAR FOTO";
            btnCapturar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnCapturar.Size = new Size(200, 50);
            btnCapturar.Location = new Point(200, 570);
            btnCapturar.BackColor = Color.FromArgb(40, 167, 69);
            btnCapturar.ForeColor = Color.White;
            btnCapturar.FlatStyle = FlatStyle.Flat;
            btnCapturar.Cursor = Cursors.Hand;
            btnCapturar.FlatAppearance.BorderSize = 0;
            btnCapturar.Click += BtnCapturar_Click;
            this.Controls.Add(btnCapturar);

            btnCancelar = new Button();
            btnCancelar.Text = "Cancelar";
            btnCancelar.Font = new Font("Segoe UI", 12);
            btnCancelar.Size = new Size(150, 50);
            btnCancelar.Location = new Point(420, 570);
            btnCancelar.BackColor = Color.FromArgb(220, 53, 69);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;
            this.Controls.Add(btnCancelar);

            this.FormClosing += FormCapturarFoto_FormClosing;
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
                    CapturaExitosa = false;
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();

                lblInstruccion.Text = "📸 Cámara lista - Presione CAPTURAR FOTO";
                lblInstruccion.ForeColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar cámara:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CapturaExitosa = false;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (fotoCapturada != null)
                {
                    fotoCapturada.Dispose();
                }

                fotoCapturada = (Bitmap)eventArgs.Frame.Clone();

                if (pictureBox.InvokeRequired)
                {
                    pictureBox.Invoke(new Action(() =>
                    {
                        if (pictureBox.Image != null)
                        {
                            var oldImage = pictureBox.Image;
                            pictureBox.Image = (Bitmap)fotoCapturada.Clone();
                            oldImage.Dispose();
                        }
                        else
                        {
                            pictureBox.Image = (Bitmap)fotoCapturada.Clone();
                        }
                    }));
                }
                else
                {
                    if (pictureBox.Image != null)
                    {
                        var oldImage = pictureBox.Image;
                        pictureBox.Image = (Bitmap)fotoCapturada.Clone();
                        oldImage.Dispose();
                    }
                    else
                    {
                        pictureBox.Image = (Bitmap)fotoCapturada.Clone();
                    }
                }
            }
            catch { }
        }

        private void BtnCapturar_Click(object? sender, EventArgs e)
        {
            if (fotoCapturada != null)
            {
                FotoCapturada = (Bitmap)fotoCapturada.Clone();
                CapturaExitosa = true;

                lblInstruccion.Text = "✅ FOTO CAPTURADA EXITOSAMENTE";
                lblInstruccion.ForeColor = Color.LightGreen;

                System.Media.SystemSounds.Beep.Play();

                var timer = new System.Windows.Forms.Timer();
                timer.Interval = 1000;
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    this.Close();
                };
                timer.Start();
            }
            else
            {
                MessageBox.Show("No se pudo capturar la imagen. Intente nuevamente.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            CapturaExitosa = false;
            FotoCapturada = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormCapturarFoto_FormClosing(object? sender, FormClosingEventArgs e)
        {
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
                if (fotoCapturada != null)
                {
                    fotoCapturada.Dispose();
                }
                if (FotoCapturada != null)
                {
                    FotoCapturada.Dispose();
                }
                if (pictureBox != null && pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                }
                pictureBox?.Dispose();
                lblInstruccion?.Dispose();
                btnCapturar?.Dispose();
                btnCancelar?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}