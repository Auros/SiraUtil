using System;
using Zenject;
using System.Linq;
using IPA.Utilities;
using System.Reflection;
using System.Collections.Generic;

namespace SiraUtil.Services
{
    public class Submission : IDisposable
    {
        private readonly List<Ticket> _tickets = new List<Ticket>();
        private readonly RichPresenceManager _richPresenceManager;
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;
        private readonly StandardLevelScenesTransitionSetupDataSO _standardLevelScenesTransitionSetupDataSO;

        public Submission(RichPresenceManager richPresenceManager, GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _richPresenceManager = richPresenceManager;
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
            _standardLevelScenesTransitionSetupDataSO = _richPresenceManager.GetField<StandardLevelScenesTransitionSetupDataSO, RichPresenceManager>("_standardLevelScenesTransitionSetupData");

        }

        public void Dispose()
        {
            if (_tickets.Count > 0)
            {
                Plugin.Log.Info("disapf");
                _gameplayCoreSceneSetupData.SetField("practiceSettings", new PracticeSettings());
            }
        }

        public Ticket[] Tickets()
        {
            return _tickets.Select(x => x.Copy()).ToArray();
        }

        public Ticket DisableScoreSubmission(string source, string subsource = null)
        {
            var ticket = _tickets.FirstOrDefault(x => x.Source == source);
            if (ticket is null)
            {
                ticket = new Ticket(source, Assembly.GetCallingAssembly());
                _tickets.Add(ticket);
                ticket.AddReason(subsource);
            }
            else
            {
                ticket.AddReason(subsource);
            }
            return ticket;
        }

        public void Remove(Ticket ticket)
        {
            _tickets.Remove(ticket);
        }

        public void Remove(string source)
        {
            Remove(_tickets.FirstOrDefault(x => x.Source == source && x.Assembly == Assembly.GetCallingAssembly()));
        }

        public sealed class Ticket
        {
            private readonly HashSet<string> _reasons = new HashSet<string>();
            internal Assembly Assembly { get; }
            public string Source { get; }

            internal Ticket(string source, Assembly assembly)
            {
                Source = source;
                Assembly = assembly;
            }

            public void AddReason(string reason)
            {
                if (!string.IsNullOrWhiteSpace(reason))
                {
                    _reasons.Add(reason);
                }
            }

            public string[] Reasons()
            {
                return _reasons.ToArray();
            }

            public Ticket Copy()
            {
                var ticket = new Ticket(Source, Assembly);
                for (int i = 0; i < _reasons.Count(); i++)
                {
                    ticket.AddReason(_reasons.ElementAt(i));
                }
                return ticket;
            }
        }

        internal class SiraPrepareLevelCompletionResults : PrepareLevelCompletionResults
        {
            public SiraPrepareLevelCompletionResults()
            {
                PrepareLevelCompletionResults original = GetComponent<PrepareLevelCompletionResults>();
                foreach (FieldInfo info in original.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    info.SetValue(this, info.GetValue(original));
                }
                Destroy(original);
            }

            private Submission _submission;
            [Inject]
            public void Construct(Submission submission)
            {
                _submission = submission;
            }

            public override LevelCompletionResults FillLevelCompletionResults(LevelCompletionResults.LevelEndStateType levelEndStateType, LevelCompletionResults.LevelEndAction levelEndAction)
            {
                var results = base.FillLevelCompletionResults(levelEndStateType, levelEndAction);
                if (_submission._tickets.Count() > 0)
                {
                    results.SetField("rawScore", 0);
                }
                return results;
            }
        }
    }
}