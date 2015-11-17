using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.Dialer;
using ININ.IceLib.Configuration.Dialer.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public static ICollection<ContactList> find_all(IEnumerable<ContactListConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new ContactListConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();
            var contactlists = new List<ContactList>();

            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            foreach (var configuration in configurations)
            {
                contactlists.Add(new ContactList(configuration));
            }
            return contactlists;
        }

        public static ContactList find(string id, IEnumerable<ContactListConfiguration.Property> properties = null)
        {
            var dialer_configuration = new DialerConfigurationManager(Application.ICSession);
            var query = new ContactListConfigurationList(dialer_configuration.ConfigurationManager);
            var query_settings = query.CreateQuerySettings();

            query_settings.SetFilterDefinition(ContactListConfiguration.Property.Id, id, FilterMatchType.Exact);
            query_settings.SetPropertiesToRetrieve((properties ?? DefaultProperties).Union(MandatoryProperties));
            query.StartCaching(query_settings);
            var configurations = query.GetConfigurationList();
            query.StopCaching();

            if (configurations.Count() == 0) throw new KeyNotFoundException(id);
            if (configurations.Count()  > 1) throw new IndexOutOfRangeException(id);
            return new ContactList(configurations.First());
        }

        public static ContactList find(ConfigurationId id, IEnumerable<ContactListConfiguration.Property> properties = null)
        {
            return find(id.Id, properties);
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

        private static List<ContactListConfiguration.Property> DefaultProperties   = new List<ContactListConfiguration.Property>();
        private static List<ContactListConfiguration.Property> MandatoryProperties = new List<ContactListConfiguration.Property> { ContactListConfiguration.Property.Id, ContactListConfiguration.Property.DisplayName };
    }
}