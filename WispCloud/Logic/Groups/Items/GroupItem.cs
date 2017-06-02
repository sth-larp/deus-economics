using Newtonsoft.Json;
using WispCloud.Data;

namespace WispCloud.Logic
{
    public class GroupItem : BaseGroupItem
    {
        [JsonIgnore]
        public Group Group { get; protected set; }

        public int GroupID { get { return Group.GroupID; } }
        public string Name { get { return Group.Name; } }
        public string ShortName { get { return Group.ShortName; } }
        public bool IsMainGroupInInstallation { get { return Group.IsMainGroupInInstallation; } }

        public GroupItem(Group powerBarsGroup)
        {
            this.Group = powerBarsGroup;
        }

    }

}