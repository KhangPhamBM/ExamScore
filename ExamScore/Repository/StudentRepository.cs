using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamScore.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class StudentRepository
    {
        private readonly ExamScoreContext _dbContext;

        public StudentRepository(ExamScoreContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _dbContext.Students.ToList();
        }

        public Student GetStudentById(int studentId)
        {
            return _dbContext.Students.Find(studentId);
        }

        public void AddStudent(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }

            _dbContext.Students.Add(student);
            _dbContext.SaveChanges();
        }

        public void UpdateStudent(Student updatedStudent)
        {
            if (updatedStudent == null)
            {
                throw new ArgumentNullException(nameof(updatedStudent));
            }

            var existingStudent = _dbContext.Students.Find(updatedStudent.Id);

            if (existingStudent != null)
            {
                existingStudent.StudentCode = updatedStudent.StudentCode;
                existingStudent.SchoolYearId = updatedStudent.SchoolYearId;
                existingStudent.Status = updatedStudent.Status;

                _dbContext.SaveChanges();
            }
        }

        public void DeleteStudent(int studentId)
        {
            var studentToDelete = _dbContext.Students.Find(studentId);

            if (studentToDelete != null)
            {
                _dbContext.Students.Remove(studentToDelete);
                _dbContext.SaveChanges();
            }
        }
    }
}
