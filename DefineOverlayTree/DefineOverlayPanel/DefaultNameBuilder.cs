using System;
using System.Collections.Generic;
using System.Linq;

namespace DefineOverlayTree.DefineOverlayPanel
{
    public class DefaultNameBuilder
    {
        public DefaultNameBuilder(string defaultNameBase)
        {
            DefaultNameBase = defaultNameBase ?? string.Empty;
        }

        public string DefaultNameBase { get; }

        public string FirstDefaultName => DefaultNameBase + 1.ToString();

        public string GetNextDefaultName(IList<string> names)
        {
            if (names == null || !names.Any())
            {
                return FirstDefaultName;
            }
            var trimmedDefaultNameBase = DefaultNameBase.Trim();
            var defaultNames = names.Where(name => name?.Length >= trimmedDefaultNameBase.Length
                && string.Equals(name.Substring(0, trimmedDefaultNameBase.Length), trimmedDefaultNameBase,
                    StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!defaultNames.Any())
            {
                return FirstDefaultName;
            }
            var indicesStrings = defaultNames.Select(name => name.Substring(trimmedDefaultNameBase.Length));
            var maximumIndex = 0;
            foreach (var indexString in indicesStrings)
            {
                if (int.TryParse(indexString, out var index))
                {
                    maximumIndex = Math.Max(maximumIndex, index);
                }
            }
            return DefaultNameBase + (maximumIndex + 1).ToString();
        }
    }
}
