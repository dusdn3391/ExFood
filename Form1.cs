using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExFood.Controls;

namespace ExFood
{
    public partial class Form1 : Form
    {
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
            // 1. 하단 네비 (Bottom 고정)
            this.Controls.Add(CreateBottomNav());

            // 2. + 버튼 (Bottom 고정, 네비 바로 위)
            Button addBtn = new Button
            {
                Size = new Size(55, 55),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.Transparent,  // ✅ 투명으로
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.FlatAppearance.MouseOverBackColor = Color.Transparent;  // ✅ 호버도 투명
            addBtn.FlatAppearance.MouseDownBackColor = Color.Transparent;  // ✅ 클릭도 투명
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, 55, 55);
            addBtn.Region = new Region(gp);
            // 폼 로드 후 위치 고정
            this.Load += (s, e) =>
            {
                addBtn.Location = new Point(
                    this.ClientSize.Width - 90,
                    this.ClientSize.Height - 135  // 네비(65) + 여유(15) + 버튼크기(55)
                );
            };

            // 원형 + 아이콘 직접 그리기
            addBtn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 원형 배경
                using (var brush = new SolidBrush(Color.FromArgb(45, 106, 79)))
                    e.Graphics.FillEllipse(brush, 1, 1, addBtn.Width - 3, addBtn.Height - 3);

                // + 텍스트
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

            addBtn.Click += (s, e) =>
            {
                AddFoodForm modal = new AddFoodForm();
                modal.ShowDialog();
            };

            this.Controls.Add(addBtn);
            addBtn.BringToFront(); 

            // 3. 스크롤 패널 (Fill - 마지막에 추가)
            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            FlowLayoutPanel layout = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Width = 400,
                Padding = new Padding(15, 10, 15, 10),
                BackColor = Color.Transparent
            };

            layout.Controls.Add(CreateHeader());
            //layout.Controls.Add(CreateBanner());

            layout.Controls.Add(CreateSectionLabel("🥬  Vegetables", "4 Items"));
            layout.Controls.Add(new FoodItemCard("Baby Spinach", 2, 7));
            layout.Controls.Add(new FoodItemCard("Carrots", 5, 14));

            layout.Controls.Add(CreateSectionLabel("🥛  Dairy & Eggs", "2 Items"));
            layout.Controls.Add(new FoodItemCard("Greek Yogurt", 12, 21));
            layout.Controls.Add(new FoodItemCard("Oat Milk", 1, 7));

            layout.Controls.Add(CreateSectionLabel("🐟  Proteins", "1 Item"));
            layout.Controls.Add(new FoodItemCard("Salmon Fillet", 3, 5));

            // 하단 여백
            layout.Controls.Add(new Panel
            {
                Size = new Size(370, 80),
                BackColor = Color.Transparent
            });

            scrollPanel.Controls.Add(layout);
            this.Controls.Add(scrollPanel);  // ✅ Fill은 항상 마지막
        }
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

        //private RoundedPanel CreateBanner()
        //{
        //    RoundedPanel banner = new RoundedPanel
        //    {
        //        Size = new Size(370, 105),
        //        BackColor = Color.FromArgb(45, 106, 79),
        //        CornerRadius = 16
        //    };

        //    Label lblTitle = new Label
        //    {
        //        Text = "Fridge Optimization",
        //        Font = new Font("Segoe UI", 13, FontStyle.Bold),
        //        ForeColor = Color.White,
        //        Location = new Point(15, 12),
        //        AutoSize = true
        //    };

        //    Label lblSub = new Label
        //    {
        //        Text = "3 items are expiring soon.\nWant to see some recipe ideas?",
        //        Font = new Font("Segoe UI", 9),
        //        ForeColor = Color.FromArgb(200, 235, 210),
        //        Location = new Point(15, 42),
        //        AutoSize = true
        //    };

        //    Button btnGenerate = new Button
        //    {
        //        Text = "✨ Generate\n    Recipes",
        //        Size = new Size(105, 55),
        //        Location = new Point(250, 25),
        //        BackColor = Color.White,
        //        ForeColor = Color.FromArgb(45, 106, 79),
        //        Font = new Font("Segoe UI", 9, FontStyle.Bold),
        //        FlatStyle = FlatStyle.Flat,
        //        Cursor = Cursors.Hand
        //    };
        //    btnGenerate.FlatAppearance.BorderSize = 0;

        //    banner.Controls.Add(lblTitle);
        //    banner.Controls.Add(lblSub);
        //    banner.Controls.Add(btnGenerate);
        //    return banner;
        //}

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
                {
                    e.Graphics.DrawLine(pen, 0, 0, nav.Width, 0);
                }
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
                    MessageBox.Show("Fridge 화면");
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

        private void ShowAlarmPanel()
        {
            MessageBox.Show(
                "🔴 오늘 만료: Oat Milk\n" +
                "🟠 D-1 임박: Baby Spinach\n" +
                "🟠 D-2 임박: Salmon Fillet",
                "🔔 유통기한 알림"
            );
        }

        private void ShowDisposalPanel()
        {
            MessageBox.Show(
                "♻️ 올바른 폐기 방법\n\n" +
                "🥬 채소류\n  → 음식물 쓰레기\n\n" +
                "🐟 생선 / 육류\n  → 음식물 쓰레기 (밀봉 후 배출)\n\n" +
                "🥛 유제품\n  → 내용물: 음식물 / 용기: 재활용\n\n" +
                "🥚 계란\n  → 음식물 쓰레기",
                "♻️ 폐기 가이드"
            );
        }

        private Button CreateAddButton()
        {
            Button btn = new Button
            {
                Text = "",
                Size = new Size(55, 55),
                BackColor = Color.FromArgb(45, 106, 79),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btn.FlatAppearance.BorderSize = 0;

            this.Load += (s, e) =>
            {
                btn.Location = new Point(
                    this.ClientSize.Width - 75,
                    this.ClientSize.Height - 135
                );
            };

            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(45, 106, 79)))
                    e.Graphics.FillEllipse(brush, 0, 0, btn.Width - 1, btn.Height - 1);
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString("+", new Font("Segoe UI", 22, FontStyle.Bold),
                        brush, new RectangleF(0, 0, btn.Width, btn.Height), sf);
                }
            };

            btn.Click += (s, e) => MessageBox.Show("식재료 추가!", "추가");

            return btn;
        }
    }
}