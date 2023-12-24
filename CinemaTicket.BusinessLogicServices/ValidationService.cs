using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;
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
        public void ValidationSameNameAlreadyExist<T>(T entity, string name)
        {
            if (entity != null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAlreadyExist, typeof(T).Name, name);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationSameNameAlreadyExist<T>(T entity, string name, int DTOId, int entityId)
        {
            if (entity != null && DTOId != entityId)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAlreadyExist, typeof(T).Name, name);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationSameNameAndDescrtiptionAlreadyExist<T>(T entity, string entityName, string entityDescription, string DTODescription)
        {
            if (entity != null && entityDescription.ToLower() == DTODescription.ToLower())
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAndDescriptionAlreadyExist, typeof(T).Name, entityName);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationSameNameAndDescrtiptionAlreadyExist<T>(T entity, string entityName, int entityId, int DTOId)
        {
            if (entity != null && entityId != DTOId)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameNameAndDescriptionAlreadyExist, typeof(T).Name, entityName);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationFieldValueAlreadyExist(string typeName, string fieldName)
        {
            var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, typeName, fieldName);
            logger.LogError(exceptionMessage);
            throw new CustomException(exceptionMessage);
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
        public void ValidationListNotFound<T>(ICollection<T> entities)
        {
            if (entities.Count == 0)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.ListNotFound, typeof(T).Name);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
        }
        public void ValidationEntityHasSoldTickets<T>(string typeName, ICollection<T> entities)
        {
            if (entities.Count > 0)
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
        public void ValidationCannotBeNullOrNegative<T>(T entity, string fieldName, DateTime fieldValue)
        {
            if (fieldValue == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.CannotBeNullOrNegative, typeof(T).Name, fieldName);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationCannotBeNullOrNegative<T>(T entity, string fieldName, ICollection<int> fieldValues)
        {
            if (fieldValues.Count > 0)
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
        public void ValidationNotAllFound<T>(ICollection<T> listFromDB, ICollection<T> listFromDTO, string listName)
        {
            if (listFromDB.Count != listFromDTO.Count)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotAllFound, listName);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationTicketIsSold(bool isSold, int ticketId)
        {
            if (isSold)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.TicketIsSold, ticketId);
                logger.LogError(exceptionMessage);
                throw new CustomException(exceptionMessage);
            }
        }
        public void ValidationTicketsAreSold<T>(ICollection<T> listFromDB, ICollection<T> listRequest)
        {
            if (listFromDB.Count != listRequest.Count)
            {
                logger.LogError(ExceptionMessageTemplate.TicketsAreSold);
                throw new CustomException(ExceptionMessageTemplate.TicketsAreSold);
            }
        }
        public void ValidationVariousHalls(int sessionHallId, int rowHallId)
        {
            if (sessionHallId != rowHallId)
            {
                logger.LogError(ExceptionMessageTemplate.VariousHalls);
                throw new CustomException(ExceptionMessageTemplate.VariousHalls);
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
