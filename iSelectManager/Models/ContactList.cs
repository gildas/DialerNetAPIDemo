using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using ININ.IceLib.Configuration.Dialer.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace iSelectManager.Models
{
    public class ContactList
    {
        public string id { get; set; }
        [Display(Name="Contact List")]
        public string DisplayName { get; set; }
        public int AffectedRecords { get; set; }

        private ININ.IceLib.Configuration.Dialer.ContactListConfiguration configuration { get; set; }

        public static ICollection<ContactList> find_all()
        {
            return Application.ContactListConfigurations.Select(item => new ContactList(item)).ToList();
        }

        public static ContactList find(string id)
        {
            try
            {
                return new ContactList(Application.ContactListConfigurations.First(item => item.ConfigurationId.Id == id));
            }
            catch(InvalidOperationException)
            {
                throw new KeyNotFoundException(string.Format("Unable to find a {0} with key {1}", MethodInfo.GetCurrentMethod().DeclaringType.Name, id));
            }
        }

        public static ContactList find(ConfigurationId id)
        {
            return find(id.Id);
        }

        public ContactList()
        {
            id = string.Empty;
            DisplayName = string.Empty;
            configuration = null;
            AffectedRecords = -1;
        }

        public ContactList(ININ.IceLib.Configuration.Dialer.ContactListConfiguration ic_contactlist)
        {
            id = ic_contactlist.ConfigurationId.Id;
            DisplayName = ic_contactlist.ConfigurationId.DisplayName;
            configuration = ic_contactlist;
            AffectedRecords = -1;
        }

        public IEnumerable<string> columns
        {
            get
            {
                var values = new List<string>();

                return values;
            }
        }

        public int ScheduleCall(string column, string key, Campaign campaign, string agent_id, string site_id, DateTime when)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var select = new SelectCommand(configuration);

            select.Where = new BinaryExpression(new ColumnExpression(configuration.ColumnMap[column]), new ConstantExpression(key, configuration.ColumnMap[column]), BinaryOperationType.Equal);
            Collection<Dictionary<string, object>> contacts = configuration.GetContacts(dialer_configuration.GetHttpRequestKey(configuration.ConfigurationId), select);

            var calls = new List<ScheduledCall>();

            foreach(var ic_contact in contacts)
            {
                calls.Add(new ScheduledCall(ic_contact[ContactListConfiguration.I3_Identity.Name].ToString(), campaign.id, "", agent_id, site_id, when));
            }
            return configuration.AddScheduledCalls(calls);
        }

        public int UpdateContacts(DBColumn search_column, string key, DBColumn value_column, string new_value)
        {
            var update = new UpdateCommand(configuration, null);

            update.Where = new BinaryExpression(new ColumnExpression(search_column), new ConstantExpression(key, search_column), BinaryOperationType.Equal);

            update.UpdateData[value_column] = new_value;

            ContactListTransaction transaction = new ContactListTransaction();
            transaction.Add(update);
            return configuration.RunTransaction(transaction);
        }

        public int UpdateContacts(string search_column, string key, string value_column, string new_value)
        {
            return UpdateContacts(configuration.ColumnMap[search_column], key, configuration.ColumnMap[value_column], new_value);
        }

        public int UpdateContacts(DBColumn search_column, string key, string value_column, string new_value)
        {
            return UpdateContacts(search_column, key, configuration.ColumnMap[value_column], new_value);
        }

        public int UpdateContacts(string search_column, string key, DBColumn value_column, string new_value)
        {
            return UpdateContacts(configuration.ColumnMap[search_column], key, value_column, new_value);
        }

        public int UpdateContactStatuses(string search_column, string key, string new_status)
        {
            return UpdateContacts(search_column, key, ContactListConfiguration.Status, new_status);
        }

        public int upload_records(string filename)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var data    = new CSVDataSet(filename);
            var fields  = data.Fields;
            var columns = configuration.GetColumns();
            var mapping = new Dictionary<DBColumn, DBColumn>();

            foreach(var field in fields)
            {
                var column = fields.First(item => string.Compare(item.Name, field.Name) == 0);

                if (column != null)
                {
                    mapping.Add(field, column);
                }
            }
            if (mapping.Keys.Count <= 0) throw new ArgumentException(string.Format("No mapping available between Contact List {0} and CSV filename {1}", DisplayName, filename), filename);

            return configuration.BulkImport(dialer_configuration.GetHttpRequestKey(configuration.ConfigurationId), data, mapping);
        }

        public int upload_records(string server, string database, string table, string user, string password)
        {
            var data = new SQLServerDataSet(server, database, table, user, password);

            return 0;
        }
    }
}