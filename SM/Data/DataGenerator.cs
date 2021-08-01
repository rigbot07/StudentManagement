using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SM.Data;
using SM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM
{
    public class DataGenerator
    {
        #region Consts

        private const int COUNT_SAMPLE_STUDENT = 5;
        private const int COUNT_SAMPLE_GRADE = 5;
        private const int GRADE_FAIL = 1;
        private const int GRADE_EXCELLENT = 5;

        #endregion Consts

        #region Methods (public)

        public static void Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                using (var context = new Data.DataContext(
                serviceProvider.GetRequiredService<DbContextOptions<Data.DataContext>>()))
                {
                    // Create sample user
                    if (!context.Users.Any())
                    {
                        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                        context.Database.EnsureCreated();
                        var user = new User
                        {
                            UserName = "admin",
                            Name = "admin",
                            Password = "edutest2021"
                        };

                        userManager.CreateAsync(user, user.Password);
                    }


                    // Look for any students
                    if (context.Students.Any())
                    {
                        return;   // Data was already seeded
                    }

                    // Create sample students
                    var students = new List<Student>();

                    for (int i = 1; i < COUNT_SAMPLE_STUDENT + 1; i++)
                    {
                        var createdStudent = CreateStudent(i, "Test Student " + i, 1, DateTime.Parse("1992-09-12"), "+36307777777");

                        if (createdStudent != null)
                        {
                            students.Add(createdStudent);
                        }
                    }

                    if (students != null && students.Count > 0)
                    {
                        context.Students.AddRange(students);
                    }

                    // Create sample grades
                    var grades = new List<Grade>();
                    int gradeId = 1;
                    Random random = new Random();

                    for (int i = 1; i < COUNT_SAMPLE_STUDENT + 1; i++)
                    {
                        // More than one grades pro student
                        for (int j = 1; j < COUNT_SAMPLE_GRADE + 1; j++)
                        {
                            // Random value of the grade
                            int randomGrade = random.Next(GRADE_FAIL, GRADE_EXCELLENT + 1);
                            var createdGrade = CreateGrade(gradeId, "Test Student " + i, randomGrade, students);

                            if (createdGrade != null)
                            {
                                grades.Add(createdGrade);
                            }

                            gradeId++;
                        }
                    }

                    if (grades != null && grades.Count > 0)
                    {
                        context.Grades.AddRange(grades);
                    }

                    context.SaveChanges();
                }
            }
            catch { }
        }

        #endregion Methods (public)

        #region Methods (private)

        // Create a new student
        private static Student CreateStudent(int id, string name, int sClass, DateTime dateOfBirth, string phone)
        {
            return new Student
            {
                Id = id,
                Name = name,
                Class = sClass,
                DateOfBirth = dateOfBirth.Date,
                Phone = phone
            };
        }

        // Create a new grade
        private static Grade CreateGrade(int id, string name, int value, List<Student> students)
        {
            if (!students.Any(s => s.Name == name))
            {
                return null;
            }

            return new Grade
            {
                Id = id,
                Name = name,
                Value = value
            };
        }

        #endregion Methods (private)
    }
}
