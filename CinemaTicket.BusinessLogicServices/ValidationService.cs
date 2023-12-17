using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;
using CinemaTicket.Entities;
using CinemaTicket.BusinessLogic.Interfaces;

namespace CinemaTicket.BusinessLogicServices
{
    public class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> logger;
        public ValidationService(ILogger<ValidationService> logger)
        {
            this.logger = logger;
        }
        public void ValidationNotFound<T>(T entity, int id)
        {
            if (entity == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, typeof(T).Name, id);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
        }
        public void ValidationNotFound<T>(T entity)
        {
            if (entity == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFound, typeof(T).Name);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
        }
        public void ValidationNotFound<T>(T entity, string name)
        {
            if (entity == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFoundName, typeof(T).Name, name);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
        }
        public void ValidationNotFoundNumber<T>(T entity, int number)
        {
            if (entity == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFoundNumber, typeof(T).Name, number);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
        }
        public void ValidationNameAlreadyExist<T>(T entity, string name)
        {
            if (entity != null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAlreadyExist, typeof(T).Name, name);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationFieldValueAlreadyExist<T>(T entity, int fieldValue)
        {
            if (entity != null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, typeof(T).Name, fieldValue);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationRequestIsNull<T>(T model)
        {
            if (model == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, typeof(T).Name);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationFieldIsRequiered(string fieldName, string fieldValue)
        {
            if (string.IsNullOrEmpty(fieldValue) || string.IsNullOrWhiteSpace(fieldValue))
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.FieldIsRequired, fieldName);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationListNotFound<T>(T entity, int number)
        {
            if (entity == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFoundNumber, typeof(T).Name, number);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
        }
        public void ValidationEntityHasSoldTickets<T>(T entity, List<Ticket> soldTickets)
        {
            if (soldTickets.Count > 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.EntityHasSoldTickets, typeof(T).Name);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationCannotBeNullOrNegative<T>(T entity, string fieldName, int fieldValue)
        {
            if (fieldValue <= 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegative, typeof(T).Name, fieldName);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationDuplicate<T>(List<T> entities, string listName)
        {
            var uniqEntities = entities.Distinct().ToList();
            if (uniqEntities.Count != entities.Count)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.Duplicate, listName);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationNotAllFound(string listName)
        {
            var exceptionMessage = string.Format(ExceptionMessageTemplate.NotAllFound, listName);
            logger.LogError(exceptionMessage);
            throw new CustomException(exceptionMessage);
        }
        public void ValidationTicketIsSold<T>(Ticket ticket)
        {
            if (ticket.IsSold)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.TicketIsSold, ticket.Id);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationTicketsAreSold<T>(List<Ticket> tickets)
        {
            var soldTickets = tickets.Where(x => x.IsSold).ToList();
            if (soldTickets.Count > 0)
            {
                logger.LogError(ExceptionMessageTemplate.TicketsAreSold);
                throw new CustomException(ExceptionMessageTemplate.TicketsAreSold);
            }
        }
        public void ValidationWrongLoginOrPassword()
        {
            logger.LogError(ExceptionMessageTemplate.WrongLoginOrPassword);
            throw new CustomException(ExceptionMessageTemplate.WrongLoginOrPassword);
        }
        public void ValidationColumnNotFound()
        {
            logger.LogError(ExceptionMessageTemplate.WrongLoginOrPassword);
            throw new CustomException(ExceptionMessageTemplate.WrongLoginOrPassword);
        }
    }
}
