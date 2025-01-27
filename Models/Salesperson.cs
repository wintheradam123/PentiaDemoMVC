namespace PentiaDemoMVC.Models
{
    public class SalesPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HireDate { get; set; } // TODO: Turn to datetime with proper converter (maybe custom?)
        public int? OrderCount { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}
