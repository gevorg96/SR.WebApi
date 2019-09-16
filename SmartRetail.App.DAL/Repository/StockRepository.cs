using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.UnitOfWork;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class StockRepository: IStockRepository
    {
        private readonly string conn;
        private readonly IUnitOfWork _unitOfWork;
        public StockRepository(string connectionString)
        {
            conn = connectionString;
        }

        public StockRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> InsertUow(Stock stock)
        {
            return await _unitOfWork.Connection.InsertAsync(stock, transaction: _unitOfWork.Transaction);
        }

        public async Task<bool> UpdateUow(Stock stock)
        {
            return await _unitOfWork.Connection.UpdateAsync(stock, transaction: _unitOfWork.Transaction);
        }

        public async Task<Stock> GetStockByShopAndProdIdsUow(int shopId, int prodId)
        {
            var sql = "select * from Stocks where shop_id = " + shopId + " and prod_id = " + prodId;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Stock>(sql, transaction:_unitOfWork.Transaction);
        }

        #region Read

        public IEnumerable<Stock> GetStocksWithProductsByBusiness(int businessId)
        {
            var sql = "select * from Shops where business_id = @businessId";
            var subSql = "select * from Stocks where shop_id = @shopId";
            var sub2Sql = "select * from Products where id = @prodId";
            var sub3Sql = "select * from Images where prod_id = @prodId";
            
            using (var connection = new SqlConnection(conn))
            {
                connection.Open();
                var shops = connection.Query<Shop>(sql, new {businessId = businessId});
                var stocks = new List<Stock>();
                foreach (var shop in shops)
                {
                    var stocksList = connection.Query<Stock>(subSql, new { shopId = shop.id }).ToList();
                    foreach (var stock in stocksList)
                    {
                        stock.Product = connection.QueryFirstOrDefault<Product>(sub2Sql, new {prodId = stock.prod_id});
                        if (stock.Product != null)
                            stock.Product.Image = connection.QueryFirstOrDefault<Image>(sub3Sql, new { prodId = stock.prod_id });
                    }
                    stocks.AddRange(stocksList);
                }
                return stocks;
            }
        }

        public Stock GetStockByShopAndProdIds(int shopId, int prodId)
        {
            var sql = "select * from Stocks where shop_id = " + shopId + " and prod_id = " + prodId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return db.Query<Stock>(sql).FirstOrDefault();
            }
        }
        
        public async Task<IEnumerable<Stock>> GetStocksWithProducts(int shopId)
        {
            var sql = "select * from Stocks where shop_id = @shopId";
            var subSql = "select * from Products where id = @prodId";
            var sub2Sql = "select * from Images where prod_id = @prodId";

            using (var connection = new SqlConnection(conn))
            {
                connection.Open();
                var stocks = await connection.QueryAsync<Stock>(sql, new { shopId = shopId });

                foreach (var stock in stocks)
                {
                    stock.Product = await connection.QueryFirstOrDefaultAsync<Product>(subSql, new { prodId = stock.prod_id });
                    if (stock.Product != null)
                    {
                        stock.Product.Image = await connection.QueryFirstOrDefaultAsync<Image>(sub2Sql, new { prodId = stock.prod_id });
                    }
                }
                return stocks;
            }
        }

        #endregion

        #region Create

        public async Task AddAsync(Stock entity)
        {
            var sql = "INSERT INTO Stocks (shop_id, prod_id, count) values(" + isNotNull(entity.shop_id) + ", " +
                      entity.prod_id +
                      ", " + isNotNull(entity.count) + ")";
          
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }
        }

        #endregion

        #region Update

        public async Task UpdateValueAsync(Stock entity)
        {
            var sql = "update Stocks set count = " + entity.count + " where id = " + entity.id;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }
        }

        #endregion
    }
}