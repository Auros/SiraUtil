using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SiraUtil.Submission
{
    /// <summary>
    /// A ticket which contains details for a score submission disable event.
    /// </summary>
    public sealed class Ticket
    {
        internal string Source { get; }
        internal Assembly Assembly { get; }
        private readonly HashSet<string> _reasons = new();
    
        internal Ticket(string source, Assembly assembly)
        {
            Source = source;
            Assembly = assembly;
        }

        /// <summary>
        /// Adds a reason to this.
        /// </summary>
        /// <param name="reason"></param>
        public void AddReason(string reason)
        {
            if (!string.IsNullOrWhiteSpace(reason))
                _reasons.Add(reason);
        }

        /// <summary>
        /// All the reasons as to why this ticket disabled score submission.
        /// </summary>
        /// <returns></returns>
        public string[] Reasons()
            => _reasons.ToArray();

        /// <summary>
        /// Copies a ticket.
        /// </summary>
        /// <returns></returns>
        public Ticket Copy()
        {
            var ticket = new Ticket(Source, Assembly);
            for (int i = 0; i < _reasons.Count(); i++)
                ticket.AddReason(_reasons.ElementAt(i));
            return ticket;
        }
    }
}