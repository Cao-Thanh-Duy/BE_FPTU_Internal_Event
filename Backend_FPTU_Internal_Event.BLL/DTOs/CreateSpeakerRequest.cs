using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.DTOs
{
    public class CreateSpeakerRequest
    {
        public required string SpeakerName { get; set; }
        public required string SpeakerDecription { get; set; }
    }
}
