using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ExFood.Forms
{
    public class DisposalGuideForm : Form
    {
        // ── 폐기 가이드 데이터 ──
        private readonly List<DisposalGuide> guides = new List<DisposalGuide>
        {
            new DisposalGuide
            {
                Category = "🥬 채소 / 과일",
                Color = Color.FromArgb(45, 106, 79),
                Steps = new List<string>
                {
                    "✅ 채소 잎, 뿌리, 껍질 → 음식물 쓰레기",
                    "✅ 과일 과육, 껍질 → 음식물 쓰레기",
                    "❌ 과일 씨앗 (복숭아, 살구 등) → 일반쓰레기",
                    "❌ 딱딱한 껍질 (파인애플, 코코넛) → 일반쓰레기",
                    "❌ 옥수수 속대 → 일반쓰레기"
                },
                Tips = "💡 부드럽고 잘 썩는 것은 음식물, 딱딱한 것은 일반쓰레기!"
            },
            new DisposalGuide
            {
                Category = "🐟 생선 / 해산물",
                Color = Color.FromArgb(30, 100, 180),
                Steps = new List<string>
                {
                    "✅ 생선 살, 내장 → 음식물 쓰레기",
                    "✅ 새우, 조개 살 → 음식물 쓰레기",
                    "❌ 생선 뼈 → 일반쓰레기",
                    "❌ 조개, 굴 껍데기 → 일반쓰레기",
                    "⚠️ 반드시 밀봉 후 배출 (냄새 방지)"
                },
                Tips = "💡 뼈와 껍데기는 일반쓰레기, 살은 음식물쓰레기!"
            },
            new DisposalGuide
            {
                Category = "🥩 육류",
                Color = Color.FromArgb(180, 60, 60),
                Steps = new List<string>
                {
                    "✅ 고기 살 → 음식물 쓰레기",
                    "❌ 닭뼈, 돼지뼈, 소뼈 → 일반쓰레기",
                    "❌ 털, 깃털 → 일반쓰레기",
                    "⚠️ 반드시 밀봉 후 배출 (냄새 방지)",
                    "⚠️ 여름철엔 냉동 후 배출 권장"
                },
                Tips = "💡 뼈는 일반쓰레기, 고기 살은 음식물쓰레기!"
            },
            new DisposalGuide
            {
                Category = "🥛 유제품",
                Color = Color.FromArgb(100, 150, 200),
                Steps = new List<string>
                {
                    "✅ 우유, 요거트 내용물 → 음식물 쓰레기",
                    "♻️ 우유팩 → 종이류 재활용",
                    "♻️ 플라스틱 용기 → 세척 후 플라스틱 재활용",
                    "♻️ 유리병 → 세척 후 유리 재활용",
                    "⚠️ 용기는 반드시 세척 후 분리배출"
                },
                Tips = "💡 내용물은 음식물, 용기는 세척 후 재활용!"
            },
            new DisposalGuide
            {
                Category = "🥚 계란",
                Color = Color.FromArgb(200, 160, 50),
                Steps = new List<string>
                {
                    "✅ 계란 내용물 → 음식물 쓰레기",
                    "❌ 계란 껍데기 → 일반쓰레기",
                    "♻️ 계란 판지 포장 → 종이 재활용",
                    "♻️ 플라스틱 계란판 → 플라스틱 재활용",
                    "⚠️ 썩은 계란은 밀봉 후 배출"
                },
                Tips = "💡 껍데기는 일반쓰레기, 내용물은 음식물쓰레기!"
            },
            new DisposalGuide
            {
                Category = "🥫 가공식품",
                Color = Color.FromArgb(150, 100, 50),
                Steps = new List<string>
                {
                    "✅ 내용물 → 음식물 쓰레기",
                    "♻️ 캔 → 세척 후 캔 재활용",
                    "♻️ 유리병 → 세척 후 유리 재활용",
                    "♻️ 플라스틱 용기 → 세척 후 플라스틱 재활용",
                    "🗑️ 라벨, 뚜껑 → 일반쓰레기"
                },
                Tips = "💡 포장재는 재질별로 분리배출!"
            },
            new DisposalGuide
            {
                Category = "🧂 조미료 / 기름",
                Color = Color.FromArgb(180, 130, 50),
                Steps = new List<string>
                {
                    "✅ 소금, 설탕, 간장 소량 → 음식물 쓰레기",
                    "❌ 식용유, 참기름 → 굳혀서 일반쓰레기",
                    "❌ 대량의 조미료 → 일반쓰레기",
                    "⚠️ 기름은 절대 하수구에 버리지 마세요!",
                    "💡 기름은 신문지에 흡수시켜 일반쓰레기로"
                },
                Tips = "💡 기름류는 절대 하수구 금지! 굳혀서 일반쓰레기!"
            },
            new DisposalGuide
            {
                Category = "🍚 곡류 / 빵",
                Color = Color.FromArgb(200, 170, 100),
                Steps = new List<string>
                {
                    "✅ 밥, 면, 빵 → 음식물 쓰레기",
                    "✅ 과자 부스러기 → 음식물 쓰레기",
                    "♻️ 과자 봉지 → 비닐 재활용",
                    "♻️ 종이 포장 → 종이 재활용",
                    "⚠️ 소량씩 나눠서 배출 권장"
                },
                Tips = "💡 곡류는 대부분 음식물쓰레기, 포장재만 분리배출!"
            }
        };

        public DisposalGuideForm()
        {
            InitializeForm();
            BuildUI();
        }

        private void InitializeForm()
        {
            this.Text = "폐기 가이드";
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
                Text = "♻️ 올바른 폐기 가이드",
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

            // 가이드 카드 추가
            foreach (var guide in guides)
            {
                layout.Controls.Add(CreateGuideCard(guide));
            }

            // 하단 여백
            layout.Controls.Add(new Panel
            {
                Size = new Size(370, 30),
                BackColor = Color.Transparent
            });

            scrollPanel.Controls.Add(layout);
            this.Controls.Add(scrollPanel);
        }

        // ── 가이드 카드 만들기 ──
        private Panel CreateGuideCard(DisposalGuide guide)
        {
            // 카드 높이 계산
            int cardHeight = 60 + (guide.Steps.Count * 25) + 45;

            Panel card = new Panel
            {
                Size = new Size(370, cardHeight),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 10)
            };

            // 카테고리 헤더
            Panel categoryHeader = new Panel
            {
                Size = new Size(370, 40),
                Location = new Point(0, 0),
                BackColor = guide.Color
            };

            Label lblCategory = new Label
            {
                Text = guide.Category,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 10),
                AutoSize = true
            };

            categoryHeader.Controls.Add(lblCategory);
            card.Controls.Add(categoryHeader);

            // 처리 방법 목록
            int yPos = 48;
            foreach (string step in guide.Steps)
            {
                Label lblStep = new Label
                {
                    Text = step,
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(50, 50, 50),
                    Location = new Point(15, yPos),
                    AutoSize = true
                };
                card.Controls.Add(lblStep);
                yPos += 25;
            }

            // 팁
            Panel tipPanel = new Panel
            {
                Size = new Size(370, 35),
                Location = new Point(0, cardHeight - 38),
                BackColor = Color.FromArgb(240, 248, 240)
            };

            Label lblTip = new Label
            {
                Text = guide.Tips,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(45, 106, 79),
                Location = new Point(10, 10),
                AutoSize = true
            };

            tipPanel.Controls.Add(lblTip);
            card.Controls.Add(tipPanel);

            return card;
        }
    }

    // ── 폐기 가이드 데이터 모델 ──
    public class DisposalGuide
    {
        public string Category { get; set; }
        public Color Color { get; set; }
        public List<string> Steps { get; set; }
        public string Tips { get; set; }
    }
}