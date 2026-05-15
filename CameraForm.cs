using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ExFood
{
    public class CameraForm : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private PictureBox picCamera;
        private Button btnCapture;
        private Button btnClose;
        private Label lblStatus;
        private Bitmap capturedFrame;

        public string CapturedImagePath { get; private set; } = "";

        public CameraForm()
        {
            InitializeForm();
            BuildUI();
            StartCamera();
        }

        private void InitializeForm()
        {
            this.Text = "카메라 촬영";
            this.Size = new Size(520, 480);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.FormClosing += CameraForm_FormClosing;
        }

        private void BuildUI()
        {
            // ── 헤더 ──
            Panel headerPanel = new Panel
            {
                Size = new Size(520, 50),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(45, 106, 79)
            };

            Label lblTitle = new Label
            {
                Text = "📷 카메라 촬영",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 13),
                AutoSize = true
            };

            btnClose = new Button
            {
                Text = "✕",
                Size = new Size(35, 35),
                Location = new Point(472, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(btnClose);
            this.Controls.Add(headerPanel);

            // ── 카메라 미리보기 ──
            picCamera = new PictureBox
            {
                Size = new Size(480, 320),
                Location = new Point(20, 60),
                BackColor = Color.Black,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(picCamera);

            // ── 상태 텍스트 ──
            lblStatus = new Label
            {
                Text = "카메라 연결 중...",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(20, 388),
                AutoSize = true
            };
            this.Controls.Add(lblStatus);

            // ── 촬영 버튼 ──
            btnCapture = new Button
            {
                Text = "📸 촬영",
                Size = new Size(150, 45),
                Location = new Point(185, 420),
                BackColor = Color.FromArgb(45, 106, 79),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnCapture.FlatAppearance.BorderSize = 0;
            btnCapture.Click += BtnCapture_Click;
            this.Controls.Add(btnCapture);
        }

        // ── 카메라 시작 ──
        private void StartCamera()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    lblStatus.Text = "❌ 카메라를 찾을 수 없습니다.";
                    return;
                }

                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();

                lblStatus.Text = "✅ 연결됨: " + videoDevices[0].Name;
                btnCapture.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ 카메라 오류: " + ex.Message;
            }
        }

        // ── 실시간 프레임 수신 ──
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (picCamera.InvokeRequired)
            {
                picCamera.Invoke(new Action(() =>
                {
                    capturedFrame = (Bitmap)eventArgs.Frame.Clone();
                    picCamera.Image = (Bitmap)capturedFrame.Clone();
                }));
            }
        }

        // ── 촬영 버튼 클릭 ──
        private void BtnCapture_Click(object sender, EventArgs e)
        {
            if (capturedFrame == null)
            {
                MessageBox.Show("카메라 준비 중입니다. 잠시 후 다시 시도해주세요.");
                return;
            }

            try
            {
                string tempPath = Path.Combine(
                    Path.GetTempPath(),
                    "kitchenfresh_" + DateTime.Now.Ticks + ".jpg"
                );

                capturedFrame.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                CapturedImagePath = tempPath;

                StopCamera();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("촬영 오류: " + ex.Message);
            }
        }

        // ── 카메라 정지 ──
        private void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.NewFrame -= VideoSource_NewFrame;
            }
        }

        // ── 폼 닫힐 때 카메라 정지 ──
        private void CameraForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCamera();
        }
    }
}