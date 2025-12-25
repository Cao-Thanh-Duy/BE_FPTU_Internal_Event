using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend_FPTU_Internal_Event.BLL.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;

        public FeedbackService(
            IFeedbackRepository feedbackRepository,
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            IEventRepository eventRepository)
        {
            _feedbackRepository = feedbackRepository;
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        public FeedbackDTO CreateFeedback(CreateFeedbackRequest request, int userId)
        {
            // 1. Validate ticket exists
            var ticket = _ticketRepository.GetTicketByTicketId(request.TicketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");
            }

            // 2. Validate ticket belongs to user
            if (ticket.UserId != userId)
            {
                throw new InvalidOperationException("You can only feedback for your own tickets");
            }

            // 3. Validate ticket status is "Checked"
            if (ticket.Status != "Checked")
            {
                throw new InvalidOperationException($"Cannot submit feedback. Ticket must be checked in first. Current status: {ticket.Status}");
            }

            // 4. Check if feedback already exists for this ticket
            var existingFeedback = _feedbackRepository.GetFeedbackByTicketId(request.TicketId);
            if (existingFeedback != null)
            {
                throw new InvalidOperationException("Feedback has already been submitted for this ticket");
            }

            // 5. Get user and event info
            var user = _userRepository.GetUserById(userId);
            var eventEntity = _eventRepository.GetEventById(ticket.EventId);

            if (user == null || eventEntity == null)
            {
                throw new KeyNotFoundException("User or Event not found");
            }

            // 6. Create feedback
            var feedback = new Feedback
            {
                TicketId = request.TicketId,
                UserId = userId,
                EventId = ticket.EventId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var createdFeedback = _feedbackRepository.AddFeedback(feedback);

            // 7. Return DTO
            return new FeedbackDTO
            {
                FeedbackId = createdFeedback.FeedbackId,
                TicketId = createdFeedback.TicketId,
                UserId = createdFeedback.UserId,
                UserName = user.UserName,
                EventId = createdFeedback.EventId,
                EventName = eventEntity.EventName,
                Rating = createdFeedback.Rating,
                Comment = createdFeedback.Comment,
                CreatedAt = createdFeedback.CreatedAt
            };
        }

        public FeedbackDTO? GetFeedbackByTicketId(int ticketId)
        {
            var feedback = _feedbackRepository.GetFeedbackByTicketId(ticketId);

            if (feedback == null)
                return null;

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                TicketId = feedback.TicketId,
                UserId = feedback.UserId,
                UserName = feedback.User?.UserName ?? string.Empty,
                EventId = feedback.EventId,
                EventName = feedback.Event?.EventName ?? string.Empty,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                CreatedAt = feedback.CreatedAt
            };
        }

        public List<FeedbackDTO> GetMyFeedbacks(int userId)
        {
            var feedbacks = _feedbackRepository.GetFeedbacksByUserId(userId);

            return feedbacks.Select(f => new FeedbackDTO
            {
                FeedbackId = f.FeedbackId,
                TicketId = f.TicketId,
                UserId = f.UserId,
                UserName = f.User?.UserName ?? string.Empty,
                EventId = f.EventId,
                EventName = f.Event?.EventName ?? string.Empty,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        public EventFeedbackSummaryDTO GetEventFeedbackSummary(int eventId)
        {
            var eventEntity = _eventRepository.GetEventById(eventId);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException($"Event with ID {eventId} not found");
            }

            var feedbacks = _feedbackRepository.GetFeedbacksByEventId(eventId);

            var feedbackDTOs = feedbacks.Select(f => new FeedbackDTO
            {
                FeedbackId = f.FeedbackId,
                TicketId = f.TicketId,
                UserId = f.UserId,
                UserName = f.User?.UserName ?? string.Empty,
                EventId = f.EventId,
                EventName = f.Event?.EventName ?? string.Empty,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();

            return new EventFeedbackSummaryDTO
            {
                EventId = eventId,
                EventName = eventEntity.EventName,
                TotalFeedbacks = feedbacks.Count,
                AverageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0,
                FiveStars = feedbacks.Count(f => f.Rating == 5),
                FourStars = feedbacks.Count(f => f.Rating == 4),
                ThreeStars = feedbacks.Count(f => f.Rating == 3),
                TwoStars = feedbacks.Count(f => f.Rating == 2),
                OneStar = feedbacks.Count(f => f.Rating == 1),
                Feedbacks = feedbackDTOs
            };
        }
    }
}