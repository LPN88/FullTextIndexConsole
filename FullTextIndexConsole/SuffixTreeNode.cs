using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextIndexConsole
{
    public class SuffixTreeNode
    {
        List<SuffixTreeNode> _children; 

        public List<SuffixTreeNode> Children { get { return _children; } }

        public SuffixTreeNode SuffixLink { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        public int SuffixIndex { get; set; }

        public SuffixTreeNode(int start,int end, SuffixTreeNode link)
        {
            _children = new List<SuffixTreeNode>();
            Start = start;
            End = end;
            SuffixLink = link;
        }
    }
}
