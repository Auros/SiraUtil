namespace SiraUtil.Submissions
{
    internal sealed class SubmissionDataContainer
    {
        private string _data = "";
        public bool Disabled { get; set; }

        internal void Set(bool disabled, Ticket[] tickets)
        {
            _data = "";
            Disabled = disabled;
            foreach (var ticket in tickets)
            {
                _data += $"{ticket.Source}\n";
                foreach (var reason in ticket.Reasons())
                    _data += $"<size=80%><color=#999999>{reason}</color></size>\n";
            }
        }

        public string Read()
        {
            return _data;
        }
    }
}