using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExFood.Models
{
    public class FoodItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string StoragePlace { get; set; }
        public string ExpirationDate { get; set; }

        // 유통기한까지 남은 일수
        public int DaysLeft
        {
            get
            {
                if (DateTime.TryParse(ExpirationDate, out DateTime expiry))
                    return (expiry - DateTime.Today).Days;
                return 0;
            }
        }

        // 최대 유통기한 일수 (신선도 바용)
        public int MaxDays
        {
            get
            {
                if (Category.Contains("채소")) return 7;
                if (Category.Contains("생선")) return 2;
                if (Category.Contains("육류")) return 3;
                if (Category.Contains("과일")) return 5;
                if (Category.Contains("계란")) return 14;
                if (Category.Contains("조미료")) return 365;
                if (Category.Contains("곡류")) return 180;
                return 30; // 기본값
            }
        }
    }
}