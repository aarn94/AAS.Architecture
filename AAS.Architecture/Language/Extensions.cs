using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AAS.Architecture.Decorators.Language;
using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AAS.Architecture.Language
{
    public static class Extensions
    {
        public static IConveyBuilder AddLanguage(this IConveyBuilder builder, string defaultLanguage, List<string> supportingLanguages)
        {
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.Configure<RequestLocalizationOptions>(opts =>
            {

                var supportedCultures = supportingLanguages.Select(e => new CultureInfo(e)).ToList();

                opts.DefaultRequestCulture = new RequestCulture(defaultLanguage);
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
               
                opts.RequestCultureProviders.Clear();
                
                opts.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                {
                    var userLangs = context.Request.Headers["Language"].ToString();
                    var firstLang = userLangs.Split(',').FirstOrDefault();
                    var defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
                    return Task.FromResult(new ProviderCultureResult(defaultLang, defaultLang));
                }));
            });
            
            builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(CorrelationContextCommandHandlerDecorator<>));
            builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(CorrelationContextEventHandlerDecorator<>));
            builder.Services.TryDecorate(typeof(IQueryHandler<,>), typeof(CorrelationContextQueryHandlerDecorator<,>));
            
            return builder;
        }
        
        public static IApplicationBuilder UseLanguage(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            return app.UseRequestLocalization(options.Value);
        }
    }
    
    
}