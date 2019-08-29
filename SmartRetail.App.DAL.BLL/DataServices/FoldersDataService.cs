using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.StructureFillers;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class FoldersDataService
    {
        private readonly IFoldersRepository foldersRepo;
        private readonly IProductRepository productRepo;
        private readonly CategoryTreeFiller categoryfiller;

        public FoldersDataService(IFoldersRepository foldersRepo, IProductRepository productRepo)
        {
            this.foldersRepo = foldersRepo;
            this.productRepo = productRepo;
            categoryfiller = new CategoryTreeFiller(foldersRepo);
        }

        public async Task<Tree<Folders>> GetFoldersTreeAsync(int businessId)
        {
            var foldersTask = Task.Run(() => foldersRepo.GetByBusinessAsync(businessId));
            var productsTask = Task.Run(() => productRepo.GetProductsByBusinessAsync(businessId));
            var (folders, products) = await Tasker.WhenAll(foldersTask, productsTask);

            var tree = categoryfiller.CreateTree(folders.ToList(), products.ToList());
            return null;

        }
    }
}
