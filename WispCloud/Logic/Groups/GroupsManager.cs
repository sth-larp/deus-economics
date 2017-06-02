using System.Collections.Generic;
using System.Linq;
using WispCloud.Data;

namespace WispCloud.Logic
{
    public sealed class GroupsManager : BaseManager
    {
        public GroupsManager(WispContext context)
            : base(context)
        {
        }

        public void AddMainGroup(Installation installation)
        {
            var mainGroup = new Group(installation, true);
            mainGroup.Name = "All";
            mainGroup.ShortName = mainGroup.Name;
            Context.Data.Groups.Add(mainGroup);
        }

        public TopGroup Get(long installationID)
        {
            var topGroup = new TopGroup(Context.Data.GetGroups(installationID));
            topGroup.AddPowerBars(Context.Data.GetPowerBars(installationID, true));
            topGroup.UpdateStatuses();
            return topGroup;
        }

        void SetGroups(List<Group> allGroups, Group editedGroup, int[] newGroupIDs)
        {
            Try.Argument(newGroupIDs, nameof(newGroupIDs));

            var newGroupIDsSet = new HashSet<int>(newGroupIDs);
            foreach (var group in allGroups)
            {
                if (newGroupIDsSet.Contains(group.GroupID))
                {
                    if (group.ParentGroupID != editedGroup.GroupID)
                        group.ParentGroupID = editedGroup.GroupID;
                }
                else if (group.ParentGroupID == editedGroup.GroupID)
                    group.ParentGroupID = null;
            }
        }

        void SetPowerBars(List<PowerBar> allPowerBars, Group editedGroup, decimal[] newPowerBarSNs)
        {
            Try.Argument(newPowerBarSNs, nameof(newPowerBarSNs));

            var powerBarSNs = allPowerBars.Where(x => x.Groups
                .Any(y => y.GroupID == editedGroup.GroupID))
                .Select(x => x.PowerBarSN)
                .ToList();

            var newPowerBarSNsSet = new HashSet<decimal>(newPowerBarSNs);

            var toDeletePowerBarSNsSet = new HashSet<decimal>(powerBarSNs);

            toDeletePowerBarSNsSet.ExceptWith(newPowerBarSNsSet);
            if (toDeletePowerBarSNsSet.Any())
                foreach (var powerBar in allPowerBars.Where(x => toDeletePowerBarSNsSet.Contains(x.PowerBarSN)))
                    powerBar.Groups.RemoveAll(x => x.GroupID == editedGroup.GroupID);

            newPowerBarSNsSet.ExceptWith(powerBarSNs);
            if (newPowerBarSNsSet.Any())
                foreach (var powerBar in allPowerBars.Where(x => newPowerBarSNsSet.Contains(x.PowerBarSN)))
                    powerBar.Groups.Add(editedGroup);
        }

        void SetGroupProperties(List<Group> allGroups, List<PowerBar> allPowerBars,
            Group editedGroup, GroupClientData clientData)
        {
            if (clientData.Name != null)
                editedGroup.Name = clientData.Name;
            if (clientData.ShortName != null)
                editedGroup.ShortName = clientData.ShortName;

            if (editedGroup.IsMainGroupInInstallation)
            {
                Try.Condition(!clientData.ParentGroupID.HasValue, $"Cant set {nameof(clientData.ParentGroupID)} for main group;");
                Try.Condition(clientData.GroupIDs == null, $"Cant set {nameof(clientData.GroupIDs)} for main group;");
                Try.Condition(clientData.PowerBarSNs == null, $"Cant set {nameof(clientData.PowerBarSNs)} for main group;");
            }
            else
            {
                if (clientData.ParentGroupID.HasValue)
                {
                    var mainGroup = allGroups.FirstOrDefault(x => x.IsMainGroupInInstallation);
                    Try.NotNull(mainGroup, "Main group not found;", false);

                    editedGroup.ParentGroupID = (clientData.ParentGroupID.Value == 0 || clientData.ParentGroupID.Value == mainGroup.GroupID)
                        ? null : clientData.ParentGroupID;
                }

                if (clientData.GroupIDs != null)
                    SetGroups(allGroups, editedGroup, clientData.GroupIDs);

                if (clientData.PowerBarSNs != null)
                    SetPowerBars(allPowerBars, editedGroup, clientData.PowerBarSNs);
            }
        }

