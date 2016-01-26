using System;
using System.Collections.Generic;
using System.Data;
using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.Core;
using ServiceStack.OrmLite;

namespace DCS.ServerRuntime.Entities
{
    public class Commits : EntitiesBase<CommitEntity, string>
    {
        public Commits(IDbConnection db) : base(db)
        {
        }

        public void Insert(CommitEntity commit)
        {
            if (commit.CommittedAt.IsDefault())
            {
                commit.CommittedAt = DateTime.UtcNow;
            }
            
            Db.Insert(commit);
        }

        public void UpdateResult(
            string checkinId, 
            AssessmentOutcome outcome, 
            string outcomeDetail, 
            TestOutputFormat testOutputFormat,
            string testOutput,
            string buildLog)
        {
            Db.UpdateOnly(
                new CommitEntity() { 
                    Outcome = outcome, 
                    OutcomeDetail = outcomeDetail, 
                    TestOutputFormat = testOutputFormat,
                    TestOutput = testOutput,
                    BuildLog = buildLog, 
                    ResultsUpdatedAt = DateTime.UtcNow },
                p => new { p.Outcome, p.OutcomeDetail, p.BuildLog, p.TestOutputFormat, p.TestOutput, p.ResultsUpdatedAt },
                p => p.Id == checkinId);
        }

        public IReadOnlyCollection<CommitEntity> ForUser(Guid userId)
        {
            return Db.Select<CommitEntity>(c => c.UserId == userId);
        }
    }
}