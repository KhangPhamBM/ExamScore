using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamScore
{
    public class ExamScoreContext:DbContext
    {
         public DbSet<SchoolYear> SchoolYears { get; set; }
         public DbSet<Score> Scores { get; set; }
         public DbSet<Student> Students { get; set; }
         public DbSet<Subject> Subjects { get; set; }
         public DbSet<ScoreDraft> ScoreDrafts { get; set; }
         public DbSet<RecordModel> RecordModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=.;database=ExamScore;uid=sa;pwd=12345;TrustServerCertificate=True;MultipleActiveResultSets=True");
        }
    }
    public class SchoolYear
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ExamYear { get; set; }
        public int Status { get; set; } = 0;
    }
    public class Score
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        public int SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public double score { get; set; }
    }

    public class ScoreDraft
    {
        public int Id { get; set; }
        public string  StudentCode { get; set; }
        public int SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public double score { get; set; }
        public int YearId { get; set; }
        [ForeignKey("YearId")]
        public SchoolYear SchoolYear { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string? StudentCode { get; set; }
        public int SchoolYearId { get; set; }
        [ForeignKey("SchoolYearId")]
        public SchoolYear SchoolYear { get; set; }
        public int Status { get; set; } = 0;
    }
    public class Subject
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class RecordModel
    {
        [Key] 
        public int Id { get; set; }
        public string SBD { get; set; }
        public double? Toan { get; set; }
        public double? Van { get; set; }
        public double? Ly { get; set; }
        public double? Sinh { get; set; }
        public double? NgoaiNgu { get; set; }
        public string Year { get; set; }
        public double? Hoa { get; set; }
        public double? LichSu { get; set; }
        public double? DiaLy { get; set; }
        public double? GDCD { get; set; }
        public string MaTinh { get; set; }
    }


    public class RecordModelCSV
    {
        public string SBD { get; set; }
        public double? Toan { get; set; }
        public double? Van { get; set; }
        public double? Ly { get; set; }
        public double? Sinh { get; set; }
        public double? NgoaiNgu { get; set; }
        public string Year { get; set; }
        public double? Hoa { get; set; }
        public double? LichSu { get; set; }
        public double? DiaLy { get; set; }
        public double? GDCD { get; set; }
        public string MaTinh { get; set; }
    }
}
