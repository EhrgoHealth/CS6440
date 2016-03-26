using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhrgoHealth.Data
{
    public class FoodLog
    {
        public long Id { get; set; }
        public DateTime LoggedDate { get; set; }
        public string FoodName { get; set; }
        public string FoodBrand { get; set; }

        // 1 to zero or 1 with FoodLog
        //[System.ComponentModel.DataAnnotations.]
        public virtual NutritionalValues NutritionalValues { get; set; }

        public override int GetHashCode()
        {
            return new { FoodName, FoodBrand, LoggedDate }.GetHashCode();
        }
    }
}