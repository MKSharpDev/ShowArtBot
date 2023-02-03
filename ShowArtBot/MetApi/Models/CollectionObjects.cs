using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowArtBot.Models
{
    // This model contains a listing of valid Object IDs available to use
    // Can be used in conjunction with all collection objects or objects found via search
    public class CollectionObjects
    {
        public int total { get; set; }
        public List<int>? objectIDs { get; set; }
    }
}
