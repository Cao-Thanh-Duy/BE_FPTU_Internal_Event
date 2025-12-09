using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Repositories
{
    public class SpeakerRepository : ISpeakerRepository
    {
        private readonly ApplicationDbContext _context;
        public SpeakerRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public Speaker? AddSpeaker(Speaker speaker)
        {
            _context.Add(speaker);
            return speaker;
        }

        public bool DeleteSpeaker(int speakerId)
        {
            var loadSpeaker = _context.Speakers.Find(speakerId);
            if (loadSpeaker != null)
            {
                _context.Speakers.Remove(loadSpeaker);
                return true;
            }
            else return false;
        }

        public void DeleteSpeakerEvents(int speakerId)
        {
            // Xóa tất cả SpeakerEvent liên quan đến Speaker này
            var speakerEvents = _context.SpeakerEvents
                .Where(se => se.SpeakerId == speakerId)
                .ToList();

            if (speakerEvents.Any())
            {
                _context.SpeakerEvents.RemoveRange(speakerEvents);
            }
        }

        public List<Speaker> GetAllSpeaker()
        {
            return _context.Speakers.ToList();
        }

        public Speaker? GetSpeakerById(int speakerId)
        {
            return _context.Speakers.FirstOrDefault(s => s.SpeakerId == speakerId);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
