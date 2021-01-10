using System;
using System.Globalization;
using AAS.Architecture.Exceptions;

namespace AAS.Architecture.Language
{
    public static class ExceptionHelper
    {
        public static void SetCulture(this Exception exception)
        {
            switch (exception)
            {
                case DomainException obj:
                    CultureInfo.CurrentCulture = new CultureInfo(obj.Lang);
                    CultureInfo.CurrentUICulture = new CultureInfo(obj.Lang);
                    break;
                case AppException obj2:
                    CultureInfo.CurrentCulture = new CultureInfo(obj2.Lang);
                    CultureInfo.CurrentUICulture = new CultureInfo(obj2.Lang);
                    break;
            }
        }
    }
}