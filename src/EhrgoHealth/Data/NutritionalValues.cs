using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhrgoHealth.Data
{
    public class NutritionalValues
    {
        public int Id { get; set; }
        public float Calories { get; set; }
        public float Carbs { get; set; }
        public float Fat { get; set; }
        public float Fiber { get; set; }
        public float Protein { get; set; }
        public float Sodium { get; set; }
    }
}