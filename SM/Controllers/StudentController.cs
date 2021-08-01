using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SM.Data;
using SM.Models;
using SM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Controllers
{
    public class StudentController : Controller
    {
        #region Fields

        private readonly DataContext _db;

        private IEnumerable<Student> _students;

        public StudentController(DataContext db)
        {
            _db = db;
            _students = _db.Students;
        }

        #endregion Fields

        #region Methods (public)

        public IActionResult Index()
        {
            IEnumerable<Student> students = _students?.OrderBy(s => s.Name);
            return View(students);
        }

        // GET - Insert student
        public IActionResult Insert()
        {
            StudentVM studentVM = new StudentVM()
            {
                Student = new Student{Id = GetNextId(), Class = 1, DateOfBirth = DateTime.Now.Date }
            };

             return View(studentVM);          
        }

        // POST - Insert student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insert(StudentVM studentVM)
        {
            if (ModelState.IsValid)
            {
                // Validation
                if (!IsClassValid(studentVM.Student.Class))
                {
                    ModelState.AddModelError("", "Invalid class value");
                    return View();
                }

                if (!IsNameValid(studentVM))
                {
                    ModelState.AddModelError("", "Invalid name / already exists");
                    return View();
                }

                // Create new student
                try
                {
                    _db.Students.Add(studentVM.Student);

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ModelState.AddModelError("", "Student cannot be created");
                    return View();
                }  
            }
            return View(studentVM);
        }

        // GET - Update student
        public IActionResult Update(int? id)
        {
            StudentVM studentVM = new StudentVM()
            {
                Student = new Student()
            };

            // Validation
            if (!IsStudentExist(id) || id == null || id == 0)
            {
                ModelState.AddModelError("", "student cannot be found");
                return View();
            }
            else
            {
                // Read student by id
                studentVM.Student = _students.Single(s => s.Id == id);
                if (studentVM.Student == null)
                {
                    ModelState.AddModelError("", "student cannot be found");
                    return View();
                }
                return View(studentVM);
            }
        }

        // POST - Update student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(StudentVM studentVM)
        {
            if (ModelState.IsValid)
            {
                // Validation
                if (!IsNameValid(studentVM))
                {
                    ModelState.AddModelError("", "Invalid name / already exists");
                    return View();
                }

                if (!IsClassValid(studentVM.Student.Class))
                {
                    ModelState.AddModelError("", "Invalid class value");
                    return View();
                }

                if (!IsStudentExist(studentVM.Student.Id))
                {
                    ModelState.AddModelError("", "student cannot be found");
                    return View();
                }
                else
                {                  
                    try
                    {
                        // Read student by id
                        var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.Id == studentVM.Student.Id);

                        student.Name = studentVM.Student.Name;
                        student.Class = studentVM.Student.Class;
                        student.DateOfBirth = studentVM.Student.DateOfBirth;
                        student.Phone = studentVM.Student.Phone;

                        // Update student
                        _db.Students.Update(studentVM.Student);
                        _db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        ModelState.AddModelError("", "student cannot be updated");
                        return View();
                    }
                }      
            }
            return View(studentVM);
        }

        // GET - Delete student
        public IActionResult Delete(int? id)
        {
            // Validation
            if (id == null || id == 0)
            {
                ModelState.AddModelError("", "student cannot be found");
                return View();
            }

            // Read student by id
            var student = _db.Students.Find(id);

            if (student == null)
            {
                ModelState.AddModelError("", "student cannot be found");
                return View();
            }

            return View(student);
        }

        // POST - Delete student
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            // Read student by id
            var student = _db.Students.Find(id);

            // Validation
            if (student == null)
            {
                ModelState.AddModelError("", "student cannot be found");
                return View();
            }

            // Read grades of the student
            var grades = GetGrades(student);

            if (grades.Count > 0)
            {
                //Delete grades of the student
                _db.Grades.RemoveRange(grades);
            }

            try
            {
                _db.Students.Remove(student);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "student cannot be deleted");
                return View();
            }
            
        }

        #endregion Methods (public)

        #region Methods (private)

        // Student validation by nae
        private bool IsNameValid(StudentVM studentVM)
        {
            if (_students.Any(s => s.Name == studentVM.Student.Name && s.Id != studentVM.Student.Id))
            {
                return false;
            }

            return true;
        }

        // Student validation by id
        private bool IsStudentExist(int? id)
        {
            if (!_students.Any(s => s.Id == id))
            {
                return false;
            }

            return true;
        }

        // Get the next student id
        private int GetNextId()
        {
            return _students.Aggregate((x, y) => x.Id > y.Id ? x : y).Id + 1;
        }

        // Class validation by value
        private bool IsClassValid(int sClass)
        {
            return sClass > 0;
        }

        // Read Grades of the student
        private List<Grade> GetGrades(Student student)
        {
            return _db.Grades.Where(g => g.Name == student.Name).ToList();
        }

        #endregion Methods (private)
    }
}
