using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WispCloud.Logic;

namespace WispCloud.Data
{
    public class Group
    {
        public int GroupID { get; protected set; }

        [JsonIgnore]
        public long InstallationID { get; protected set; }

        [JsonIgnore]
        public virtual Installation Installation { get; protected set; }

        [JsonIgnore]
        [Required]
        public bool IsMainGroupInInstallation { get; protected set; }

        public int? ParentGroupID { get; set; }

        [JsonIgnore]
        [ForeignKey("ParentGroupID")]
        public virtual Group ParentGroup { get; set; }

        [JsonIgnore]
        public virtual List<PowerBar> PowerBars { get; set; }

        [JsonIgnore]
        public virtual List<ButtonWallSwitch> ButtonWallSwitches { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public Group()
        {
            this.Name = string.Empty;
            this.ShortName = string.Empty;
        }

        public Group(Installation installation, bool isMainGroupInInstallation = false)
            : this()
        {
            this.Installation = installation;
            this.IsMainGroupInInstallation = isMainGroupInInstallation;
            this.ShortName = StaticRandom.GenerateName(4);
            this.Name = $"Group {this.ShortName}";
        }

    }

}