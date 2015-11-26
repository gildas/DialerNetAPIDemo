using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DialerNetAPIDemo.Models
{
    public class Contact
    {
        public string id { get; set; }
        public Dictionary<string, object> Columns { get; set; }
        public ContactList ContactList { get; set; }

        public IEnumerable<string> EditableColumns
        {
            get
            {
                if (Columns == null) return new List<string>();
                var pattern = new Regex("(^I3_.*)|(.*HISTORY$)|(.*LOG$)");
                return Columns.Where(column => { return !pattern.Match(column.Key).Success; }).OrderBy(item => item.Key).Select(columns => columns.Key);
            }
        }

        public const string CONTACT_KEYNAME = "I3_IDENTITY";

        public static IEnumerable<Contact> find_all(ContactList contactList)
        {
            return contactList.find_all_contacts();
        }

        public static Contact find(ContactList contactList, string id)
        {
            return contactList.find_contact(id);
        }
    }
}
