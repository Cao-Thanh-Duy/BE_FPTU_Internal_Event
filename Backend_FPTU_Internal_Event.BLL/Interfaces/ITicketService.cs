using Backend_FPTU_Internal_Event.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_FPTU_Internal_Event.BLL.Interfaces
{
    public interface ITicketService
    {
        TicketDTO GenerateTicket(GenTicketRequest request);
        string GenerateQrCode(Guid ticketCode);
        List<TicketEventDTO> GetTikets(int userId);
        Guid GetTicketCodeByTicketId(int tiketId);
        TicketEventDTO GetTicketByTicketCode(Guid ticketCode);
        bool CheckIn(int ticketId);
        bool Cancelled(int ticketId);

        EventAttendeesResponse GetEventAttendees(int eventId);
    }
}
