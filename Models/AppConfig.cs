using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ExFood.Models
{
    public static class AppConfig
    {
        private static IConfiguration _config;

        static AppConfig()
        {
            string configPath = FindConfigPath();

            _config = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }

        // ✅ appsettings.json 찾기
        private static string FindConfigPath()
        {
            string[] searchPaths = new string[]
            {
                AppDomain.CurrentDomain.BaseDirectory,

                Path.GetFullPath(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "..", "..")),

                Path.GetFullPath(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..")),

                Directory.GetCurrentDirectory()
            };

            foreach (string path in searchPaths)
            {
                string fullPath = Path.Combine(path, "appsettings.json");
                if (File.Exists(fullPath))
                    return path;
            }

            // 못 찾으면 경로 알려주기
            System.Windows.Forms.MessageBox.Show(
                "appsettings.json 을 찾을 수 없어요!\n\n" +
                "아래 경로 중 하나에 파일을 만들어주세요:\n" +
                searchPaths[0],
                "설정 파일 오류"
            );

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        // ── Google Vision API 키 ──
        public static string GoogleVisionKey
        {
            get { return _config?["ApiKeys:GoogleVision"] ?? ""; }
        }

        // ── 식품안전처 API 키 ──
        public static string FoodSafetyKey
        {
            get { return _config?["ApiKeys:FoodSafety"] ?? ""; }
        }

        // ── DB 연결 문자열 ──
        public static string ConnectionString
        {
            get
            {
                return string.Format(
                    "Host={0};Port={1};Username={2};Password={3};Database={4};SSL Mode=Require;Trust Server Certificate=true;",
                    _config["Database:Server"],
                    _config["Database:Port"],
                    _config["Database:User"],
                    _config["Database:Password"],
                    _config["Database:Name"]
                );
            }
        }
    }
}