using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamScore.Repository
{
    public class ScoreRepository
    {
        private readonly ExamScoreContext _dbContext;

        public ScoreRepository(ExamScoreContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<Score> GetAllScores()
        {
            return _dbContext.Scores.ToList();
        }

        public Score GetScoreById(int scoreId)
        {
            return _dbContext.Scores.Find(scoreId);
        }

        public void AddScore(Score score)
        {
            if (score == null)
            {
                throw new ArgumentNullException(nameof(score));
            }

            _dbContext.Scores.Add(score);
            _dbContext.SaveChanges();
        }

        public void UpdateScore(Score updatedScore)
        {
            if (updatedScore == null)
            {
                throw new ArgumentNullException(nameof(updatedScore));
            }

            var existingScore = _dbContext.Scores.Find(updatedScore.Id);

            if (existingScore != null)
            {
                existingScore.StudentId = updatedScore.StudentId;
                existingScore.SubjectId = updatedScore.SubjectId;
                existingScore.score = updatedScore.score;

                _dbContext.SaveChanges();
            }
        }

        public void DeleteScore(int scoreId)
        {
            var scoreToDelete = _dbContext.Scores.Find(scoreId);

            if (scoreToDelete != null)
            {
                _dbContext.Scores.Remove(scoreToDelete);
                _dbContext.SaveChanges();
            }
        }
    }
}
