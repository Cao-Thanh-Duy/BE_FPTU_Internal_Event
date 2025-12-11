using Backend_FPTU_Internal_Event.DAL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.DAL.Interface
{
    public interface ITicketRepository
    {
        bool UserExistsInEvent(int userId, int eventId);
        bool UserExists(int userId);
        bool EventExists(int eventId);
        Ticket AddTicket(Ticket ticket);

        void SaveChanges();
        List<Ticket> GetTicketByUserId(int userId);
        Ticket GetTicketByTicketId(int tiketId);
        Ticket? GetTicketByTicketCode(Guid ticketCode);
    }
}
