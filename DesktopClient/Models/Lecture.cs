using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopClient.Models
{
    public class Lecture
    {
        public string Id { get; set; }
        public string HostId { get; set; }
        public string CourseId { get; set; }
        public string DateOf { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int IsComplete { get; set; }
        public string Transcript { get; set; }
    }
}
