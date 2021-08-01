using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Models
{
    public class Statistic
    {
        [Key]
        public string Name { get; set; }
        public double Average { get; set; }
        public int NumberOfUnsatisfactoryGrades { get; set; }
        public int BestGrade { get; set; }
    }
}
