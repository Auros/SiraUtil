using SiraUtil.Attributes;
using SiraUtil.Logging;
using SiraUtil.Submissions;
using SiraUtil.Zenject;
using Zenject;

namespace SiraUtil.Suite.Tests.SubmissionTest
{
    [Bind(Location.StandardPlayer)]
    internal class DisableSubmission : IInitializable
    {
        private readonly SiraLog _siraLog;
        private readonly Submission _submission;

        public DisableSubmission(SiraLog siraLog, Submission submission)
        {
            _siraLog = siraLog;
            _submission = submission;
        }

        public void Initialize()
        {
            _submission.DisableScoreSubmission("SiraUtil", "Testing");
        }
    }
}