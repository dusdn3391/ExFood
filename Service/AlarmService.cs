using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExFood.Models;


namespace ExFood.Services
{
    public static class AlarmService
    {
        // ── 임박 식재료 조회 (1~2일) ──
        public static List<FoodItem> GetExpiringItems(int days = 2)
        {
            var all = DbService.GetAllIngredients();
            var expiring = new List<FoodItem>();

            foreach (var item in all)
            {
                if (item.DaysLeft >= 0 && item.DaysLeft <= days)
                    expiring.Add(item);
            }

            return expiring;
        }

        // ── 만료된 식재료 조회 ──
        public static List<FoodItem> GetExpiredItems()
        {
            var all = DbService.GetAllIngredients();
            var expired = new List<FoodItem>();

            foreach (var item in all)
            {
                if (item.DaysLeft < 0)
                    expired.Add(item);
            }

            return expired;
        }
    }
}
