using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ExFood.Controls;
using ExFood.Forms;
using ExFood.Models;
using ExFood.Services;

namespace ExFood
{
    public partial class Form1 : Form
    {
        private FlowLayoutPanel layout;
        private Panel scrollPanel;

        public Form1()
        {
            InitializeComponent();
            SetupForm();
            BuildUI();
        }

        private void SetupForm()
        {
            this.Text = "KitchenFresh";
            this.Size = new Size(420, 780);
            this.BackColor = Color.FromArgb(245, 243, 235);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void BuildUI()
        {
            // 1. 하단 네비
            this.Controls.Add(CreateBottomNav());

            // 2. + 버튼
            Button addBtn = new Button
            {
                Size = new Size(55, 55),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            addBtn.FlatAppearance.MouseDownBackColor = Color.Transparent;

            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, 55, 55);
            addBtn.Region = new Region(gp);

            this.Load += (s, e) =>
            {
                addBtn.Location = new Point(
                    this.ClientSize.Width - 90,
                    this.ClientSize.Height - 135
                );
            };

            addBtn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(45, 106, 79)))
                    e.Graphics.FillEllipse(brush, 1, 1, addBtn.Width - 3, addBtn.Height - 3);
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 24, FontStyle.Bold))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString("+", font, brush,
                        new RectangleF(0, 0, addBtn.Width, addBtn.Height), sf);
                }
            };

            // ✅ 저장 후 목록 새로고침
            addBtn.Click += (s, e) =>
            {
                AddFoodForm modal = new AddFoodForm();
                modal.FormClosed += (ms, me) => LoadIngredients(); // 닫히면 새로고침
                modal.ShowDialog();
            };

            this.Controls.Add(addBtn);
            addBtn.BringToFront();

            // 3. 스크롤 패널
            scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            layout = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Width = 400,
                Padding = new Padding(15, 10, 15, 10),
                BackColor = Color.Transparent
            };

            scrollPanel.Controls.Add(layout);
            this.Controls.Add(scrollPanel);

            // 4. 첫 로딩
            this.Load += (s, e) => LoadIngredients();
        }

        // ── DB 에서 식재료 불러와서 화면에 표시 ──
        private void LoadIngredients()
        {
            layout.Controls.Clear();

            // 헤더
            layout.Controls.Add(CreateHeader());

            // DB 에서 카테고리별 데이터 불러오기
            var categoryData = DbService.GetIngredientsByCategory();

            if (categoryData.Count == 0)
            {
                // 데이터 없을 때
                Label lblEmpty = new Label
                {
                    Text = "아직 등록된 식재료가 없어요!\n+ 버튼을 눌러 추가해보세요 🌿",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(150, 150, 150),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(370, 100),
                    Padding = new Padding(10)
                };
                layout.Controls.Add(lblEmpty);
            }
            else
            {
                // 카테고리별로 표시
                foreach (var category in categoryData)
                {
                    string categoryName = category.Key;
                    List<FoodItem> items = category.Value;

                    // 섹션 라벨
                    layout.Controls.Add(
                        CreateSectionLabel(
                            GetCategoryEmoji(categoryName) + "  " + categoryName,
                            items.Count + " Items"
                        )
                    );

                    // 각 식재료 카드
                    foreach (var item in items)
                    {
                        layout.Controls.Add(CreateFoodCard(item));
                    }
                }
            }

            // 하단 여백
            layout.Controls.Add(new Panel
            {
                Size = new Size(370, 80),
                BackColor = Color.Transparent
            });
        }

        // ── 식재료 카드 만들기 (이미지 없이) ──
        private Panel CreateFoodCard(FoodItem item)
        {
            Panel card = new Panel
            {
                Size = new Size(370, 70),
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // 이름
            Label lblName = new Label
            {
                Text = item.Name,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(15, 10),
                AutoSize = true
            };

            // 남은 날짜
            Label lblDays = new Label
            {
                Text = item.DaysLeft >= 0
                    ? item.DaysLeft + " days left"
                    : "만료됨",
                Font = new Font("Segoe UI", 10),
                ForeColor = GetDaysColor(item.DaysLeft),
                Location = new Point(200, 10),
                AutoSize = true
            };

            // 보관 장소
            Label lblStorage = new Label
            {
                Text = string.IsNullOrEmpty(item.StoragePlace)
                    ? "" : "📍 " + item.StoragePlace,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(15, 35),
                AutoSize = true
            };

            // 신선도 바
            FreshnessBar bar = new FreshnessBar
            {
                Location = new Point(15, 52),
                Size = new Size(300, 7),
                Value = item.MaxDays > 0
                    ? Math.Max(0, (int)((item.DaysLeft / (double)item.MaxDays) * 100))
                    : 0
            };

            // ✅ 삭제 버튼
            Button btnDelete = new Button
            {
                Text = "🗑️",
                Size = new Size(35, 35),
                Location = new Point(325, 17),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 240, 240),
                ForeColor = Color.FromArgb(200, 60, 60),
                Font = new Font("Segoe UI", 11),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;

            btnDelete.Click += (s, e) =>
            {
                // 삭제 확인 메시지
                DialogResult confirm = MessageBox.Show(
                    $"'{item.Name}' 을(를) 삭제할까요?",
                    "삭제 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirm == DialogResult.Yes)
                {
                    bool success = DbService.DeleteIngredient(item.Id);
                    if (success)
                    {
                        MessageBox.Show("✅ 삭제 완료!", "성공");
                        LoadIngredients(); // 목록 새로고침
                    }
                }
            };

            card.Controls.Add(lblName);
            card.Controls.Add(lblDays);
            card.Controls.Add(lblStorage);
            card.Controls.Add(bar);
            card.Controls.Add(btnDelete);

            return card;
        }

        // ── 날짜별 색상 ──
        private Color GetDaysColor(int days)
        {
            if (days <= 0) return Color.FromArgb(200, 60, 60);   // 빨강
            if (days <= 2) return Color.FromArgb(200, 60, 60);   // 빨강
            if (days <= 5) return Color.FromArgb(230, 140, 50);  // 주황
            return Color.FromArgb(45, 106, 79);                   // 초록
        }

        // ── 카테고리 이모지 ──
        private string GetCategoryEmoji(string category)
        {
            if (category.Contains("가공식품")) return "🥫";
            if (category.Contains("채소")) return "🥬";
            if (category.Contains("생선")) return "🐟";
            if (category.Contains("육류")) return "🥩";
            if (category.Contains("과일")) return "🍎";
            if (category.Contains("계란")) return "🥚";
            if (category.Contains("유제품")) return "🥛";
            if (category.Contains("조미료")) return "🧂";
            if (category.Contains("곡류")) return "🍚";
            return "🍽️";
        }

        // ── 헤더 ──
        private Panel CreateHeader()
        {
            Panel panel = new Panel
            {
                Size = new Size(370, 55),
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "🌿 KitchenFresh",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 100, 60),
                Location = new Point(0, 8),
                AutoSize = true
            };

            panel.Controls.Add(lblTitle);
            return panel;
        }

        // ── 섹션 라벨 ──
        private Panel CreateSectionLabel(string title, string count)
        {
            Panel panel = new Panel
            {
                Size = new Size(370, 40),
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 106, 79),
                Location = new Point(0, 8),
                AutoSize = true
            };

            Label lblCount = new Label
            {
                Text = count,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.FromArgb(220, 220, 210),
                Location = new Point(280, 10),
                Size = new Size(70, 22),
                TextAlign = ContentAlignment.MiddleCenter
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblCount);
            return panel;
        }

        // ── 하단 네비 ──
        private Panel CreateBottomNav()
        {
            Panel nav = new Panel
            {
                Height = 65,
                Dock = DockStyle.Bottom,
                BackColor = Color.White
            };

            nav.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                    e.Graphics.DrawLine(pen, 0, 0, nav.Width, 0);
            };

            string[] labels = { "🧊\nFridge", "🔔\n알림", "♻️\n폐기", "⚙️\nSettings" };
            int btnWidth = 90;
            int startX = 15;

            for (int i = 0; i < labels.Length; i++)
            {
                int index = i;
                Button btn = new Button
                {
                    Text = labels[i],
                    Size = new Size(btnWidth, 55),
                    Location = new Point(startX + (i * (btnWidth + 5)), 5),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = i == 0
                        ? Color.FromArgb(45, 106, 79)
                        : Color.FromArgb(150, 150, 150),
                    Font = new Font("Segoe UI", 8, FontStyle.Regular),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => OnNavClick(index, nav);
                nav.Controls.Add(btn);
            }

            return nav;
        }

        // ── 네비 클릭 ──
        private void OnNavClick(int index, Panel nav)
        {
            foreach (Control c in nav.Controls)
            {
                if (c is Button b)
                    b.ForeColor = Color.FromArgb(150, 150, 150);
            }
            ((Button)nav.Controls[index]).ForeColor = Color.FromArgb(45, 106, 79);

            switch (index)
            {
                case 0:
                    LoadIngredients();
                    break;
                case 1:
                    ShowAlarmPanel();
                    break;
                case 2:
                    ShowDisposalPanel();
                    break;
                case 3:
                    MessageBox.Show("Settings 화면");
                    break;
            }
        }

        // ── 알림 패널 ──
        private void ShowAlarmPanel()
        {
            var items = DbService.GetAllIngredients();
            string msg = "";

            foreach (var item in items)
            {
                if (item.DaysLeft < 0)
                    msg += $"🔴 만료됨: {item.Name}\n";
                else if (item.DaysLeft == 0)
                    msg += $"🔴 오늘 만료: {item.Name}\n";
                else if (item.DaysLeft <= 2)
                    msg += $"🟠 D-{item.DaysLeft} 임박: {item.Name}\n";
            }

            if (string.IsNullOrEmpty(msg))
                msg = "✅ 임박한 식재료가 없어요!";

            MessageBox.Show(msg, "🔔 유통기한 알림");
        }

        // ── 폐기 가이드 ──
        private void ShowDisposalPanel()
        {
            DisposalGuideForm guide = new DisposalGuideForm();
            guide.ShowDialog();

        }

    }
}