namespace MireaNFCProjectAPI.Models
{
    public class Lesson
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public string GroupId { get; set; }
        public short RoomId { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public System.DateTime FinishDateTime { get; set; }
    }
}
