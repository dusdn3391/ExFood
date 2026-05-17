using System;
using System.Drawing;
using System.Windows.Forms;
using ExFood.Models;
using ExFood.Services;

namespace ExFood.Forms
{
    public class AddFoodForm : Form
    {
        private string API_KEY;  // ✅ 선언만
        private Panel headerPanel;
        private Label lblTitle;
        private Button btnClose;
        private PictureBox picPreview;
        private Button btnUpload;
        private Button btnCamera;
        private Label lblCategoryTitle;
        private ComboBox cmbCategory;
        private Label lblNameTitle;
        private TextBox txtName;
        private Label lblDateTitle;
        private TextBox txtDate;
        private Label lblStorageTitle;
        private TextBox txtStorage;
        private Button btnForeign;
        private Button btnSave;
        private string imagePath = "";
        private bool isForeignMode = false;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000;
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
            this.Size = new Size(380, 730);
            this.BackColor = Color.FromArgb(245, 243, 235);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            API_KEY = AppConfig.GoogleVisionKey; 

            this.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(45, 106, 79), 2))
                    e.Graphics.DrawRectangle(pen, 1, 1, this.Width - 3, this.Height - 3);
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

            // ── 외국 식품 버튼 ──
            btnForeign = new Button
            {
                Text = "🌍 외국 식품 모드 OFF",
                Size = new Size(330, 35),
                Location = new Point(25, 315),
                BackColor = Color.FromArgb(220, 220, 210),
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnForeign.FlatAppearance.BorderSize = 0;
            btnForeign.Click += BtnForeign_Click;
            this.Controls.Add(btnForeign);

            // ── 카테고리 ──
            lblCategoryTitle = new Label
            {
                Text = "카테고리",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 106, 79),
                Location = new Point(25, 362),
                AutoSize = true
            };
            this.Controls.Add(lblCategoryTitle);

            cmbCategory = new ComboBox
            {
                Size = new Size(330, 35),
                Location = new Point(25, 385),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(CategoryService.GetCategories());
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += CmbCategory_Changed;
            this.Controls.Add(cmbCategory);

            // ── 식재료 이름 ──
            lblNameTitle = new Label
            {
                Text = "식재료 이름",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 106, 79),
                Location = new Point(25, 430),
                AutoSize = true
            };
            this.Controls.Add(lblNameTitle);

            txtName = new TextBox
            {
                Size = new Size(330, 35),
                Location = new Point(25, 453),
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
                Location = new Point(25, 498),
                AutoSize = true
            };
            this.Controls.Add(lblDateTitle);

            txtDate = new TextBox
            {
                Size = new Size(330, 35),
                Location = new Point(25, 521),
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

            // ── 보관 장소 ──
            lblStorageTitle = new Label
            {
                Text = "보관 장소",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 106, 79),
                Location = new Point(25, 566),
                AutoSize = true
            };
            this.Controls.Add(lblStorageTitle);

            txtStorage = new TextBox
            {
                Size = new Size(330, 35),
                Location = new Point(25, 589),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Text = "예: 냉장고",
                ForeColor = Color.FromArgb(180, 180, 180)
            };
            txtStorage.GotFocus += (s, e) =>
            {
                if (txtStorage.Text == "예: 냉장고")
                {
                    txtStorage.Text = "";
                    txtStorage.ForeColor = Color.Black;
                }
            };
            txtStorage.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtStorage.Text))
                {
                    txtStorage.Text = "예: 냉장고";
                    txtStorage.ForeColor = Color.FromArgb(180, 180, 180);
                }
            };
            this.Controls.Add(txtStorage);

            // ── 저장 버튼 ──
            btnSave = new Button
            {
                Text = "✅ 저장",
                Size = new Size(330, 50),
                Location = new Point(25, 645),
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

        // ── 카테고리 변경 ──
        private void CmbCategory_Changed(object sender, EventArgs e)
        {
            string selected = cmbCategory.SelectedItem.ToString();

            if (CategoryService.IsOcrCategory(selected))
            {
                txtDate.Text = "예: 2025-12-31";
                txtDate.ForeColor = Color.FromArgb(180, 180, 180);
                return;
            }

            string expiryDate = CategoryService.GetExpiryDate(selected);
            if (!string.IsNullOrEmpty(expiryDate))
            {
                txtDate.Text = expiryDate;
                txtDate.ForeColor = Color.Black;
            }
        }

        // ── 파일 업로드 클릭 ──
        private void BtnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dlg.FileName;
                    picPreview.Image = System.Drawing.Image.FromFile(imagePath);
                    picPreview.Controls.Clear();
                    ProcessImage(imagePath);
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
                picPreview.Image = System.Drawing.Image.FromFile(imagePath);
                picPreview.Controls.Clear();
                ProcessImage(imagePath);
            }
        }

        // ── 이미지 처리 (OCR 실행) ──
        private void ProcessImage(string path)
        {
            string selected = cmbCategory.SelectedItem.ToString();

            // OCR 불필요한 카테고리면 스킵
            if (!CategoryService.IsOcrCategory(selected))
                return;

            // 일일 제한 체크
            if (!DbService.CheckDailyLimit())
                return;

            // API 키 확인
            if (string.IsNullOrEmpty(API_KEY))
            {
                MessageBox.Show("API 키가 없습니다!\nappsettings.json 을 확인해주세요.", "API 키 오류");
                return;
            }

            try
            {
                // OCR 실행
                string text = OcrServices.RunOCR(path, API_KEY);

                if (text == null)
                {
                    MessageBox.Show("텍스트를 인식하지 못했어요.\n더 선명한 사진으로 시도해주세요.", "인식 실패");
                    return;
                }

                // 카운트 증가
                DbService.IncreaseDailyCount();

                // 결과 파싱
                var (name, date) = OcrServices.ParseOCRResult(text, isForeignMode);

                // 이름 입력
                if (!string.IsNullOrEmpty(name))
                {
                    txtName.Text = name;
                    txtName.ForeColor = System.Drawing.Color.Black;
                }

                // 날짜 입력
                if (!string.IsNullOrEmpty(date))
                {
                    txtDate.Text = date;
                    txtDate.ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    MessageBox.Show("날짜를 찾지 못했어요.\n직접 입력해주세요.", "날짜 인식 실패");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "오류");
            }
        }

        // ── 외국 식품 모드 토글 ──
        private void BtnForeign_Click(object sender, EventArgs e)
        {
            isForeignMode = !isForeignMode;

            if (isForeignMode)
            {
                btnForeign.Text = "🌍 외국 식품 모드 ON";
                btnForeign.BackColor = System.Drawing.Color.FromArgb(45, 106, 79);
                btnForeign.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                btnForeign.Text = "🌍 외국 식품 모드 OFF";
                btnForeign.BackColor = System.Drawing.Color.FromArgb(220, 220, 210);
                btnForeign.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            }
        }

        // ── MySQL 저장 ──
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                txtName.Text == "OCR 인식 후 자동 입력" ||
                string.IsNullOrWhiteSpace(txtDate.Text) ||
                txtDate.Text == "예: 2025-12-31")
            {
                MessageBox.Show("이름과 유통기한을 입력해주세요!", "입력 오류");
                return;
            }

            bool success = DbService.SaveIngredient(
                txtName.Text,
                cmbCategory.SelectedItem.ToString(),
                txtStorage.Text == "예: 냉장고" ? "" : txtStorage.Text,
                txtDate.Text
            );

            if (success)
            {
                MessageBox.Show("✅ 저장 완료!", "성공");
                this.Close();
            }
        }
    }
}