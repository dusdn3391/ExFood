using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace ExFood.Services
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
                string url = $"{BASE_URL}/{API_KEY}/COOKRCP01/json/1/20";

                using (var client = new HttpClient())
                {
                    string responseJson = client.GetStringAsync(url).Result;
                    dynamic result = JsonConvert.DeserializeObject(responseJson);

                    var rows = result.COOKRCP01.row;

                    foreach (var row in rows)
                    {
                        string parts = row.RCP_PARTS_DTLS?.ToString() ?? "";

                        // 재료에 검색어 포함된 것만
                        if (parts.Contains(ingredient) ||
                            row.RCP_NM.ToString().Contains(ingredient))
                        {
                            recipes.Add(new Recipe
                            {
                                Name = row.RCP_NM?.ToString() ?? "",
                                Parts = parts,
                                ImageUrl = row.ATT_FILE_NO_MAIN?.ToString() ?? "",
                                Manual01 = row.MANUAL01?.ToString() ?? "",
                                Manual02 = row.MANUAL02?.ToString() ?? "",
                                Manual03 = row.MANUAL03?.ToString() ?? "",
                                Manual04 = row.MANUAL04?.ToString() ?? "",
                                Manual05 = row.MANUAL05?.ToString() ?? "",
                                Manual06 = row.MANUAL06?.ToString() ?? "",
                                Calorie = row.INFO_ENG?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("레시피 검색 오류: " + ex.Message);
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
        public string Calorie { get; set; }

        // 조리순서 합치기
        public string FullManual
        {
            get
            {
                string manual = "";
                if (!string.IsNullOrEmpty(Manual01)) manual += "1. " + Manual01 + "\n\n";
                if (!string.IsNullOrEmpty(Manual02)) manual += "2. " + Manual02 + "\n\n";
                if (!string.IsNullOrEmpty(Manual03)) manual += "3. " + Manual03 + "\n\n";
                if (!string.IsNullOrEmpty(Manual04)) manual += "4. " + Manual04 + "\n\n";
                if (!string.IsNullOrEmpty(Manual05)) manual += "5. " + Manual05 + "\n\n";
                if (!string.IsNullOrEmpty(Manual06)) manual += "6. " + Manual06 + "\n\n";
                return manual.Trim();
            }
        }
    }
}