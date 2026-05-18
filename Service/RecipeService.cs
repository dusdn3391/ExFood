using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Forms;

namespace ExFood.Service
{
    public static class RecipeService
    {
        private static string API_KEY = ExFood.Models.AppConfig.FoodSafetyKey;
        private static string BASE_URL = "http://openapi.foodsafetykorea.go.kr/api";

        // ── 식재료로 레시피 검색 ──
        public static List<Recipe> SearchRecipes(string ingredient)
        {
            List<Recipe> recipes = new List<Recipe>();

            try
            {
                string url = $"{BASE_URL}/{API_KEY}/COOKRCP01/json/1/100";

                using (var client = new HttpClient())
                {
                    string responseJson = client.GetStringAsync(url).Result;

                    // ✅ result 변수 추가
                    dynamic result = JsonConvert.DeserializeObject(responseJson);

                    var rows = result.COOKRCP01.row;

                    foreach (var row in rows)
                    {
                        string parts = row.RCP_PARTS_DTLS?.ToString() ?? "";
                        string name = row.RCP_NM?.ToString() ?? "";

                        // 재료 또는 이름에 검색어 포함된 것만
                        if (parts.Contains(ingredient) ||
                            name.Contains(ingredient))
                        {
                            recipes.Add(new Recipe
                            {
                                Name = name,
                                Parts = parts,
                                ImageUrl = row.ATT_FILE_NO_MAIN?.ToString() ?? "",
                                Manual01 = row.MANUAL01?.ToString() ?? "",
                                Manual02 = row.MANUAL02?.ToString() ?? "",
                                Manual03 = row.MANUAL03?.ToString() ?? "",
                                Manual04 = row.MANUAL04?.ToString() ?? "",
                                Manual05 = row.MANUAL05?.ToString() ?? "",
                                Manual06 = row.MANUAL06?.ToString() ?? "",
                                Manual07 = row.MANUAL07?.ToString() ?? "",
                                Manual08 = row.MANUAL08?.ToString() ?? "",
                                Manual09 = row.MANUAL09?.ToString() ?? "",
                                Manual10 = row.MANUAL10?.ToString() ?? "",
                                Calorie = row.INFO_ENG?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("레시피 검색 오류: " + ex.Message);
            }

            return recipes;
        }
    }

    // ── 레시피 데이터 모델 ──
    public class Recipe
    {
        public string Name { get; set; }
        public string Parts { get; set; }
        public string ImageUrl { get; set; }
        public string Manual01 { get; set; }
        public string Manual02 { get; set; }
        public string Manual03 { get; set; }
        public string Manual04 { get; set; }
        public string Manual05 { get; set; }
        public string Manual06 { get; set; }
        public string Manual07 { get; set; }
        public string Manual08 { get; set; }
        public string Manual09 { get; set; }
        public string Manual10 { get; set; }
        public string Calorie { get; set; }

        // ✅ 조리순서 - 빈값 스킵 + 번호 재정렬
        public string FullManual
        {
            get
            {
                var manuals = new string[]
                {
                    Manual01, Manual02, Manual03, Manual04, Manual05,
                    Manual06, Manual07, Manual08, Manual09, Manual10
                };

                string result = "";
                int step = 1;

                foreach (string manual in manuals)
                {
                    if (string.IsNullOrWhiteSpace(manual))
                        continue;

                    // 앞에 숫자 제거 후 새로 번호 붙이기
                    string cleaned = System.Text.RegularExpressions
                        .Regex.Replace(manual.Trim(), @"^\d+\.\s*", "");

                    result += step + ". " + cleaned + "\n\n";
                    step++;
                }

                return result.Trim();
            }
        }
    }
}