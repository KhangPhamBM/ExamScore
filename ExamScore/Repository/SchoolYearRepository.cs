using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamScore.Repository
{
    public class SchoolYearRepository
    {
        private readonly ExamScoreContext _dbContext;

        public SchoolYearRepository(ExamScoreContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<SchoolYear> GetAllSchoolYears()
        {
            return _dbContext.SchoolYears.ToList();
        }

        public SchoolYear GetSchoolYearById(int schoolYearId)
        {
            return _dbContext.SchoolYears.Find(schoolYearId);
        }

        public void AddSchoolYear(SchoolYear schoolYear)
        {
            if (schoolYear == null)
            {
                throw new ArgumentNullException(nameof(schoolYear));
            }

            _dbContext.SchoolYears.Add(schoolYear);
            _dbContext.SaveChanges();
        }

        public void UpdateSchoolYear(SchoolYear updatedSchoolYear)
        {
            if (updatedSchoolYear == null)
            {
                throw new ArgumentNullException(nameof(updatedSchoolYear));
            }

            var existingSchoolYear = _dbContext.SchoolYears.Find(updatedSchoolYear.Id);

            if (existingSchoolYear != null)
            {
                existingSchoolYear.Name = updatedSchoolYear.Name;
                existingSchoolYear.ExamYear = updatedSchoolYear.ExamYear;
                existingSchoolYear.Status = updatedSchoolYear.Status;

                _dbContext.SaveChanges();
            }
        }

        public void DeleteSchoolYear(int schoolYearId)
        {
            var schoolYearToDelete = _dbContext.SchoolYears.Find(schoolYearId);

            if (schoolYearToDelete != null)
            {
                _dbContext.SchoolYears.Remove(schoolYearToDelete);
                _dbContext.SaveChanges();
            }
        }
    }
}
