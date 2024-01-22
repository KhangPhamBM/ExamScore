using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamScore.Repository
{
    public class SubjectRepository
    {
        private readonly ExamScoreContext _dbContext;

        public SubjectRepository(ExamScoreContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<Subject> GetAllSubjects()
        {
            return _dbContext.Subjects.ToList();
        }

        public Subject GetSubjectById(int subjectId)
        {
            return _dbContext.Subjects.Find(subjectId);
        }

        public void AddSubject(Subject subject)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            _dbContext.Subjects.Add(subject);
            _dbContext.SaveChanges();
        }

        public void UpdateSubject(Subject updatedSubject)
        {
            if (updatedSubject == null)
            {
                throw new ArgumentNullException(nameof(updatedSubject));
            }

            var existingSubject = _dbContext.Subjects.Find(updatedSubject.Id);

            if (existingSubject != null)
            {
                existingSubject.Code = updatedSubject.Code;
                existingSubject.Name = updatedSubject.Name;

                _dbContext.SaveChanges();
            }
        }

        public void DeleteSubject(int subjectId)
        {
            var subjectToDelete = _dbContext.Subjects.Find(subjectId);

            if (subjectToDelete != null)
            {
                _dbContext.Subjects.Remove(subjectToDelete);
                _dbContext.SaveChanges();
            }
        }
    }
}
