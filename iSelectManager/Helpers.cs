using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace iSelectManager
{
    static public class Helpers
    {
        // Get [DysplayName] attribute of property
        // http://stackoverflow.com/questions/5474460/get-displayname-attribute-of-a-property-in-strongly-typed-way
        public static string DisplayName<TModel>(Expression <Func<TModel, object>> expression)
        {
            Type type = typeof(TModel);
            IEnumerable<string> properties;

            switch(expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var unary = expression.Body as UnaryExpression;

                    properties = (unary != null ? unary.Operand : null).ToString().Split(".".ToCharArray()).Skip(1);
                    break;
                default:
                    properties = expression.Body.ToString().Split(".".ToCharArray()).Skip(1);
                    break;
            }
            string property_name = properties.Last();

            Expression subexpression = null;
            foreach(var property in properties.Take(properties.Count() - 1))
            {
                PropertyInfo info = type.GetProperty(property);
                subexpression = Expression.Property(subexpression, type.GetProperty(property));
                type = info.PropertyType;
            }

            DisplayAttribute attribute = (DisplayAttribute)type.GetProperty(property_name).GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

            // Look for [MetadataType] attribute in type hierarchy
            // http://stackoverflow.com/questions/1910532/attribute-isdefined-doesnt-see-attributes-applied-with-metadatatype-class
            if (attribute == null)
            {
                MetadataTypeAttribute metadata_type = (MetadataTypeAttribute)type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                if (metadata_type != null)
                {
                    var property = metadata_type.MetadataClassType.GetProperty(property_name);

                    if (property != null)
                    {
                        attribute = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    }
                }
            }
            return (attribute != null) ? attribute.GetName() : string.Empty;
        }
    }
}