using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WispCloud.Logic
{
    public abstract class BaseGroupItem
    {
        [JsonIgnore]
        public BaseGroupItem Parent { get; set; }

        [JsonIgnore]
        public bool HasStatus { get; private set; }

        public int? ModeID { get; private set; }
        public int? MaxShading { get; private set; }
        public int? MinShading { get; set; }
        public int? MaxUserBrightness { get; private set; }
        public int? MinUserBrightness { get; private set; }
        public int? MaxRealBrightness { get; private set; }
        public int? MinRealBrightness { get; private set; }

        public List<GroupItem> Groups { get; set; }

        [JsonIgnore]
        public List<PowerBarItem> PowerBars { get; set; }

        public IEnumerable<decimal> PowerBarSNs
        {
            get
            {
                if (PowerBars == null)
                    return null;

                return PowerBars.Select(x => x.PowerBarSN);
            }
        }

        public BaseGroupItem()
        {
            ClearStatus();
        }

        public void ClearStatus()
        {
            ModeID = null;

            MaxShading = null;
            MinShading = null;

            MaxRealBrightness = null;
            MinRealBrightness = null;

            MaxUserBrightness = null;
            MinRealBrightness = null;

            HasStatus = false;
        }

        public void UpdateStatus()
        {
            ClearStatus();
            bool isStatusInited = false;

            if (Groups != null && Groups.Any())
            {
                foreach (var childGroupWithStatus in Groups.Where(x => x.HasStatus))
                {
                    if (isStatusInited)
                    {
                        UpdateStatusFromGroup(childGroupWithStatus);
                    }
                    else
                    {
                        InitStatusFromGroup(childGroupWithStatus);
                        isStatusInited = true;
                    }
                    HasStatus = true;
                }
            }

            if (PowerBars != null && PowerBars.Any())
            {
                if (!isStatusInited)
                {
                    InitStatusFromPowerBar(PowerBars.First());
                    isStatusInited = true;
                }

                foreach (var childPowerBar in PowerBars)
                    UpdateStatusFromPowerBar(childPowerBar);

                HasStatus = true;
            }
        }

        void InitStatusFromGroup(BaseGroupItem group)
        {
            ModeID = group.ModeID;

            MaxShading = group.MaxShading;
            MinShading = group.MinShading;

            MaxRealBrightness = group.MaxRealBrightness;
            MinRealBrightness = group.MinRealBrightness;

            MaxUserBrightness = group.MaxUserBrightness;
            MinUserBrightness = group.MinUserBrightness;
        }

        void InitStatusFromPowerBar(PowerBarItem powerBar)
        {
            ModeID = powerBar.ModeID;

            MaxShading = powerBar.Shading;
            MinShading = powerBar.Shading;

            MaxRealBrightness = powerBar.RealBrightness;
            MinRealBrightness = powerBar.RealBrightness;

            MaxUserBrightness = powerBar.UserBrightness;
            MinUserBrightness = powerBar.UserBrightness;
        }

        void UpdateStatusFromGroup(BaseGroupItem group)
        {
            if (group.MaxShading > MaxShading)
                MaxShading = group.MaxShading;
            if (group.MinShading < MinShading)
                MinShading = group.MinShading;

            if (group.MaxRealBrightness > MaxRealBrightness)
                MaxRealBrightness = group.MaxRealBrightness;
            if (group.MinRealBrightness < MinRealBrightness)
                MinRealBrightness = group.MinRealBrightness;

            if (group.MaxUserBrightness > MaxUserBrightness)
                MaxUserBrightness = group.MaxUserBrightness;
            if (group.MinUserBrightness < MinUserBrightness)
                MinUserBrightness = group.MinUserBrightness;

            if (ModeID != group.ModeID)
                ModeID = -1;
        }

        void UpdateStatusFromPowerBar(PowerBarItem powerBar)
        {
            if (powerBar.Shading > MaxShading)
                MaxShading = powerBar.Shading;
            if (powerBar.Shading < MinShading)
                MinShading = powerBar.Shading;

            if (powerBar.RealBrightness > MaxRealBrightness)
                MaxRealBrightness = powerBar.RealBrightness;
            if (powerBar.RealBrightness < MinRealBrightness)
                MinRealBrightness = powerBar.RealBrightness;

            if (powerBar.UserBrightness > MaxUserBrightness)
                MaxUserBrightness = powerBar.UserBrightness;
            if (powerBar.UserBrightness < MinUserBrightness)
                MinUserBrightness = powerBar.UserBrightness;

            if (ModeID != powerBar.ModeID)
                ModeID = -1;
        }

        public IEnumerable<BaseGroupItem> GetAllGroups()
        {
            if (Groups == null)
                yield break;

            var itemsTreeQueue = new Queue<BaseGroupItem>();
            itemsTreeQueue.Enqueue(this);

            while (itemsTreeQueue.Any())
            {
                var item = itemsTreeQueue.Dequeue();
                yield return item;

                if (item.Groups == null || !item.Groups.Any())
                    continue;

                foreach (var itemChild in item.Groups)
                    itemsTreeQueue.Enqueue(itemChild);
            }
        }

    }

}