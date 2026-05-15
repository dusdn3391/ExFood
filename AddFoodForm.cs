using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using ExFood.Models;

namespace ExFood
{
    public class AddFoodForm : Form
    {
        private Panel headerPanel;
        private Label lblTitle;
        private Button btnClose;
        private PictureBox picPreview;
        private Button btnUpload;
        private Button btnCamera;
        private Label lblNameTitle;
        private TextBox txtName;
        private Label lblDateTitle;
        private TextBox txtDate;
        private Button btnSave;
        private string imagePath = "";

        // 환경변수에서 API 키 읽기
        private string API_KEY = AppConfig.GoogleVisionKey;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000; // 그림자
                return cp;
            }
        }

        public AddFoodForm()
        {
            InitializeForm();
            BuildUI();
        }

        private void InitializeForm()
        {
            this.Text = "식재료 추가";
            this.Size = new Size(380, 580);
            this.BackColor = Color.FromArgb(245, 243, 235);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;

            // 테두리
            this.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(45, 106, 79), 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, this.Width - 3, this.Height - 3);
                }
            };
        }

        private void BuildUI()
        {
            // ── 헤더 ──
            headerPanel = new Panel
            {
                Size = new Size(380, 55),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(45, 106, 79)
            };

            lblTitle = new Label
            {
                Text = "🌿 식재료 추가",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            btnClose = new Button
            {
                Text = "✕",
                Size = new Size(35, 35),
                Location = new Point(330, 10),
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

            // ── 이미지 미리보기 ──
            picPreview = new PictureBox
            {
                Size = new Size(330, 180),
                Location = new Point(25, 70),
                BackColor = Color.FromArgb(220, 220, 210),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };

            Label lblPreviewHint = new Label
            {
                Text = "📷 사진을 업로드하거나\n카메라로 촬영하세요",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(150, 150, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            picPreview.Controls.Add(lblPreviewHint);
            this.Controls.Add(picPreview);

            // ── 파일 업로드 버튼 ──
            btnUpload = new Button
            {
                Text = "🖼️ 파일 업로드",
                Size = new Size(155, 40),
                Location = new Point(25, 265),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(45, 106, 79),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnUpload.FlatAppearance.BorderColor = Color.FromArgb(45, 106, 79);
            btnUpload.Click += BtnUpload_Click;
            this.Controls.Add(btnUpload);

            // ── 카메라 버튼 ──
            btnCamera = new Button
            {
                Text = "📷 카메라 촬영",
                Size = new Size(155, 40),
                Location = new Point(200, 265),
                BackColor = Color.FromArgb(45, 106, 79),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCamera.FlatAppearance.BorderSize = 0;
            btnCamera.Click += BtnCamera_Click;
            this.Controls.Add(btnCamera);

            // ── 식재료 이름 ──
            lblNameTitle = new Label
            {
                Text = "식재료 이름",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 106, 79),
                Location = new Point(25, 320),
                AutoSize = true
            };
            this.Controls.Add(lblNameTitle);

            txtName = new TextBox
            {
                Size = new Size(330, 35),
                Location = new Point(25, 345),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Text = "OCR 인식 후 자동 입력",
                ForeColor = Color.FromArgb(180, 180, 180)
            };
            txtName.GotFocus += (s, e) =>
            {
                if (txtName.Text == "OCR 인식 후 자동 입력")
                {
                    txtName.Text = "";
                    txtName.ForeColor = Color.Black;
                }
            };
            txtName.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    txtName.Text = "OCR 인식 후 자동 입력";
                    txtName.ForeColor = Color.FromArgb(180, 180, 180);
                }
            };
            this.Controls.Add(txtName);

            // ── 유통기한 ──
            lblDateTitle = new Label
            {
                Text = "유통기한",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 106, 79),
                Location = new Point(25, 390),
                AutoSize = true
            };
            this.Controls.Add(lblDateTitle);

            txtDate = new TextBox
            {
                Size = new Size(330, 35),
                Location = new Point(25, 415),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Text = "예: 2025-12-31",
                ForeColor = Color.FromArgb(180, 180, 180)
            };
            txtDate.GotFocus += (s, e) =>
            {
                if (txtDate.Text == "예: 2025-12-31")
                {
                    txtDate.Text = "";
                    txtDate.ForeColor = Color.Black;
                }
            };
            txtDate.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtDate.Text))
                {
                    txtDate.Text = "예: 2025-12-31";
                    txtDate.ForeColor = Color.FromArgb(180, 180, 180);
                }
            };
            this.Controls.Add(txtDate);

            // ── 저장 버튼 ──
            btnSave = new Button
            {
                Text = "✅ 저장",
                Size = new Size(330, 50),
                Location = new Point(25, 470),
                BackColor = Color.FromArgb(45, 106, 79),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        // ── 파일 업로드 클릭 ──
        private void BtnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp";
                dlg.Title = "이미지 선택";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dlg.FileName;
                    picPreview.Image = Image.FromFile(imagePath);
                    picPreview.Controls.Clear();
                    RunOCR(imagePath);  // ✅ Google Vision 호출
                }
            }
        }

        // ── 카메라 촬영 클릭 ──
        private void BtnCamera_Click(object sender, EventArgs e)
        {
            CameraForm cameraForm = new CameraForm();
            if (cameraForm.ShowDialog() == DialogResult.OK)
            {
                imagePath = cameraForm.CapturedImagePath;
                picPreview.Image = Image.FromFile(imagePath);
                picPreview.Controls.Clear();
                RunOCR(imagePath);  // ✅ Google Vision 호출
            }
        }

        // ── Google Vision OCR 실행 ──
        private void RunOCR(string path)
        {
            // API 키 확인
            if (string.IsNullOrEmpty(API_KEY))
            {
                MessageBox.Show(
                    "API 키가 없습니다!\n\n" +
                    "환경변수 GOOGLE_VISION_KEY 를\n" +
                    "설정해주세요.",
                    "API 키 오류"
                );
                return;
            }

            try
            {
                // 1. 이미지 → Base64 변환
                byte[] imageBytes = File.ReadAllBytes(path);
                string base64Image = Convert.ToBase64String(imageBytes);

                // 2. 요청 만들기
                var requestBody = new
                {
                    requests = new[]
                    {
                        new
                        {
                            image = new { content = base64Image },
                            features = new[]
                            {
                                new { type = "TEXT_DETECTION" }
                            }
                        }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);

                // 3. API 호출
                using (var client = new HttpClient())
                {
                    string url = "https://vision.googleapis.com/v1/images:annotate?key=" + API_KEY;
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).Result;
                    string responseJson = response.Content.ReadAsStringAsync().Result;

                    // 4. 결과 파싱
                    dynamic result = JsonConvert.DeserializeObject(responseJson);

                    string text = "";
                    try
                    {
                        text = result.responses[0].fullTextAnnotation.text;
                    }
                    catch
                    {
                        MessageBox.Show(
                            "텍스트를 인식하지 못했어요.\n더 선명한 사진으로 시도해주세요.",
                            "인식 실패"
                        );
                        return;
                    }

                    // 5. 텍스트에서 이름 + 날짜 추출
                    ParseOCRResult(text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("OCR 오류: " + ex.Message, "오류");
            }
        }

        // ── OCR 결과 파싱 ──
        private void ParseOCRResult(string text)
        {
            // 날짜 패턴 찾기
            // 예: 2025.12.31 / 2025-12-31 / 25.12.31 / 2025/12/31
            string datePattern = @"(\d{4}|\d{2})[-./]\d{2}[-./]\d{2}";
            Match match = Regex.Match(text, datePattern);

            if (match.Success)
            {
                string date = match.Value
                    .Replace(".", "-")
                    .Replace("/", "-");

                // 2자리 연도 → 4자리로 변환
                if (date.Length == 8)
                    date = "20" + date;

                txtDate.Text = date;
                txtDate.ForeColor = Color.Black;
            }
            else
            {
                MessageBox.Show(
                    "유통기한을 찾지 못했어요.\n직접 입력해주세요.",
                    "날짜 인식 실패"
                );
            }

            // 날짜 제외 첫 줄을 식재료 이름으로
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Length > 1 && !trimmed.Contains(match.Value))
                {
                    txtName.Text = trimmed;
                    txtName.ForeColor = Color.Black;
                    break;
                }
            }
        }

        // MySQL 저장 부분도 수정
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ appsettings.json 에서 읽기
                using (var conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO ingredients (name, expiry_date) VALUES (@name, @date)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@date", txtDate.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ 저장 완료!", "성공");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("저장 오류: " + ex.Message);
            }
        }
    }
}