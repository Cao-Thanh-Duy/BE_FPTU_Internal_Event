using Backend_FPTU_Internal_Event.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Interfaces
{
    public interface ISpeakerService
    {
        SpeakerDTO? CreateSpeaker(CreateSpeakerRequest request);
        SpeakerDTO? GetSpeakerById(int speakerId);
        List<SpeakerDTO> GetAllSpeaker();

        bool DeleteSpeaker(int speakerId);
    }
}
