namespace WispCloud.Logic.EventArgs
{
    public sealed class GroupEventArgs : BaseEventArgs
    {
        public long InstallationID { get; set; }
        public int GroupID { get; set; }

    }

}