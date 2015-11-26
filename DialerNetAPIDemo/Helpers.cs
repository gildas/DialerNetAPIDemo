using ININ.IceLib.Configuration.DataTypes;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

namespace DialerNetAPIDemo
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

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) { action(item); }
            return enumerable;
        }

        public static DataTable LoadCSV(string filepath)
        {
            var data = new DataTable();

            using (var parser = new TextFieldParser(filepath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                while (!parser.EndOfData)
                {
                    try
                    {
                        var fields = parser.ReadFields();
                        if (data.Columns.Count == 0)
                        {
                            foreach (var column_name in fields)
                            {
                                DataColumn column = new DataColumn(column_name);

                                column.AllowDBNull = true;
                                data.Columns.Add(column);
                            }
                        }
                        else
                        {
                            data.Rows.Add(fields);
                        }
                    }
                    catch(MalformedLineException)
                    {
                        // We will ignore malformed lines
                    }
                }
            }
            return data;
        }

        public static IEnumerable<SkillSettings> ParseManySkillSettings(string data, char separator = ',')
        {
            var settings = new List<SkillSettings>();

            if (! string.IsNullOrWhiteSpace(data))
            {
                data.Split(separator).Where(x => !string.IsNullOrWhiteSpace(x)).ForEach(x => settings.Add(ParseSkillSettings(x)));
            }
            return settings;
        }

        public static SkillSettings ParseSkillSettings(string data, params char[] separators)
        {
            var    settings      = data.Split((separators.Count() > 0) ? separators : new[] { '|', ';' });
            string workgroup_id  = settings[0];
            int    proficiency   = 1;
            int    desire_to_use = 1;

            if (settings.Count() > 0) { Int32.TryParse(settings[1], out proficiency  ); }
            if (settings.Count() > 1) { Int32.TryParse(settings[2], out desire_to_use); }
            return new SkillSettings(workgroup_id, proficiency, desire_to_use);
        }
    }

    public class CSVMapper
    {
        public DataTable DataTable { get; set; }

        public int this[string index_name] { get { return _map[index_name];  } }

        public int map(string index_name, params string[] aliases)
        {
            for (int i = 0; i < DataTable.Columns.Count; i++)
            {
                var column = DataTable.Columns[i];

                if (aliases.Any(x => string.Equals(x, column.ColumnName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return _map[index_name] = i;
                }
            }
            return -1;
        }

        private Dictionary<string, int> _map = new Dictionary<string, int>();
    }

    static public class Uploader
    {
        public static string Process(HttpPostedFileBase http_upload)
        {
            return Process(http_upload.FileName, http_upload.InputStream, http_upload.ContentLength);
        }

        public static string Process(string filename, System.IO.Stream stream, int content_length)
        {
            var rootpath = WebConfigurationManager.AppSettings["UploadPath"] ?? Environment.GetEnvironmentVariable("TEMP") ?? @"C:\Windows\Temp";
            if (rootpath.StartsWith("/")) rootpath = HttpContext.Current.Server.MapPath(rootpath);
            var filepath = Path.GetFullPath(Path.Combine(rootpath, Path.GetFileName(filename)));

            using (var reader = new BinaryReader(stream))
            {
                using (var writer = File.Create(filepath))
                {
                    writer.Write(reader.ReadBytes(content_length), 0, content_length);
                }
            }
            return filepath;
        }
    }
}
