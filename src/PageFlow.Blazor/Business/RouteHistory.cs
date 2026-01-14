namespace PageFlow.Blazor.Business
{
    /// <summary>
    /// Represents a history of navigated routes, allowing tracking and retrieval of previously visited routes.
    /// </summary>
    /// <remarks>This class maintains a stack-like history of routes, where each entry consists of a route and
    /// its associated parameters. It provides functionality to record new routes, retrieve the previous route, and
    /// clear the history.</remarks>
    public sealed class RouteHistory : IRouteHistory
    {
        private readonly List<(PageFlowInfo Route, Dictionary<string, string>? Parameters)> _history = new();
        /// <summary>
        /// Gets a value indicating whether the user can navigate back in the history.
        /// </summary>
        public bool CanGoBack => _history.Count > 1;
        public int EntrySize => _history.Count;
        /// <summary>
        /// Records the specified route and its associated parameters into the navigation history.
        /// </summary>
        /// <remarks>The recorded route and parameters are added to the internal history for later
        /// retrieval or analysis.</remarks>
        /// <param name="route">The route information to be recorded. This parameter cannot be <see langword="null"/>.</param>
        /// <param name="parameters">An optional dictionary of parameters associated with the route. Can be <see langword="null"/> if no
        /// parameters are provided.</param>
        public void Record(PageFlowInfo route, Dictionary<string, string>? parameters)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }
            if (_history.Count > 0)
            {
                var (lastRoute, lastParams) = _history[^1];

                if (lastRoute.Equals(route) && AreParamsEqual(lastParams, parameters))
                    return;
            }
            _history.Add((route, parameters));
        }
        /// <summary>
        /// Retrieves the previous route and its associated parameters from the navigation history.
        /// </summary>
        /// <remarks>This method removes the current entry from the navigation history and returns the
        /// previous entry. If there is no previous entry, the method returns <see langword="null"/>.</remarks>
        /// <returns>A tuple containing the previous route and its associated parameters, or <see langword="null"/> if there is
        /// no previous entry in the navigation history.</returns>
        public (PageFlowInfo Route, Dictionary<string, string>? Parameters)? GetPrevious()
        {
            if (!CanGoBack)
            {
                return null;
            }

            // Remove current entry and return the previous one
            _history.RemoveAt(_history.Count - 1);
            var previous = _history[^1];
            return previous;
        }
        /// <summary>
        /// Clears all entries from the history.
        /// </summary>
        /// <remarks>After calling this method, the history will be empty. This operation does not throw
        /// exceptions.</remarks>
        public void Clear() => _history.Clear();
        /// <summary>
        /// Compares two dictionaries of parameters for equality.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool AreParamsEqual(Dictionary<string, string>? a, Dictionary<string, string>? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            if (a.Count != b.Count) return false;

            foreach (var kv in a)
            {
                if (!b.TryGetValue(kv.Key, out var value) || value != kv.Value)
                    return false;
            }
            return true;
        }
    }
}
