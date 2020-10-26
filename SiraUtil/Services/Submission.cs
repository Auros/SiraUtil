using System;
using System.Linq;
using IPA.Utilities;
using System.Reflection;
using System.Collections.Generic;

namespace SiraUtil.Services
{
    public class Submission : IDisposable
    {
        private readonly List<Ticket> _tickets = new List<Ticket>();
        private readonly GameplayCoreSceneSetupData _gameplaycoreSceneSetupData;

        public Submission(GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _gameplaycoreSceneSetupData = gameplayCoreSceneSetupData;
        }

        public void Dispose()
        {
            if (_tickets.Count > 0)
            {
                _gameplaycoreSceneSetupData.SetField("practiceSettings", new PracticeSettings());
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
    }
}