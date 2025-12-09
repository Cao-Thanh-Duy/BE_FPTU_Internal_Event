using Backend_FPTU_Internal_Event.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Interface
{
    public interface ISpeakerRepository
    {
        Speaker? AddSpeaker(Speaker speaker);

        Speaker? GetSpeakerById(int speakerId);
        List<Speaker> GetAllSpeaker();
        void SaveChanges();

        void DeleteSpeakerEvents(int speakerId);
        bool DeleteSpeaker(int speakerId);
    }
}
