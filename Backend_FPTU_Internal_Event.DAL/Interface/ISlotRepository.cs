using Backend_FPTU_Internal_Event.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Interface
{
    public interface ISlotRepository
    {
        Slot? AddSlot(Slot slot);
        Slot? GetSlotById(int slotId);
        List<Slot> GetAllSlot();
        void SaveChanges();

        bool DeleteSlot(int slotId);
    }
}
