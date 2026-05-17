using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ExFood.Services
{
    public static class OcrServices
    {
        // ── Google Vision OCR 실행 ──
        public static string RunOCR(string path, string apiKey)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(path);
                string base64Image = Convert.ToBase64String(imageBytes);

                var requestBody = new
                {
                    requests = new[]
                    {
                        new
                        {
                            image = new { content = base64Image },
                            features = new[] { new { type = "TEXT_DETECTION" } },
                            imageContext = new
                            {
                                languageHints = new[] { "ko", "en" }
                            }
                        }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);

                using (var client = new HttpClient())
                {
                    string url = "https://vision.googleapis.com/v1/images:annotate?key=" + apiKey;
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).Result;
                    string responseJson = response.Content.ReadAsStringAsync().Result;

                    dynamic result = JsonConvert.DeserializeObject(responseJson);

                    try
                    {
                        return result.responses[0].fullTextAnnotation.text;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OCR 오류: " + ex.Message);
            }
        }

        // ── OCR 결과 파싱 ──
        public static (string name, string date) ParseOCRResult(string text, bool isForeignMode)
        {
            string name = "";
            string date = "";

            // 날짜 찾기
            string datePattern = @"(\d{2,4})[.\-/](\d{2})[.\-/](\d{2,4})";
            var matches = Regex.Matches(text, datePattern);

            if (matches.Count > 0)
            {
                DateTime maxDate = DateTime.MinValue;

                foreach (Match m in matches)
                {
                    try
                    {
                        string first = m.Groups[1].Value;
                        string second = m.Groups[2].Value;
                        string third = m.Groups[3].Value;

                        int year, month, day;

                        if (isForeignMode)
                        {
                            // 외국 형식: 일/월/년
                            day = int.Parse(first);
                            month = int.Parse(second);
                            year = third.Length == 2
                                ? int.Parse("20" + third)
                                : int.Parse(third);
                        }
                        else
                        {
                            // 한국 형식: 년/월/일
                            year = first.Length == 2
                                ? int.Parse("20" + first)
                                : int.Parse(first);
                            month = int.Parse(second);
                            day = third.Length == 4
                                ? int.Parse(third.Substring(2))
                                : int.Parse(third);
                        }

                        DateTime parsed = new DateTime(year, month, day);

                        if (parsed > maxDate)
                            maxDate = parsed;
                    }
                    catch { }
                }

                if (maxDate != DateTime.MinValue)
                    date = maxDate.ToString("yyyy-MM-dd");
            }

            // 이름 찾기
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (trimmed.Length < 2) continue;
                if (Regex.IsMatch(trimmed, @"\d{2,4}[.\-/]\d{2}[.\-/]\d{2,4}")) continue;
                if (Regex.IsMatch(trimmed, @"\d{2}:\d{2}")) continue;
                if (Regex.IsMatch(trimmed, @"^[\d\s\-\.\:\/\\]+$")) continue;
                if (Regex.IsMatch(trimmed, @"^[A-Z]{2}\s+[A-Z0-9]+")) continue;

                name = trimmed;
                break;
            }

            return (name, date);
        }
    }
}