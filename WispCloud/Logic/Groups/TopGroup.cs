using System.Collections.Generic;
using System.Linq;
using WispCloud.Data;

namespace WispCloud.Logic
{
    public sealed class TopGroup : GroupItem
    {
        Dictionary<int, GroupItem> _groupsByID;

        public List<PowerBarItem> AllPowerBars { get; private set; }

        public TopGroup(List<Group> groups)
            : base(null)
        {
            this.AddGroups(groups);
        }

        void AddGroups(List<Group> groups)
        {
            if (_groupsByID == null)
                _groupsByID = new Dictionary<int, GroupItem>();

            foreach (var group in groups)
            {
                if (group.IsMainGroupInInstallation)
                    Group = group;

                _groupsByID.Add(group.GroupID, new GroupItem(group));
            }

            Try.NotNull(Group, "Main group not found;", false);

            foreach (var group in groups)
            {
                if (!group.ParentGroupID.HasValue)
                    continue;

                var item = _groupsByID[group.GroupID];
                var parentItem = _groupsByID[group.ParentGroupID.Value];
                if (parentItem.Groups == null)
                    parentItem.Groups = new List<GroupItem>();
                parentItem.Groups.Add(item);
                item.Parent = parentItem;
            }

            if (_groupsByID.Any())
            {
                Groups = _groupsByID.Values.Cast<GroupItem>()
                    .Where(x => x.Parent == null && !x.IsMainGroupInInstallation)
                    .ToList();

                if (!Groups.Any())
                {
                    Groups = null;
                }
                else
                {
                    foreach (var topChild in Groups)
                        topChild.Parent = this;
                }
            }
        }

        public GroupItem GetGroupById(int groupID)
        {
            if (Group.GroupID == groupID)
                return this;

            GroupItem item;
            _groupsByID.TryGetValue(groupID, out item);
            return item;
        }

        public void AddPowerBars(List<PowerBar> powerBars)
        {
            foreach (var group in powerBars)
            {
                var item = new PowerBarItem(group);

                if (AllPowerBars == null)
                    AllPowerBars = new List<PowerBarItem>();
                AllPowerBars.Add(item);

                if (group.Groups != null && group.Groups.Any())
                {
                    foreach (var parent in group.Groups)
                    {
                        var parentItem = _groupsByID[parent.GroupID];
                        if (parentItem.PowerBars == null)
                            parentItem.PowerBars = new List<PowerBarItem>();
                        parentItem.PowerBars.Add(item);
                        if (item.Groups == null)
                            item.Groups = new List<GroupItem>();
                        item.Groups.Add(parentItem);
                    }
                }
                else
                {
                    if (PowerBars == null)
                        PowerBars = new List<PowerBarItem>();
                    PowerBars.Add(item);
                }
            }
        }

        public void UpdateStatuses()
        {
            if (Groups == null || !Groups.Any())
            {
                UpdateStatus();
            }
            else
            {
                var itemsStatusQueue = new List<BaseGroupItem>();

                var allGroups = GetAllGroups();
                itemsStatusQueue.AddRange(allGroups);

                foreach (var item in itemsStatusQueue.AsEnumerable().Reverse())
                    item.UpdateStatus();
            }
        }

        public void CheckCycles()
        {
            Try.Condition(GetAllGroups().Count() == _groupsByID.Count, "Groups hierarchy contains cycles;");
        }

    }

}