using MaleFashion.Application.DTOs;
using MaleFashion.Application.Features.Orders.Command;

namespace MaleFashion.Web.Areas.Customer.Models
{
    public class CheckoutViewModel
    {

        public CreateOrderCommand Command { get; set; } = new();


        public CartDto Cart { get; set; } = new();

        public decimal Subtotal => Cart?.SubTotal ?? 0m;
        public decimal ShippingFee => (Subtotal >= 100.00m || Subtotal == 0m) ? 0m : 5.00m;
        public decimal GrandTotal => Subtotal + ShippingFee;
    }
}
