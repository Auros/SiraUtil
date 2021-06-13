using SiraUtil.Submissions;
using Zenject;

namespace SiraUtil.Suite.Tests.SubmissionTest
{
    internal class DisableSubmission : IInitializable
    {
        private readonly Submission _submission;

        public DisableSubmission(Submission submission)
        {
            _submission = submission;
        }

        public void Initialize()
        {
            _submission.DisableScoreSubmission("SiraUtil", "Testing");
        }
    }
}