        public GroupItem Create(long installationID, GroupClientData clientData)
        {
            Try.Argument(clientData, nameof(clientData));

            using (var transaction = Context.Data.Database.BeginTransaction())
            {
                var groups = Context.Data.GetGroups(installationID);
                var powerBars = Context.Data.GetPowerBars(installationID, true);

                var installatioin = Context.Data.Installations.Find(installationID);
                var newGroup = new Group(installatioin);

                SetGroupProperties(groups, powerBars, newGroup, clientData);

                Context.Data.Groups.Add(newGroup);
                groups.Add(newGroup);
                var newGroupId = newGroup.GroupID;

                var hierarchy = new TopGroup(groups);
                hierarchy.CheckCycles();
                hierarchy.AddPowerBars(powerBars);

                var newGroupItem = hierarchy.GetGroupById(newGroupId);
                Try.NotNull(newGroupItem, "Cant find new group in hierarchy;", false);

                hierarchy.UpdateStatuses();

                Context.Data.SaveChanges();
                transaction.Commit();

                Context.Events.GroupChange(installationID, newGroupItem.GroupID, EventActionType.Create);

                return newGroupItem;
            }
        }

        public GroupItem Edit(long installationID, int groupID, GroupClientData clientData)
        {
            Try.Argument(clientData, nameof(clientData));

            using (var transaction = Context.Data.Database.BeginTransaction())
            {
                var groups = Context.Data.GetGroups(installationID);
                var powerBars = Context.Data.GetPowerBars(installationID, true);

                var editedGroup = groups.FirstOrDefault(x => x.GroupID == groupID);
                Try.NotNull(editedGroup, $"Cant find group with ID: {groupID};");

                SetGroupProperties(groups, powerBars, editedGroup, clientData);

                var editedHierarchy = new TopGroup(groups);
                editedHierarchy.CheckCycles();
                editedHierarchy.AddPowerBars(powerBars);

                var editedGroupItem = editedHierarchy.GetGroupById(editedGroup.GroupID);
                Try.NotNull(editedGroupItem, $"Cant find group with ID: {editedGroup.GroupID} in hierarchy;", false);

                editedHierarchy.UpdateStatuses();

                Context.Data.SaveChanges();
                transaction.Commit();

                Context.Events.GroupChange(installationID, editedGroupItem.GroupID, EventActionType.Edit);

                return editedGroupItem;
            }
        }

        public void Delete(long installationID, int groupID)
        {
            using (var transaction = Context.Data.Database.BeginTransaction())
            {
                var groups = Context.Data.GetGroups(installationID);
                var powerBars = Context.Data.GetPowerBars(installationID, true);

                var toDeleteGroup = groups.FirstOrDefault(x => x.GroupID == groupID);
                Try.NotNull(toDeleteGroup, $"Cant find group with ID: {groupID};");

                foreach (var group in groups)
                    if (group.ParentGroupID == toDeleteGroup.GroupID)
                        group.ParentGroupID = null;

                foreach (var powerBar in powerBars)
                    powerBar.Groups.RemoveAll(x => x.GroupID == toDeleteGroup.GroupID);

                Context.Data.SaveChanges();

                Context.Data.Groups.Remove(Context.Data.Groups.Find(toDeleteGroup.GroupID));

                Context.Data.SaveChanges();
                transaction.Commit();

                Context.Events.GroupChange(installationID, groupID, EventActionType.Delete);
            }
        }

    }

}