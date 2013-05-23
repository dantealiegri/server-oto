using System;
using System.Text.RegularExpressions;

namespace OtoServer.DataStore
{
    public class VersionTuple : IComparable<VersionTuple>
    {
        public int major;
        public int minor;
        public int micro;
        public string extra;

        public VersionTuple(string as_string)
        {
            Match m = null;
            if ((m = Regex.Match(as_string, @"(\d+)\.(\d+)\.(\d+)([\.-]([^\s]+))?")) != null && m.Success)
            {
                major = Int32.Parse(m.Groups[1].Value);
                minor = Int32.Parse(m.Groups[2].Value);
                micro = Int32.Parse(m.Groups[3].Value);
                extra = m.Groups[5].Value;
            }
        }

        public VersionTuple(int Major, int Minor, int Micro, string Extra)
        {
            major = Major;
            minor = Minor;
            micro = Micro;
            extra = Extra;
        }

        public override string ToString()
        {
            int as_int;
            if (Int32.TryParse(extra, out as_int))
                return String.Format("{0}.{1}.{2}.{3}", major, minor, micro, extra);
            else
                if (extra.Length == 0)
                    return String.Format("{0}.{1}.{2}", major, minor, micro);
                else
                    return String.Format("{0}.{1}.{2}-{3}", major, minor, micro, extra);
        }

        #region Comparisons

        public static bool operator >(VersionTuple a, VersionTuple b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(VersionTuple a, VersionTuple b)
        {
            return a.CompareTo(b) < 0;
        }


        public int CompareTo(VersionTuple other)
        {
            Int32 this_int, that_int;
            //major
            this_int = major; that_int = other.major;
            if (this_int.CompareTo(that_int) != 0)
                return this_int.CompareTo(that_int);

            //minor
            this_int = minor; that_int = other.minor;
            if (this_int.CompareTo(that_int) != 0)
                return this_int.CompareTo(that_int);

            //micro
            this_int = micro; that_int = other.micro;
            if (this_int.CompareTo(that_int) != 0)
                return this_int.CompareTo(that_int);

            // we do not compare extra currently.

            return 0;
        }
        #endregion Comparisons

    }
}