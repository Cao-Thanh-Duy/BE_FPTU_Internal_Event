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
    public class SlotRepositoty : ISlotRepository
    {
        private readonly ApplicationDbContext _context;
        public SlotRepositoty(ApplicationDbContext context)
        {
            _context = context;
        }


        public Slot? AddSlot(Slot slot)
        {
            _context.Add(slot);
            return slot;
        }

        public bool DeleteSlot(int slotId)
        {
            var loadSlot = _context.Slots.Find(slotId);
            if (loadSlot != null)
            {
                _context.Slots.Remove(loadSlot);
                return true;
            }
            else return false;
        }

        public List<Slot> GetAllSlot()
        {
            return _context.Slots.ToList();
        }

        public Slot? GetSlotById(int slotId)
        {
            return _context.Slots.FirstOrDefault(v => v.SlotId == slotId);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public bool IsSlotOverlapping(TimeOnly startTime, TimeOnly endTime)
        {
            return _context.Slots.Any(s =>
                // Check if new slot overlaps with existing slots
                (startTime >= s.StartTime && startTime < s.EndtTime) ||  // StartTime nằm trong slot hiện có
                (endTime > s.StartTime && endTime <= s.EndtTime) ||      // EndTime nằm trong slot hiện có
                (startTime <= s.StartTime && endTime >= s.EndtTime)      // Slot mới bao trùm slot hiện có
            );
        }
    }
}
