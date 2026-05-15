using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ExFood.Controls
{
    internal class FreshnessBar : Control
    {
        private int _value = 100;

        public int Value
        {
            get => _value;
            set
            {
                if (value < 0) _value = 0;
                else if (value > 100) _value = 100;
                else _value = value;
                Invalidate();
            }
        }

        public Color BarColor
        {
            get
            {
                if (_value <= 20) return Color.FromArgb(200, 60, 60);   // 빨강
                else if (_value <= 50) return Color.FromArgb(230, 140, 50); // 주황
                else return Color.FromArgb(45, 106, 79);                // 초록
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 배경 바
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(220, 220, 220)))
            {
                g.FillRectangle(bgBrush, 0, 0, Width, Height);
            }

            // 채워진 바
            int fillWidth = (int)(Width * (_value / 100.0));
            if (fillWidth > 0)
            {
                using (SolidBrush fillBrush = new SolidBrush(BarColor))
                {
                    g.FillRectangle(fillBrush, 0, 0, fillWidth, Height);
                }
            }
        }
    }
}
