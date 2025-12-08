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
        List<Speaker> GetAllSpeaker();
        void SaveChanges();
    }
}
