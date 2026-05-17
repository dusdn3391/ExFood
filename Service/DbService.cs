using ExFood.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExFood.Services
{
    public static class DbService
    {
        // ── 전체 식재료 불러오기 ──
        public static List<FoodItem> GetAllIngredients()
        {
            List<FoodItem> list = new List<FoodItem>();

            try
            {
                using (var conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM ingredients ORDER BY ExpirationDate ASC";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new FoodItem
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Category = reader.IsDBNull(reader.GetOrdinal("Category"))
                                                 ? "" : reader.GetString("Category"),
                                StoragePlace = reader.IsDBNull(reader.GetOrdinal("StoragePlace"))
                                                 ? "" : reader.GetString("StoragePlace"),
                                ExpirationDate = reader.GetDateTime("ExpirationDate")
                                                 .ToString("yyyy-MM-dd")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("데이터 불러오기 오류: " + ex.Message);
            }

            return list;
        }

        // ── 카테고리별 식재료 불러오기 ──
        public static Dictionary<string, List<FoodItem>> GetIngredientsByCategory()
        {
            var all = GetAllIngredients();
            var dict = new Dictionary<string, List<FoodItem>>();

            foreach (var item in all)
            {
                // 카테고리 이름 정리 (이모지 + 괄호 제거)
                string category = CleanCategory(item.Category);

                if (!dict.ContainsKey(category))
                    dict[category] = new List<FoodItem>();

                dict[category].Add(item);
            }

            return dict;
        }

        // ── 카테고리 이름 정리 ──
        private static string CleanCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return "기타";

            // 괄호 안 내용 제거 (예: "🥫 가공식품 (OCR 사용)" → "가공식품")
            string clean = System.Text.RegularExpressions.Regex
                .Replace(category, @"\(.*?\)", "").Trim();

            // 이모지 제거
            clean = System.Text.RegularExpressions.Regex
                .Replace(clean, @"[^\u0000-\u007F\uAC00-\uD7A3\u1100-\u11FF\u3130-\u318F\s]", "")
                .Trim();

            return string.IsNullOrEmpty(clean) ? "기타" : clean;
        }
        // ── 일일 제한 체크 ──
        public static bool CheckDailyLimit(int limit = 20)
        {
            try
            {
                using (var conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT ItemCount FROM api_usage WHERE UsageDate = @today";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@today", DateTime.Today.ToString("yyyy-MM-dd"));
                        var result = cmd.ExecuteScalar();
                        int count = result != null ? Convert.ToInt32(result) : 0;

                        if (count >= limit)
                        {
                            MessageBox.Show(
                                $"오늘 OCR 사용 횟수({limit}회)를 초과했어요!\n내일 다시 시도해주세요.",
                                "사용량 초과"
                            );
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch
            {
                return true;
            }
        }

        // ── 일일 사용 횟수 증가 ──
        public static void IncreaseDailyCount()
        {
            try
            {
                using (var conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO api_usage (UsageDate, ItemCount) 
                                   VALUES (@today, 1)
                                   ON DUPLICATE KEY UPDATE ItemCount = ItemCount + 1";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@today", DateTime.Today.ToString("yyyy-MM-dd"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        // ── 식재료 저장 ──
        public static bool SaveIngredient(string name, string category,
     string storage, string date)
        {
            try
            {
                // ✅ 이모지 제거 후 저장
                string cleanCategory = System.Text.RegularExpressions.Regex
                    .Replace(category, @"[^\u0000-\u007F\uAC00-\uD7A3\u1100-\u11FF\u3130-\u318F]", "")
                    .Trim();

                using (var conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO ingredients 
                (Name, Category, StoragePlace, ExpirationDate) 
                VALUES (@name, @category, @storage, @date)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@category", cleanCategory);
                        cmd.Parameters.AddWithValue("@storage", storage);
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("저장 오류: " + ex.Message);
                return false;
            }
        }
        // ── 식재료 삭제 ──
        public static bool DeleteIngredient(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM ingredients WHERE Id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("삭제 오류: " + ex.Message);
                return false;
            }
        }

    }
}