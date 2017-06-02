namespace WispCloudClient.ApiTypes
{
    public sealed class WindowItem
    {
        public int WindowID { get; set; }
        public int? OrderID { get; set; }
        public string PowerBarSN { get; set; }
        public string Description { get; set; }
        public int SequenceNumber { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float DiagonalLTRB { get; set; }
        public float DiagonalRTLB { get; set; }
        public BarLocation BarLocation { get; set; }
        public WindowItemStatus Status { get; set; }

    }

}