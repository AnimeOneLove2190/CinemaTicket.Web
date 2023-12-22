using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTicket.Infrastructure.Constants
{
    public static class ExceptionMessageTemplate
    {
        public const string NotFound = "{0} with id {1} not found.";
        public const string NotFoundNumber = "{0} with number {1} not found.";
        public const string NotFoundName = "{0} with name ({1}) not found.";
        public const string SameNameAlreadyExist = "{0} with the same name ({1}) already exists.";
        public const string SameNameAndDescriptionAlreadyExist = "{0} with the same name ({1}) and description already exists.";
        public const string SameFieldValueAlreadyExist = "{0} with the same {1} field value already exists.";
        public const string RequestIsNull = "Request {0} is null.";
        public const string FieldIsRequired = "Field {0} is required";
        public const string ListNotFound = "List of {0} not found";
        public const string EntityHasSoldTickets = "{0} has sold tickets";
        public const string CannotBeNullOrNegative = "{0} {1} field must not be empty or contain a negative value";
        public const string Duplicate = "{0} list has one or more duplicate.";
        public const string NotAllFound = "Not all items from the {0} were found in the database";
        public const string TicketIsSold = "Ticket with Id {0} has already been sold, it cannot be changed";
        public const string TicketsAreSold = "One or more tickets are sold";
        public const string VariousHalls = "The Hall specified in the Session and the Hall specified in the Plaсе do not match";
        public const string WrongLoginOrPassword = "Wrong login or password";
        public const string ColumnNotFound = "Column {0} not found";
    }
}
