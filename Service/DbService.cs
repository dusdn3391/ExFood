using ExFood.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Npgsql; 

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
                using (var conn = new NpgsqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM ingredients ORDER BY expirationdate ASC";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new FoodItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Category = reader.IsDBNull(reader.GetOrdinal("category"))
                                                 ? "" : reader.GetString(reader.GetOrdinal("category")),
                                StoragePlace = reader.IsDBNull(reader.GetOrdinal("storageplace"))
                                                 ? "" : reader.GetString(reader.GetOrdinal("storageplace")),
                                ExpirationDate = reader.GetDateTime(reader.GetOrdinal("expirationdate"))
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
                string category = CleanCategory(item.Category);

                if (!dict.ContainsKey(category))
                    dict[category] = new List<FoodItem>();

                dict[category].Add(item);
            }

            return dict;
        }

        // ── 식재료 저장 ──
        public static bool SaveIngredient(string name, string category,
            string storage, string date)
        {
            try
            {
                using (var conn = new NpgsqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO ingredients 
                    (name, category, storageplace, expirationdate) 
                    VALUES (@name, @category, @storage, @date)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@storage", storage);
                        cmd.Parameters.AddWithValue("@date", DateTime.Parse(date));
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
                using (var conn = new NpgsqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM ingredients WHERE id = @id";

                    using (var cmd = new NpgsqlCommand(query, conn))
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

        // ── 일일 제한 체크 ──
        public static bool CheckDailyLimit(int limit = 20)
        {
            try
            {
                using (var conn = new NpgsqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT itemcount FROM api_usage WHERE usagedate = @today";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@today", DateTime.Today);
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
                using (var conn = new NpgsqlConnection(AppConfig.ConnectionString))
                {
                    conn.Open();

                    // ✅ PostgreSQL 문법 (ON CONFLICT)
                    string query = @"INSERT INTO api_usage (usagedate, itemcount) 
                               VALUES (@today, 1)
                               ON CONFLICT (usagedate) 
                               DO UPDATE SET itemcount = api_usage.itemcount + 1";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@today", DateTime.Today);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        // ── 카테고리 이름 정리 ──
        private static string CleanCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return "기타";

            string clean = System.Text.RegularExpressions.Regex
                .Replace(category, @"\(.*?\)", "").Trim();

            clean = System.Text.RegularExpressions.Regex
                .Replace(clean, @"[^\u0000-\u007F\uAC00-\uD7A3\u1100-\u11FF\u3130-\u318F\s]", "")
                .Trim();

            return string.IsNullOrEmpty(clean) ? "기타" : clean;
        }
    }

}