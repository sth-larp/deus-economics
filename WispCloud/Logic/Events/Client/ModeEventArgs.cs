namespace WispCloud.Logic.EventArgs
{
    public sealed class ModeEventArgs : BaseEventArgs
    {
        public long InstallationID { get; set; }
        public int ModeID { get; set; }

    }

}