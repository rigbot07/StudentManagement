using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SM.Data;
using SM.Models;
using SM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Controllers
{
    public class GradeController : Controller
    {
        #region Fields

        private readonly DataContext _db;

        private IEnumerable<Grade> _grades;

        public GradeController(DataContext db)
        {
            _db = db;
            _grades = _db.Grades;
        }

        #endregion Fields

        #region Methods (public)

        public IActionResult Index()
        {
            // Read all grades & order by Name
            IEnumerable<Grade> grades = _grades?.OrderBy(s => s.Name);
            return View(grades);
        }

        // GET - Insert grade
        public IActionResult Insert()
        {
            GradeVM gradeVM = new GradeVM()
            {
                Grade = new Grade { Id = GetNextId(), Value = 1 }
            };

            return View(gradeVM);
        }

        // POST - Insert grade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insert(GradeVM gradeVM)
        {
            if (ModelState.IsValid)
            {
                // Validation
                if (!IsGradeValid(gradeVM.Grade.Value))
                {
                    ModelState.AddModelError("", "Invalid grade value");
                    return View();
                }

                if (!IsStudentExist(gradeVM.Grade.Name))
                {
                    ModelState.AddModelError("", "Student cannot be found");
                    return View();
                }

                // Create new grade
                try
                {
                    _db.Grades.Add(gradeVM.Grade);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("", "Grade cannot be created");
                    return View();
                }
                
                
            }
            return View(gradeVM);
        }

        // GET - Update grade
        public IActionResult Update(int? id)
        {
            GradeVM gradeVM = new GradeVM()
            {
                Grade = new Grade()
            };

            // Validation
            if (!IsGradeExist(id) || id == null || id == 0)
            {
                ModelState.AddModelError("", "Grade cannot be found");
                return View();
            }
            else
            {
                // Read student by id
                gradeVM.Grade = _grades?.Single(s => s.Id == id);
                if (gradeVM.Grade == null)
                {
                    ModelState.AddModelError("", "Grade cannot be found");
                    return View();
                }
                return View(gradeVM);
            }
        }


        // POST - Update grade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(GradeVM gradeVM)
        {
            if (ModelState.IsValid)
            {
                // Validation
                if (!IsGradeValid(gradeVM.Grade.Value))
                {
                    ModelState.AddModelError("", "Invalid grade value");
                    return View();
                }

                if (!IsGradeExist(gradeVM.Grade.Id) || !IsStudentExist(gradeVM.Grade.Name))
                {
                    ModelState.AddModelError("", "Grade / student cannot be found");
                    return View();
                }
                else
                { 
                    try
                    {
                        // Read grade by id
                        var grade = _db.Grades.AsNoTracking().FirstOrDefault(s => s.Id == gradeVM.Grade.Id);

                        // Update grade
                        grade.Name = gradeVM.Grade.Name;
                        grade.Value = gradeVM.Grade.Value;

                        _db.Grades.Update(gradeVM.Grade);

                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Grade / student cannot be found");
                        return View();
                    } 
                }
            }
            return View(gradeVM);
        }


        // GET - Delete grade
        public IActionResult Delete(int? id)
        {
            // Validation
            if (id == null || id == 0)
            {
                ModelState.AddModelError("", "Grade cannot be found");
                return View();
            }

            // Search grade by id
            var grade = _db.Grades.Find(id);

            if (grade == null)
            {
                ModelState.AddModelError("", "Grade cannot be found");
                return View();
            }

            return View(grade);
        }

        // POST - Delete grade
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            // Read grade by id
            var grade = _db.Grades.Find(id);
            
            // Validation
            if (grade == null)
            {
                ModelState.AddModelError("", "Grade cannot be found");
                return View();
            }

            // Delete grade
            try
            {
                _db.Grades.Remove(grade);
                _db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("", "Grade cannot be deleted");
                return View();
            }

            return RedirectToAction("Index");
        }

        #endregion Methods (public)

        #region Methods (private)

        // Student validation by name
        private bool IsStudentExist(string name)
        {
            try
            {
                if (_db.Students.Single(s => s.Name == name) == null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            

            return true;
        }

        // Grade validation by id 
        private bool IsGradeExist(int? id)
        {

            if (!_db.Grades.Any(s => s.Id == id))
            {
                return false;
            }

            return true;
        }

        // Grade validation by value
        private bool IsGradeValid(int value)
        {
            return (value > 0 && value < 6);
        }

        // Get the next grade id
        private int GetNextId()
        {
            return _grades.Aggregate((x, y) => x.Id > y.Id ? x : y).Id + 1;
        }

        #endregion Methods (private)
    }
}
