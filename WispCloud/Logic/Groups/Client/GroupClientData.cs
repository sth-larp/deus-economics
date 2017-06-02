namespace WispCloud.Logic
{
    public sealed class GroupClientData : BaseModel
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? ParentGroupID { get; set; }
        public int[] GroupIDs { get; set; }
        public decimal[] PowerBarSNs { get; set; }

    }

}