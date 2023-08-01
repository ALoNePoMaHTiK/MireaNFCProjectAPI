namespace MireaNFCProjectAPI.Models
{
    public class Checkout
    {
        public Guid CheckoutId { get; set; }
        public System.DateTime CheckoutDateTime { get; set; }
        public string TagId { get; set; }
        public string StudentId { get; set; }
        public string EmployeId { get; set; }
    }
}
