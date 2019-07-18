namespace SmartRetail.App.DAL.Entities
{
    public sealed class UserProfile : IEntity
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? shop_id { get; set; }
        public int? business_id { get; set; }
        public int? access_grade { get; set; }
        public Business Business { get; set; }
    }
}
