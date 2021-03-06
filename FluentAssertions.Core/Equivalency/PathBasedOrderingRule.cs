using System;
using System.Text.RegularExpressions;

namespace FluentAssertions.Equivalency
{
    /// <summary>
    /// Represents a rule for determining whether or not a certain collection within the object graph should be compared using
    /// strict ordering.
    /// </summary>
    internal class PathBasedOrderingRule : IOrderingRule
    {
        private readonly string path;

        public PathBasedOrderingRule(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Determines if ordering of the member refered to by the current <paramref name="subjectInfo"/> is relevant.
        /// </summary>
        public bool AppliesTo(ISubjectInfo subjectInfo)
        {
            string currentPropertyPath = subjectInfo.SelectedMemberPath;
            if (!ContainsIndexingQualifiers(path))
            {
                currentPropertyPath = RemoveInitialIndexQualifier(currentPropertyPath);
            }

            return currentPropertyPath.Equals(path, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool ContainsIndexingQualifiers(string path)
        {
            return path.Contains("[") && path.Contains("]");
        }

        private string RemoveInitialIndexQualifier(string sourcePath)
        {
            var indexQualifierRegex = new Regex(@"^\[\d+]\.");

            if (!indexQualifierRegex.IsMatch(path))
            {
                var match = indexQualifierRegex.Match(sourcePath);
                if (match.Success)
                {
                    sourcePath = sourcePath.Substring(match.Length);
                }
            }

            return sourcePath;
        }
    }
}