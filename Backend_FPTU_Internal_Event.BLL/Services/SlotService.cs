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
            // Validate: StartTime phải nhỏ hơn EndTime
            if (request.StartTime >= request.EndTime)
            {
                throw new InvalidOperationException("StartTime must be less than EndTime");
            }

            // Check trùng slot
            if (_slotRepository.IsSlotOverlapping(request.StartTime, request.EndTime))
            {
                throw new InvalidOperationException($"Slot time ({request.StartTime} - {request.EndTime}) overlaps with an existing slot");
            }

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

        public bool DeleteSlot(int slotId)
        {
            var user = _slotRepository.DeleteSlot(slotId);

            if (!user)
                throw new KeyNotFoundException($"Slot with id {slotId} does not exist");

            _slotRepository.SaveChanges();
            return true;
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
                SlotId = slot.SlotId,
                SlotName = slot.SlotName,
                StartTime = slot.StartTime,
                EndTime = slot.EndtTime

            };
        }

        public SlotDTO? UpdateSlot(int slotId, CreateUpdateSlotRequest request)
        {


            // Get existing slot
            var slot = _slotRepository.GetSlotById(slotId);
            if (slot == null)
            {
                throw new KeyNotFoundException($"Slot with ID {slotId} not found");
            }

            // Validate: StartTime phải nhỏ hơn EndTime
            if (request.StartTime >= request.EndTime)
            {
                throw new InvalidOperationException("StartTime must be less than EndTime");
            }

            // Check trùng slot với các slot khác (exclude slot hiện tại)
            if (_slotRepository.IsSlotOverlappingExcludeCurrent(slotId, request.StartTime, request.EndTime))
            {
                throw new InvalidOperationException($"Updated slot time ({request.StartTime} - {request.EndTime}) overlaps with another existing slot");
            }

            // Update slot properties
            slot.SlotName = request.SlotName;
            slot.StartTime = request.StartTime;
            slot.EndtTime = request.EndTime;

            _slotRepository.SaveChanges();

            return new SlotDTO
            {
                SlotId = slot.SlotId,
                SlotName = slot.SlotName,
                StartTime = slot.StartTime,
                EndTime = slot.EndtTime
            };
        }
    }
}
