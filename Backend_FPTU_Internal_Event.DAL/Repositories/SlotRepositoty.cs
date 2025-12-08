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

        public List<Slot> GetAllSlot()
        {
            return _context.Slots.ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
