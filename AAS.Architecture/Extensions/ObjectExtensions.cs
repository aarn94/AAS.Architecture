using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AAS.Architecture.Extensions
{
    public static class ObjectExtensions
    {
          public static  string GetQueryParameters<TS, TAT>(this TS  obj, bool encode = true, List<string> selectedProperties = null, Func<TAT, string> attributeValueProvider = null) 
              where TAT : Attribute
          {
              var properties = from p in obj.GetType()
                      .GetProperties()
                      .Where(e => selectedProperties is null || !selectedProperties.Contains(e.Name))
                  where p.GetValue(obj, null) != null
                  select GetNameOrAttribute(p, attributeValueProvider) + "=" + GetValue(obj, p, encode, attributeValueProvider);
  
              return string.Join("&", properties.ToArray());
          }
          
          public static  string GetFilteredQueryParameters<TS, TAT>(this TS  obj, bool encode = true, List<string> selectedProperties = null, Func<TAT, string> attributeValueProvider = null) 
              where TAT : Attribute
          {
              var properties = from p in obj.GetType()
                      .GetProperties()
                      .Where(e => selectedProperties is null || selectedProperties.Contains(e.Name))
                  where p.GetValue(obj, null) != null
                  select GetNameOrAttribute(p, attributeValueProvider) + "=" + GetValue(obj, p, encode, attributeValueProvider);
  
              return string.Join("&", properties.ToArray());
          }
          
          public static string GetValue<TS, TAT>(this TS obj, PropertyInfo p, bool encode, Func<TAT, string> attributeValueProvider = null) where TAT : Attribute
          {
              if (encode)
                  return HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());
              else
                  return p.GetValue(obj, null).ToString();
          }
  
          private static string GetNameOrAttribute<TAT>(PropertyInfo p, Func<TAT, string> attributeValueProvider = null) where TAT : Attribute
          {
              if (attributeValueProvider is null)
                  return p.Name.ToLower();
  
              var attribute = p.GetCustomAttribute<TAT>();
              return attribute is null ? p.Name.ToLower() : attributeValueProvider(attribute);
          }
        
    }
}