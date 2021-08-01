using Microsoft.AspNetCore.Mvc;
using SM.Data;
using SM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Controllers
{
    public class StatisticController : Controller
    {
        #region Fields

        private readonly DataContext _db;
        private const int GRADE_FAIL = 1; 
        public StatisticController(DataContext db)
        {
            _db = db;
        }

        #endregion Fields

        #region Methods (public)

        public IActionResult Index()
        {
            IEnumerable<Grade> grades = _db.Grades;

            IEnumerable<Statistic> statistics;

            // Read statistic
            // **************
            //  - Name
            //  - Average
            //  - Number of unsatisfactory grades
            //  - best grade
            // order by average
            statistics = (from g in grades
                              group g by g.Name into s
                              let average = s.Average(s => s.Value)
                              let numberOfUnsatisfactoryGrades = s.Count(s => s.Value == GRADE_FAIL)
                              let bestGrade = s.Max(s => s.Value)
                              orderby average ascending
                              select new Statistic { Name = s.Key, Average = average, NumberOfUnsatisfactoryGrades = numberOfUnsatisfactoryGrades, BestGrade = bestGrade }).ToList();


            return View(statistics);
        }

        #endregion Methods (public)
    }
}
