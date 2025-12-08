using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Services
{

    public class SpeakerService : ISpeakerService
    {
        private readonly ISpeakerRepository _speakerRepository;

        public SpeakerService(ISpeakerRepository speakerRepository)
        {
            _speakerRepository = speakerRepository;
        }
        public SpeakerDTO? CreateSpeaker(CreateSpeakerRequest request)
        {
            var newSpeaker = new Speaker
            {
                SpeakerName = request.SpeakerName,
                SpeakerDescription = request.SpeakerDecription,
            };

            _speakerRepository.AddSpeaker(newSpeaker);
            _speakerRepository.SaveChanges();

            return new SpeakerDTO
            {
                SpeakerName = newSpeaker.SpeakerName,
                SpeakerDecription = newSpeaker.SpeakerDescription
            };
        }

        public List<SpeakerDTO> GetAllSpeaker()
        {
            List<SpeakerDTO> listSpeakerDTO = new();
            var listSpeaker = _speakerRepository.GetAllSpeaker();

            foreach(var speaker in listSpeaker)
            {
                var speakerDTO = new SpeakerDTO()
                {
                    SpeakerId = speaker.SpeakerId,
                    SpeakerName = speaker.SpeakerName,
                    SpeakerDecription = speaker.SpeakerDescription
                };
                listSpeakerDTO.Add(speakerDTO);
            }
            return listSpeakerDTO;
        }
    }
}
