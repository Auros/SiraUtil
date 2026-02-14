using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SiraUtil.Submissions
{
    /// <summary>
    /// A service for disabling and enabling score submission.
    /// </summary>
    public sealed class Submission : IDisposable
    {
        private readonly List<Ticket> _tickets = [];
        private readonly SubmissionDataContainer _submissionDataContainer;
        internal bool Activated => _tickets.Count != 0;

        internal Submission(SubmissionDataContainer submissionDataContainer)
        {
            _submissionDataContainer = submissionDataContainer;
        }

        /// <summary>
        /// The dispose method.
        /// </summary>
        public void Dispose()
        {
            var disabled = _tickets.Count > 0;
            _submissionDataContainer.Set(disabled, [.. _tickets]);
        }

        /// <summary>
        /// Get all the currently active tickets.
        /// </summary>
        /// <returns>All the currently active tickets.</returns>
        public Ticket[] Tickets()
        {
            return [.. _tickets.Select(x => x.Copy())];
        }

        /// <summary>
        /// Disables score submission for the currently played level.
        /// </summary>
        /// <param name="source">The name of the entity that is disabling score submission.</param>
        /// <param name="subsource">A secondary source that is disabling score submission. Use this to be more specific about why submission is being disabled (ex. specific modifier)</param>
        /// <returns>A ticket which can be used to disable the disabling of score submission.</returns>
        public Ticket DisableScoreSubmission(string source, string? subsource = null)
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

        /// <summary>
        /// Reenables score submission for a ticket.
        /// </summary>
        /// <param name="ticket"></param>
        public void Remove(Ticket ticket)
        {
            _tickets.Remove(ticket);
        }

        /// <summary>
        /// Reenables score submission from a source.
        /// </summary>
        /// <param name="source"></param>
        public void Remove(string source)
        {
            Remove(_tickets.FirstOrDefault(x => x.Source == source && x.Assembly == Assembly.GetCallingAssembly()));
        }
    }
}