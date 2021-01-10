using System;
using System.Net;
using AAS.Architecture.Language;
using Convey.WebApi.Exceptions;

namespace AAS.Architecture.Exceptions
{
    public abstract class DefaultInternalizationExceptionToResponseMapper: IExceptionToResponseMapper
    {

        public ExceptionResponse Map(Exception exception)
        {
            exception.SetCulture();
            switch (exception)
            {
                case ExternalException ex:
                    return new ExceptionResponse(new ExceptionDetails {Code = ex.Code, Reason = ex.Message},
                        HttpStatusCode.BadRequest);
                case DomainException ex:
                    return new ExceptionResponse(
                        new ExceptionDetails
                        {
                            Code = ex.Code,
                            Reason = ex.TranslationParameters.Length > 0
                                ? string.Format(GetTranslation(ex.TranslationKey), ex.TranslationParameters)
                                : GetTranslation(ex.TranslationKey)
                        }, HttpStatusCode.BadRequest);
                case AppException ex:
                    return new ExceptionResponse(
                        new ExceptionDetails
                        {
                            Code = ex.Code,
                            Reason = ex.TranslationParameters.Length > 0
                                ? string.Format(GetTranslation(ex.TranslationKey), ex.TranslationParameters)
                                : GetTranslation(ex.TranslationKey)
                        }, HttpStatusCode.BadRequest);
                default:
                    return new ExceptionResponse(new {code = "error", reason = "There was an error."},
                        HttpStatusCode.BadRequest);
            }
        }

        protected abstract string GetTranslation(string key);
    }
}