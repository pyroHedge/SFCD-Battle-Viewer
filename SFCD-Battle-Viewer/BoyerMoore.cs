using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroPatchViewer
{
    class BoyerMoore
    {
        public byte[,] Table { get; private set; }
        public byte?[] Pattern { get; }
        public byte[,] BadCharacterRuleTable;
        public byte[] GoodSuffixTable;

        public BoyerMoore(byte?[] pattern)
        {
            this.Pattern = pattern;
            MakeTable(Pattern);
        }


        /// <summary>
        /// Create and combine the Bad Character and the Good Suffix tables together.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool MakeTable(byte?[] pattern)
        {
            //Since bytes are used to build the tables, the pattern length cannot exceed byte size.
            if (pattern.Length == 0 || pattern.Length > 256)
            {
                return false;
            }

            //If the last byte of pattern is a wildcard, it will always match and causes problems for the Good Suffix Table. Therefore, remove any trailing wildcards.
            while (pattern.Last() == null)
            {
                Array.Resize(ref pattern, pattern.Length - 1);
            }

            //Build the tables.
            Table = new byte[pattern.Length, 256];
            BadCharacterRuleTable = makeBadCharacterRuleTable(pattern);
            GoodSuffixTable = makeGoodSuffixTable(pattern);

            for (int n1 = 0; n1 < pattern.Length; n1++)
            {
                for (int n2 = 0; n2 < 256; n2++)
                {
                    Table[n1, n2] = Math.Max(BadCharacterRuleTable[n1, n2], GoodSuffixTable[n1]);
                }
            }

            return true;
        }


        /// <summary>
        /// The values populating this table represent how far to jump ahead in the search.
        /// </summary>
        /// <param name="pattern"></param>
        private byte[,] makeBadCharacterRuleTable(byte?[] pattern)
        {
            byte[,] badCharacterRuleTable = new byte[pattern.Length, 256];

            //The case for n1 = 0 is skipped here because it will be handled when the tables are combined; if this is used independently then the n1 = 0 case must be added.
            for (int n1 = 1; n1 < pattern.Length; n1++)
            {
                for (int n2 = 0; n2 < 256; n2++)
                {
                    badCharacterRuleTable[n1, n2] = (byte)(n1 - simpleFindLast(new byte?[] { (byte?)n2 }, pattern, pattern.Length - n1));
                }
            }
            return badCharacterRuleTable;
        }


        /// <summary>
        /// The values populating this table represent how far to jump ahead in the search.
        /// </summary>
        /// <param name="pattern"></param>
        private byte[] makeGoodSuffixTable(byte?[] pattern)
        {
            byte[] goodSuffixTable = new byte[pattern.Length];

            for (int n = 0; n < goodSuffixTable.Length - 1; n++)
            {
                //Examples:
                //Length  n  findLast  Result
                //  3     3    2         1
                //  6     2    -1        3
                //  6     2    0         2
                //  6     2    1         1
                //  6     3    1         2
                //  6     4    1         3
                //  6     5    1         4
                //  6     5    2         3
                //  6     5    3         2
                //  7     5    3         2
                //  8     5    3         2
                goodSuffixTable[n] = (byte)(n - simpleFindLast(pattern.Skip(n).ToArray(), pattern, 1)); //n bounds the maximum shift
            }
            goodSuffixTable[goodSuffixTable.Length - 1] = 1; //nothing matches

            return goodSuffixTable;
        }



        /// <summary>
        /// Standard search for use in creating the Boyer Moore tables. Return the index of the location of patternToFind in dataToSearch. 
        /// </summary>
        /// <param name="patternToFind"></param>
        /// <param name="dataToSearch"></param>
        /// <param name="startLocationFromEnd"></param>
        /// <returns>Return the index of the location of patternToFind in dataToSearch (ex: searching for CDE in ABCDEFGH returns 2).</returns>
        public int simpleFindLast(byte?[] patternToFind, byte?[] dataToSearch, int startLocationFromEnd = 0)
        {
            int len = patternToFind.Length;
            for (int i = dataToSearch.Length - patternToFind.Length - startLocationFromEnd; i >= 0; i--)
            {
                int n = 0;
                for (; n < len; n++)
                {
                    if (patternToFind[n] != null && dataToSearch[i + n] != null && patternToFind[n] != dataToSearch[i + n])
                    {
                        break;
                    }
                }
                if (n == len)
                {
                    return i;
                }
            }
            return -1;
        }



        //Same as above, but starting with the beginning rather than end of the array.
        private int simpleFind(byte?[] patternToFind, byte[] dataToSearch, int startLocation = 0)
        {
            //Referenced https://stackoverflow.com/questions/4859023/find-an-array-byte-inside-another-array for speed optimizations.
            int len = patternToFind.Length;
            int limit = dataToSearch.Length - patternToFind.Length + 1;
            for (int i = startLocation; i < limit; i++)
            {
                int n = 0;
                for (; n < len; n++)
                {
                    if (patternToFind[n] != null && patternToFind[n] != dataToSearch[i + n])
                    {
                        break;
                    }
                }
                if (n == len)
                {
                    return i;
                }
            }
            return -1;
        }


    }
}
