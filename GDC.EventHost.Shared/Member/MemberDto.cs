using GDC.EventHost.Shared.ShoppingCart;

namespace GDC.EventHost.Shared.Member
{
    public class MemberDto
    {
        public Guid Id { get; set; }

        public required string UserId { get; set; }

        public required string Username { get; set; }

        public required string FullName { get; set; }

        public required string EmailAddress { get; set; } 

        public bool IsActive { get; set; } = false;

        public ShoppingCartDto? ShoppingCart { get; set; }
    }
}