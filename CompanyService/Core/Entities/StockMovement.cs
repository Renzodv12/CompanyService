using CompanyService.Core.Enums;

namespace CompanyService.Core.Entities
{
    public class StockMovement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public MovementType Type { get; set; } // Entrada, Salida, Ajuste
        public int Quantity { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public string Reason { get; set; }
        public string Reference { get; set; } // Referencia a venta, compra, etc.
        public Guid UserId { get; set; } // Usuario que realizó el movimiento
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Product Product { get; set; }
    }
}
