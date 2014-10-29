using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SCBasics.AuditTrail.ExtensionMethods
{
    public static class StringExtensions
    {
        #region Cached Compiled Regular Expressions

        private static object _initLock = new object();
        
        private static Regex _escapeRegEx;
        private static Regex EscapeRegEx
        {
            get
            {
                if (_escapeRegEx == null)
                    lock (_initLock)
                        if (_escapeRegEx == null)
                            InitExpressions();

                return _escapeRegEx;
            }
        }

        private static Regex _selectorRegEx;
        private static Regex SelectorRegEx
        {
            get
            {
                if (_selectorRegEx == null)
                    lock (_initLock)
                        if (_selectorRegEx == null)
                            InitExpressions();

                return _selectorRegEx;
            }
        }

        private static void InitExpressions()
        {
            // init the lot at once: we're going to need them all
            _escapeRegEx = new Regex(@"([\[\^\$\.\|\?\*\+\(\)])", RegexOptions.Compiled);
            _selectorRegEx = new Regex(@"\{([0-9]+)\}");
        }

        #endregion

        public static object[] Unformat(this String input, string formatString)
        {
            // Escape special regular expression characters with a backslash
            string interim = EscapeRegEx.Replace(formatString, @"\$1");

            // Turn format string style {0} into regex style (?<C0>.+)
            // Note that this doesn't support better formatting yet: e.g. {0:d}
            interim = SelectorRegEx.Replace(interim, @"(?<C$1>.+)");

            // add start and end markers
            interim = String.Format(@"^{0}$", interim);

            // perform the match
            Regex regex = new Regex(interim);
            Match match = regex.Match(input);

            // loop from zero until we don't get a matched capture group
            List<object> output = new List<object>();
            int loop = 0;
            while (true)
            {
                // build a capture group name and check for it
                string captureName = String.Format("C{0}", loop++);
                Group capture = match.Groups[captureName];

                //  see if this capture was found
                if (capture == null || !capture.Success)
                    break;

                // add it to the output list
                output.Add(capture.Value);
            }

            return output.ToArray();
        }
    }
}
