using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SM.Controllers;
using SM.Data;
using SM.Models;
using System.Collections.Generic;
using System.Linq;

namespace SM.Test
{
    public class StudentTest
    {
        private DataContext _context;

        [SetUp]
        public void Setup()
        {
            // Database setup
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(databaseName: "TestDB").UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;

            _context = new DataContext(option);
            if (_context != null)
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();
            }

            // Database seed
            Seed();

        }

        [Test]
        public void Task_Get_Student()
        {
            IEnumerable<Student> students = _context.Students;

            Assert.AreEqual(3, students.Count());
            Assert.AreEqual("Test1", students.ElementAt(0).Name);
            Assert.AreEqual("Test2", students.ElementAt(1).Name);
            Assert.AreEqual("Test3", students.ElementAt(2).Name);              
        }

        [Test]
        public void Task_Insert_Student()
        {
            var student = new Student
            {
                Id = 4,
                Name = "Test4",
                Class = 1,
                DateOfBirth = System.DateTime.Now,
                Phone = "+36301111111"
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            IEnumerable<Student> students = _context.Students;

            Assert.AreEqual(4, students.Count());
            Assert.AreEqual("Test4", students.ElementAt(3).Name);
        }

        [Test]
        public void Task_Insert_Student_When_Already_Existing()
        {
            var student = new Student
            {
                Id = 3,
                Name = "Test3",
                Class = 1,
                DateOfBirth = System.DateTime.Now,
                Phone = "+36301111111"
            };

            try
            {
                _context.Students.Add(student);
                _context.SaveChanges();
            }
            catch { }

            IEnumerable<Student> students = _context.Students;
            
            Assert.AreEqual(3, students.Count());
        }

        [Test]
        public void Task_Insert_Student_Time()
        {
            for (int i = 4; i <= 1003; i++)
            {
                var student = new Student() {Id = i ,Name = "Test" + i, Class = 1, DateOfBirth = System.DateTime.Now, Phone = "+36301111111" };
                _context.Students.Add(student);
            }

            _context.SaveChanges();


            IEnumerable<Student> students = _context.Students;
            Assert.AreEqual(1003, students.Count());
 
            var singleStudent = students.Where(s => s.Id == 17).FirstOrDefault();
            if (singleStudent != null)
            {
                Assert.AreEqual("Test17", singleStudent.Name);
            }
        }

        private void Seed()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var student1 = new Student
            {
                Id = 1,
                Name = "Test1",
                Class = 1,
                DateOfBirth = System.DateTime.Now,
                Phone = "+36301111111"
            };

            var student2 = new Student
            {
                Id = 2,
                Name = "Test2",
                Class = 1,
                DateOfBirth = System.DateTime.Now,
                Phone = "+36301111111"
            };

            var student3 = new Student
            {
                Id = 3,
                Name = "Test3",
                Class = 1,
                DateOfBirth = System.DateTime.Now,
                Phone = "+36301111111"
            };


            _context.AddRange(student1, student2, student3);
            _context.SaveChanges();

        }

    }
}