using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistRetriever.Models
{
    public enum DialogResultAction
    {
        Cancel,
        Submit
    }
    public abstract class DialogResponse
    {
        public DialogResultAction DialogResult { get; set; }
    }
}
