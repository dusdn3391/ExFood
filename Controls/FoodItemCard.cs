using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ExFood.Controls
{
    internal class FoodItemCard : UserControl
    {
        private PictureBox picFood;
        private Label lblName;
        private Label lblDaysLeft;
        private FreshnessBar freshnessBar;

        public FoodItemCard(string name, int daysLeft, int maxDays, Image image = null)
        {
            InitializeCard();
            SetData(name, daysLeft, maxDays, image);
        }

        private void InitializeCard()
        {
            this.Size = new Size(370, 85);
            this.BackColor = Color.White;
            this.Padding = new Padding(10);

            // 음식 이미지
            picFood = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(10, 12),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 245, 235)
            };

            // 음식 이름
            lblName = new Label
            {
                Location = new Point(85, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };

            // 남은 날짜
            lblDaysLeft = new Label
            {
                Location = new Point(230, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            // 신선도 바
            freshnessBar = new FreshnessBar
            {
                Location = new Point(85, 52),
                Size = new Size(265, 8)
            };

            this.Controls.Add(picFood);
            this.Controls.Add(lblName);
            this.Controls.Add(lblDaysLeft);
            this.Controls.Add(freshnessBar);
        }

        private void SetData(string name, int daysLeft, int maxDays, Image image)
        {
            lblName.Text = name;
            lblDaysLeft.Text = daysLeft + " days left";

            // 날짜에 따른 텍스트 색상
            if (daysLeft <= 2)
                lblDaysLeft.ForeColor = Color.FromArgb(200, 60, 60);   // 빨강
            else if (daysLeft <= 5)
                lblDaysLeft.ForeColor = Color.FromArgb(230, 140, 50);  // 주황
            else
                lblDaysLeft.ForeColor = Color.FromArgb(45, 106, 79);   // 초록

            // 신선도 바 값 계산 (0~100)
            int percent = (int)((daysLeft / (double)maxDays) * 100);
            freshnessBar.Value = percent;

            // 이미지 설정
            if (image != null)
                picFood.Image = image;
        }
    }
}
