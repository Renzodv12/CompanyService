using CompanyService.Core.Enums;

namespace CompanyService.Core.Models.Product
{

    public class StockMovementDto
    {
        public DateTime Date { get; set; }
        public MovementType Type { get; set; }
        public int Quantity { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public string Reason { get; set; }
        public string Reference { get; set; }
    }
}
