using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Affinity
{
    internal class AffinityManager
    {
        private readonly HashSet<IAffinity> _affinities = [];

        public IEnumerable<IAffinity> Affinities => _affinities;

        public AffinityManager([Inject(Optional = true, Source = InjectSources.Local)] List<IAffinity> affinities)
        {
            foreach (var affinity in affinities)
            {
                _affinities.Add(affinity);
            }
        }

        public void Add(IAffinity affinity)
        {
            _affinities.Add(affinity);
        }
    }
}