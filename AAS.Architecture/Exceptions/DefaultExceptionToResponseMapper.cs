using System;
using System.Net;
using Convey.WebApi.Exceptions;
using JetBrains.Annotations;

namespace AAS.Architecture.Exceptions
{
    [UsedImplicitly]
    public class DefaultExceptionToResponseMapper : IExceptionToResponseMapper
    {
        public ExceptionResponse Map(Exception exception)
            => exception switch
            {
                ExternalException ex => new ExceptionResponse(new ExceptionDetails
                {
                    Code = ex.Code,
                    Reason = ex.Message
                }, HttpStatusCode.BadRequest),
                DomainException ex => new ExceptionResponse(new ExceptionDetails
                {
                    Code = ex.Code,
                    Reason = ex.Message
                }, HttpStatusCode.BadRequest),
                AppException ex => new ExceptionResponse(new ExceptionDetails
                {
                    Code = ex.Code,
                    Reason = ex.Message
                }, HttpStatusCode.BadRequest),
                _ => new ExceptionResponse(new {code = "error", reason = "There was an error."},
                    HttpStatusCode.BadRequest)
            };
    }
}