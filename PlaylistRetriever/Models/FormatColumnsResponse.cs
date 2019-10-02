using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistRetriever.Models
{
    public class FormatColumnsResponse : DialogResponse
    {
        public List<PlaylistWriter.PlaylistColumn> Columns { get; set; }
    }
}
