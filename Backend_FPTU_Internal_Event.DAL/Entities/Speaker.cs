using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Entities
{
    public class Speaker
    {
        public int SpeakerId { get; set; } 
        public string SpeakerName { get; set; } = string.Empty;
        public string SpeakerDescription { get; set; } = string.Empty;
    }
}
