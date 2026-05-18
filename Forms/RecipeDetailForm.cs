using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using ExFood.Service;

namespace ExFood.Forms
{
    public class RecipeDetailForm : Form
    {
        private Recipe recipe;

        public RecipeDetailForm(Recipe recipe)
        {
            this.recipe = recipe;
            InitializeForm();
            BuildUI();
        }

        private void InitializeForm()
        {
            this.Text = recipe.Name;
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
                Text = recipe.Name,
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

            FlowLayoutPanel layout = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Width = 400,
                Padding = new Padding(15, 10, 15, 10),
                BackColor = Color.Transparent
            };

            // ── 칼로리 ──
            if (!string.IsNullOrEmpty(recipe.Calorie))
            {
                Label lblCalorie = new Label
                {
                    Text = "🔥 칼로리: " + recipe.Calorie + " kcal",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(230, 140, 50),
                    AutoSize = true,
                    Margin = new Padding(0, 5, 0, 10)
                };
                layout.Controls.Add(lblCalorie);
            }

            // ── 재료 ──
            Label lblPartsTitle = new Label
            {
                Text = "🥘 재료",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 106, 79),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            };
            layout.Controls.Add(lblPartsTitle);

            Panel partsPanel = new Panel
            {
                Size = new Size(370, 80),
                BackColor = Color.White,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 15)
            };

            Label lblParts = new Label
            {
                Text = recipe.Parts,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(10, 10),
                Size = new Size(350, 60),
                AutoSize = false
            };
            partsPanel.Controls.Add(lblParts);
            layout.Controls.Add(partsPanel);

            // ── 조리순서 ──
            Label lblManualTitle = new Label
            {
                Text = "📋 조리순서",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 106, 79),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            };
            layout.Controls.Add(lblManualTitle);

            Panel manualPanel = new Panel
            {
                Size = new Size(370, 400),
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            Label lblManual = new Label
            {
                Text = recipe.FullManual,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(10, 10),
                Size = new Size(350, 380),
                AutoSize = false
            };
            manualPanel.Controls.Add(lblManual);
            layout.Controls.Add(manualPanel);

            scrollPanel.Controls.Add(layout);
            this.Controls.Add(scrollPanel);
        }
    }
}

