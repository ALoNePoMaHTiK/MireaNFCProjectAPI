namespace MireaNFCProjectAPI.Models
{
    public class Employe
    {
        public string EmployeId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
        public byte RoleId { get; set; }
        public short DepartamentId { get; set; }
    }
}
