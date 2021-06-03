namespace SR.Application.Persistence
{
    public class ProductView
    {
        public long Id { get; set; }
        
        public long BusinessId { get; set; }
        
        public long SupplierId { get; set; }

        public string Name { get; set; } = default!;
        
        public long ProductPropertyId { get; set; }
        
        public long UoMId { get; set; }
        
        public long FolderId { get; set; }
        
        public string Color { get; set; } = default!;
        
        public string Size { get; set; } = default!;
    }
}

// CREATE VIEW public."ProductView" as
//     select 
// p."Id",
// p."BusinessId",
// p."SupplierId",
// p."Name",
// pp."Id" as "ProductPropertyId",
// pp."UoMId",
// pp."FolderId",
// pp."Color",
// pp."Size" 
// from "Products" p 
//     join "ProductProperties" pp 
//     on p."Id" = pp."ProductId";