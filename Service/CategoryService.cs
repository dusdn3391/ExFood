using System;

namespace ExFood.Services
{
    public static class CategoryService
    {
        // ── 카테고리 목록 ──
        public static string[] GetCategories()
        {
            return new string[]
            {
                "🥫 가공식품 (OCR 사용)",
                "🥬 채소 (구매일 +7일)",
                "🐟 생선 (구매일 +2일)",
                "🥩 육류 (구매일 +3일)",
                "🍎 과일 (구매일 +5일)",
                "🥚 계란 (구매일 +14일)",
                "🥛 유제품 (OCR 사용)",
                "🧂 조미료 (구매일 +365일)",
                "🍚 곡류 (구매일 +180일)"
            };
        }

        // ── OCR 사용 여부 ──
        public static bool IsOcrCategory(string category)
        {
            return category.Contains("OCR 사용");
        }

        // ── 카테고리별 유통기한 날짜 계산 ──
        public static string GetExpiryDate(string category)
        {
            int days = GetExpiryDays(category);
            if (days > 0)
                return DateTime.Today.AddDays(days).ToString("yyyy-MM-dd");
            return "";
        }

        // ── 카테고리별 유통기한 일수 ──
        private static int GetExpiryDays(string category)
        {
            if (category.Contains("채소")) return 7;
            if (category.Contains("생선")) return 2;
            if (category.Contains("육류")) return 3;
            if (category.Contains("과일")) return 5;
            if (category.Contains("계란")) return 14;
            if (category.Contains("조미료")) return 365;
            if (category.Contains("곡류")) return 180;
            return 0;
        }
    }
}