using System;
using System.Collections.Generic;
using System.Linq;

namespace DR.Marvin.Dashboard.Models
{
    public class Check
    {
        public DateTime TimeStamp { get { return DateTime.UtcNow;} }
        public TimeSpan Duration { get; set; }
        public IList<Group> Groups { get; set; }

        public Check()
        {
            Groups = new List<Group>();
        }
        public bool AllPassed
        {
            get { return Groups.All(g => g.CheckPassed); }
        }

    }
    public class Group
    {
        public string Name { get; set; }
        public IList<Node> Nodes { get; set; }

        public Group()
        {
            Nodes = new List<Node>();
        }
        public bool CheckPassed
        {
            get
            {
                var first = Nodes.First().Version;
                return Nodes.All(n => n.CheckPassed.GetValueOrDefault(true)) &&
                       Nodes.Skip(1).All(n => n.Version.Equals(first));
            }
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public bool? CheckPassed { get; set; }
        public Uri CheckUri { get; set; }
        public Exception Exception { get; set; }

        public TimeSpan Time { get; set; }
    }

    public class Version
    {
        public DateTime BuildTime { get; set; }
        public string GitHash { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Version;
            return other != null && Equals(other);
        }

        protected bool Equals(Version other)
        {
            return BuildTime.Equals(other.BuildTime) && string.Equals(GitHash, other.GitHash);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (BuildTime.GetHashCode()*397) ^ (GitHash != null ? GitHash.GetHashCode() : 0);
            }
        }
    }
}