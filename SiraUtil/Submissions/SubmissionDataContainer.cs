using IPA.Loader;
using System;
using System.Reflection;

namespace SiraUtil.Submissions
{
    internal sealed class SubmissionDataContainer
    {
        private string _data = "";
        private PropertyInfo? _ssssdi;
        public bool Disabled { get; set; }

        internal void Set(bool disabled, Ticket[] tickets)
        {
            _data = "";
            Disabled = disabled;
            foreach (Ticket ticket in tickets)
            {
                _data += $"{ticket.Source}\n";
                foreach (string reason in ticket.Reasons())
                    _data += $"<size=80%><color=#999999>{reason}</color></size>\n";
            }
        }

        public string Read()
        {
            return _data;
        }

        internal void SSS(bool value)
        {
            if (_ssssdi is null)
            {
                PluginMetadata? scoreSaber = PluginManager.GetPluginFromId("ScoreSaber");
                if (scoreSaber is not null && scoreSaber.PluginType is not null)
                {
                    Type? type = scoreSaber.Assembly.GetType(scoreSaber.PluginType.FullName);
                    if (type != null)
                        _ssssdi = type.GetProperty("ScoreSubmission");
                }
            }

            _ssssdi?.SetValue(null, value);
        }
    }
}