using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using CinemaTicket.Infrastructure.Constants;
using CinemaTicket.Infrastructure.Exceptions;

namespace CinemaTicket.BusinessLogicServices
{
    public class ValidationService
    {
        private readonly ILogger<ValidationService> logger;
        public void ValidationRequestIsNull<T>(T entity)
        {
            if (entity != null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.RequestIsNull, typeof(T).Name);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
        }
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
        public void ValidationNotFound<T>(T entity, string name)
        {
            if (entity == null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.NotFoundName, typeof(T).Name, name);
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
                throw new NotFoundException(exceptionMessage);
            }
        }
        public void ValidationFieldValueAlreadyExist<T>(T entity, int value)
        {
            if (entity != null)
            {
                var exceptionMessage = string.Format(ExceptionMessageTemplate.SameFieldValueAlreadyExist, typeof(T).Name, value);
                logger.LogError(exceptionMessage);
                throw new NotFoundException(exceptionMessage);
            }
        }
    }
}
