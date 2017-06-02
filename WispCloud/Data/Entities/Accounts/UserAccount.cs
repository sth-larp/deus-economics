using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DeusCloud.Serialization;
using Newtonsoft.Json;

namespace DeusCloud.Data.Entities.Accounts
{
    public class UserAccount : Account
    {
        [NotMapped]
        public UserSettings Settings { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SettingsJson
        {
            get { return WispJsonSerializer.SerializeToJsonString(Settings); }
            set { Settings = WispJsonSerializer.DeserializeJson<UserSettings>(value); }
        }

        public UserAccount()
        {
        }

        public UserAccount(string email, AccountRole role)
            : base(email, role)
        {
        }

    }

}