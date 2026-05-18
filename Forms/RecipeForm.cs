using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using ExFood.Models;
using ExFood.Service;

namespace ExFood.Forms
{
    public class RecipeForm : Form
    {
        private List<FoodItem> expiringItems;
        private FlowLayoutPanel layout;
        private Label lblLoading;

        public RecipeForm(List<FoodItem> items)
        {
            expiringItems = items;
            InitializeForm();
            BuildUI();
            LoadRecipes();
        }

        private void InitializeForm()
        {
            this.Text = "레시피 추천";
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
                Text = "✨ 레시피 추천",
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

            // ── 임박 식재료 표시 ──
            Panel ingredientPanel = new Panel
            {
                Size = new Size(420, 40),
                Location = new Point(0, 55),
                BackColor = Color.FromArgb(255, 248, 230)
            };

            string ingredientNames = "";
            foreach (var item in expiringItems)
                ingredientNames += item.Name + ", ";
            ingredientNames = ingredientNames.TrimEnd(',', ' ');

            Label lblIngredients = new Label
            {
                Text = "🥬 임박 식재료: " + ingredientNames,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 100, 0),
                Location = new Point(15, 12),
                AutoSize = true
            };
            ingredientPanel.Controls.Add(lblIngredients);
            this.Controls.Add(ingredientPanel);

            // ── 로딩 라벨 ──
            lblLoading = new Label
            {
                Text = "레시피 검색 중...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(150, 150, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(420, 50),
                Location = new Point(0, 100)
            };
            this.Controls.Add(lblLoading);

            // ── 스크롤 패널 ──
            Panel scrollPanel = new Panel
            {
                Location = new Point(0, 95),
                Size = new Size(420, 685),
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

        // ── 레시피 불러오기 ──
        private void LoadRecipes()
        {
            layout.Controls.Clear();
            lblLoading.Visible = true;

            // 임박 식재료별로 레시피 검색
            foreach (var item in expiringItems)
            {
                var recipes = RecipeService.SearchRecipes(item.Name);

                if (recipes.Count > 0)
                {
                    // 섹션 라벨
                    Label lblSection = new Label
                    {
                        Text = $"🍽️ '{item.Name}' 으로 만들 수 있는 레시피",
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.FromArgb(45, 106, 79),
                        AutoSize = true,
                        Margin = new Padding(0, 10, 0, 5)
                    };
                    layout.Controls.Add(lblSection);

                    // 레시피 카드
                    foreach (var recipe in recipes)
                        layout.Controls.Add(CreateRecipeCard(recipe));
                }
            }

            lblLoading.Visible = false;

            if (layout.Controls.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "추천 레시피를 찾지 못했어요 😢\n다른 식재료로 검색해보세요!",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(150, 150, 150),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(370, 100)
                };
                layout.Controls.Add(lblEmpty);
            }
        }

        // ── 레시피 카드 ──
        private Panel CreateRecipeCard(Recipe recipe)
        {
            Panel card = new Panel
            {
                Size = new Size(370, 90),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 8),
                Cursor = Cursors.Hand
            };

            // 레시피 이름
            Label lblName = new Label
            {
                Text = recipe.Name,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(15, 12),
                AutoSize = true
            };

            // 칼로리
            Label lblCalorie = new Label
            {
                Text = string.IsNullOrEmpty(recipe.Calorie)
                    ? "" : "🔥 " + recipe.Calorie + " kcal",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(230, 140, 50),
                Location = new Point(15, 38),
                AutoSize = true
            };

            // 재료 일부
            string partsPreview = recipe.Parts.Length > 50
                ? recipe.Parts.Substring(0, 50) + "..."
                : recipe.Parts;

            Label lblParts = new Label
            {
                Text = "🥘 " + partsPreview,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(15, 58),
                Size = new Size(320, 20)
            };

            // 화살표
            Label lblArrow = new Label
            {
                Text = "›",
                Font = new Font("Segoe UI", 20),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(340, 28),
                AutoSize = true
            };

            card.Controls.Add(lblName);
            card.Controls.Add(lblCalorie);
            card.Controls.Add(lblParts);
            card.Controls.Add(lblArrow);

            // 클릭시 상세보기
            card.Click += (s, e) => ShowRecipeDetail(recipe);
            lblName.Click += (s, e) => ShowRecipeDetail(recipe);
            lblParts.Click += (s, e) => ShowRecipeDetail(recipe);

            return card;
        }

        // ── 레시피 상세보기 ──
        private void ShowRecipeDetail(Recipe recipe)
        {
            RecipeDetailForm detail = new RecipeDetailForm(recipe);
            detail.ShowDialog();
        }
    }
}
