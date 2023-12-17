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
        public void ValidationNameAlreadyExist<T>(T entity, string name);
        public void ValidationFieldValueAlreadyExist<T>(T entity, int fieldValue);
        public void ValidationRequestIsNull<T>(T model);
        public void ValidationFieldIsRequiered(string fieldName, string fieldValue);
        public void ValidationListNotFound<T>(T entity, int number);
        public void ValidationEntityHasSoldTickets<T>(T entity, List<Ticket> soldTickets);
        public void ValidationCannotBeNullOrNegative<T>(T entity, string fieldName, int fieldValue);
        public void ValidationDuplicate<T>(List<T> entities, string listName);
        public void ValidationNotAllFound(string listName);
        public void ValidationTicketIsSold<T>(Ticket ticket);
        public void ValidationTicketsAreSold<T>(List<Ticket> tickets);
        public void ValidationWrongLoginOrPassword();
        public void ValidationColumnNotFound();
    }
}
