using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using ExFood.Models;
using ExFood.Services;

namespace ExFood.Forms
{
    public class AlarmForm : Form
    {
        private FlowLayoutPanel layout;
        private List<FoodItem> expiringItems;
        private List<FoodItem> expiredItems;

        public AlarmForm()
        {
            InitializeForm();
            BuildUI();
            LoadAlarms();
        }

        private void InitializeForm()
        {
            this.Text = "유통기한 알림";
            this.Size = new Size(420, 780);
            this.BackColor = Color.FromArgb(245, 243, 235);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;

            this.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(45, 106, 79), 2))
                    e.Graphics.DrawRectangle(pen, 1, 1, this.Width - 3, this.Height - 3);
            };
        }

        private void BuildUI()
        {
            // ── 헤더 ──
            Panel headerPanel = new Panel
            {
                Size = new Size(420, 55),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(45, 106, 79)
            };

            Label lblTitle = new Label
            {
                Text = "🔔 유통기한 알림",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Button btnClose = new Button
            {
                Text = "✕",
                Size = new Size(35, 35),
                Location = new Point(372, 10),
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

            // ── 스크롤 패널 ──
            Panel scrollPanel = new Panel
            {
                Location = new Point(0, 55),
                Size = new Size(420, 725),
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
        }

        private void LoadAlarms()
        {
            layout.Controls.Clear();

            expiredItems = AlarmService.GetExpiredItems();
            expiringItems = AlarmService.GetExpiringItems(2);

            // ── 만료된 식재료 ──
            if (expiredItems.Count > 0)
            {
                layout.Controls.Add(CreateSectionLabel(
                    "🔴 만료된 식재료", expiredItems.Count));

                foreach (var item in expiredItems)
                    layout.Controls.Add(CreateAlarmCard(item, true));
            }

            // ── 임박 식재료 ──
            if (expiringItems.Count > 0)
            {
                layout.Controls.Add(CreateSectionLabel(
                    "🟠 임박 식재료 (1~2일)", expiringItems.Count));

                foreach (var item in expiringItems)
                    layout.Controls.Add(CreateAlarmCard(item, false));

                // ── 레시피 추천 버튼 ──
                Button btnRecipe = new Button
                {
                    Text = "✨ 임박 식재료로 레시피 추천받기",
                    Size = new Size(370, 50),
                    BackColor = Color.FromArgb(45, 106, 79),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(0, 10, 0, 0)
                };
                btnRecipe.FlatAppearance.BorderSize = 0;
                btnRecipe.Click += BtnRecipe_Click;
                layout.Controls.Add(btnRecipe);
            }

            // ── 아무것도 없을 때 ──
            if (expiredItems.Count == 0 && expiringItems.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "✅ 임박하거나 만료된\n식재료가 없어요!",
                    Font = new Font("Segoe UI", 13),
                    ForeColor = Color.FromArgb(45, 106, 79),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(370, 100),
                    Padding = new Padding(10)
                };
                layout.Controls.Add(lblEmpty);
            }

            // 하단 여백
            layout.Controls.Add(new Panel
            {
                Size = new Size(370, 30),
                BackColor = Color.Transparent
            });
        }

        // ── 알람 카드 ──
        private Panel CreateAlarmCard(FoodItem item, bool isExpired)
        {
            Panel card = new Panel
            {
                Size = new Size(370, 65),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 5)
            };

            // 왼쪽 색상 바
            Panel colorBar = new Panel
            {
                Size = new Size(5, 65),
                Location = new Point(0, 0),
                BackColor = isExpired
                    ? Color.FromArgb(200, 60, 60)
                    : Color.FromArgb(230, 140, 50)
            };
            card.Controls.Add(colorBar);

            // 이름
            Label lblName = new Label
            {
                Text = item.Name,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(18, 10),
                AutoSize = true
            };
            card.Controls.Add(lblName);

            // 날짜
            Label lblDays = new Label
            {
                Text = isExpired
                    ? $"⚠️ {Math.Abs(item.DaysLeft)}일 지남"
                    : item.DaysLeft == 0
                        ? "⚠️ 오늘 만료!"
                        : $"⏰ D-{item.DaysLeft}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = isExpired
                    ? Color.FromArgb(200, 60, 60)
                    : Color.FromArgb(230, 140, 50),
                Location = new Point(250, 10),
                AutoSize = true
            };
            card.Controls.Add(lblDays);

            // 유통기한
            Label lblDate = new Label
            {
                Text = "📅 " + item.ExpirationDate,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(18, 38),
                AutoSize = true
            };
            card.Controls.Add(lblDate);

            // 카테고리
            Label lblCategory = new Label
            {
                Text = item.Category.Length > 10
                    ? item.Category.Substring(0, 10) + "..."
                    : item.Category,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(200, 38),
                AutoSize = true
            };
            card.Controls.Add(lblCategory);

            return card;
        }

        // ── 섹션 라벨 ──
        private Panel CreateSectionLabel(string title, int count)
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
                Text = count + " Items",
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

        // ── 레시피 추천 버튼 클릭 ──
        private void BtnRecipe_Click(object sender, EventArgs e)
        {
            RecipeForm recipeForm = new RecipeForm(expiringItems);
            recipeForm.ShowDialog();
        }
    }
}
