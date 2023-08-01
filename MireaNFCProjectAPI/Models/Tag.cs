namespace MireaNFCProjectAPI.Models
{
    public class Tag
    {
        public string TagId { get; set; }
        public DateTime PlacementDateTime { get; set; }
        public short RoomId { get; set; }
        public string Note { get; set; }
    }
}
