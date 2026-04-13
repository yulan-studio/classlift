using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SequenceHelper
    {
        public static int GetSequenceNumber(string target, List<string> stringList)
        {
            if (string.IsNullOrEmpty(target) || stringList == null || !stringList.Any())
                return -1; // Return -1 if the target or list is invalid

            // Sort the list alphabetically
            var sortedList = stringList.OrderBy(s => s).ToList();

            // Find the index of the target string (case-sensitive by default)
            int index = sortedList.IndexOf(target);

            // Return the sequence number (index + 1) or -1 if not found
            return index >= 0 ? index + 1 : -1;
        }

    }
}
