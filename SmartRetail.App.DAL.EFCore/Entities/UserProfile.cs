namespace SmartRetail.App.DAL.EFCore.Entities
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? shop_id { get; set; }
        public int? business_id { get; set; }
        public int? access_grade { get; set; }
        public virtual Business Business { get; set; }
        public virtual Shop Shop { get; set; }
    }
}