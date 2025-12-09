using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using Backend_FPTU_Internal_Event.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Services
{
    
    public class SlotService : ISlotService
    {
        private readonly ISlotRepository _slotRepository;
        public SlotService(ISlotRepository slotRepository)
        {
            _slotRepository = slotRepository;
        }

        public SlotDTO? CreateSlot(CreateSlotRequest request)
        {
            var newSlot = new Slot
            {
                SlotName = request.SlotName,
                StartTime = request.StartTime,
                EndtTime = request.EndTime,

            };

            _slotRepository.AddSlot(newSlot);
            _slotRepository.SaveChanges();

            return new SlotDTO
            {
               SlotName = newSlot.SlotName,
               StartTime = newSlot.StartTime,
               EndTime = newSlot.EndtTime
            };
        }

        public List<SlotDTO> GetAllSlot()
        {
            List<SlotDTO> listSlotDTO = new();
            var listSlot = _slotRepository.GetAllSlot();
            foreach (var slot in listSlot)
            {
                var slotDTO = new SlotDTO()
                {
                    SlotId = slot.SlotId,
                    SlotName = slot.SlotName,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndtTime
                };
                listSlotDTO.Add(slotDTO);
            }
            return listSlotDTO;
        }

        public SlotDTO? GetSlotById(int slotId)
        {
            var slot = _slotRepository.GetSlotById(slotId);
            if (slot == null) return null;

            return new SlotDTO
            {
                SlotId = slotId,
                SlotName = slot.SlotName,
                StartTime = slot.StartTime,
                EndTime = slot.EndtTime

            };
        }
    }
}
