using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.ShoppingCart
{
    public class ShoppingCartForUpdateDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid MemberId { get; set; }

        public List<ShoppingCartItemForUpdateDto> ShoppingCartItems { get; set; } = [];
    }
}
