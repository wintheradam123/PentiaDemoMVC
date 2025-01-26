namespace PentiaDemoMVC.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderName { get; set; }
        public int OrderPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public int SalespersonId { get; set; }
    }
}
