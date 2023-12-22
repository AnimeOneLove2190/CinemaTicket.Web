using System;
using System.Collections.Generic;
using System.Text;
using CinemaTicket.Entities;


namespace CinemaTicket.BusinessLogic.Interfaces
{
    public interface IValidationService
    {
        public void ValidationNotFound<T>(T entity, int id);
        public void ValidationNotFound<T>(T entity, string name);
        public void ValidationNotFound<T>(T entity);
        public void ValidationNotFoundNumber<T>(T entity, int number);
        public void ValidationSameNameAlreadyExist<T>(T entity, string name);
        public void ValidationSameNameAlreadyExist<T>(T entity, string name, int DTOId, int entityId);
        public void ValidationSameNameAndDescrtiptionAlreadyExist<T>(T entity, string entityName, string entityDescription, string DTODescription);
        public void ValidationSameNameAndDescrtiptionAlreadyExist<T>(T entity, string entityName, int entityId, int DTOId);
        public void ValidationFieldValueAlreadyExist(string typeName, string fieldName);
        public void ValidationRequestIsNull<T>(T model);
        public void ValidationFieldIsRequiered(string fieldName, string fieldValue);
        public void ValidationListNotFound<T>(ICollection<T> entities);
        public void ValidationEntityHasSoldTickets<T>(string typeName, ICollection<T> entities);
        public void ValidationCannotBeNullOrNegative<T>(T entity, string fieldName, int fieldValue);
        public void ValidationCannotBeNullOrNegative<T>(T entity, string fieldName, DateTime fieldValue);
        public void ValidationCannotBeNullOrNegative<T>(T entity, string fieldName, ICollection<int> fieldValues);
        public void ValidationDuplicate<T>(List<T> entities, string listName);
        public void ValidationNotAllFound<T>(ICollection<T> listFromDB, ICollection<T> listFromDTO, string listName);
        public void ValidationTicketIsSold(bool isSold, int ticketId);
        public void ValidationTicketsAreSold<T>(ICollection<T> listFromDB, ICollection<T> listRequest);
        public void ValidationVariousHalls(int sessionHallId, int rowHallId);
        public void ValidationWrongLoginOrPassword();
        public void ValidationColumnNotFound();
    }
}
