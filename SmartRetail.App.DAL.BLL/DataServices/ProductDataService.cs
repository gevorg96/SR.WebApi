using System;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.UnitOfWork;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class ProductDataService
    {
        private IProductRepository _productRepository;
        private IPriceRepository _priceRepository;
        private IPictureWareHouse _dbBase;
        private IBusinessRepository _businessRepository;
        private IImageRepository _imageRepository;
        private IStrategy _strategy;
        private IOrdersRepository _ordersRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private ICostRepository _costRepository;

        public ProductDataService(IPictureWareHouse dbBase)
        {
            _dbBase = dbBase;
        }

        public async Task<Product> GetById(int id)
        {
            using (var session = new Session())
            {   
                var uow = session.UnitOfWork;
                _productRepository = new ProductRepository(uow);
                uow.Begin();
                try
                {
                    var res = await _productRepository.GetByIdUow(id);
                    uow.Commit();
                    return res;
                }
                catch (Exception e)
                {
                    uow.RollBack();
                    throw e;
                }
            }
        }

        public async Task<int> Insert(Product product)
        {
            using (var session = new Session())
            {
                var uow = session.UnitOfWork;
                _productRepository = new ProductRepository(uow);
                _priceRepository = new PriceRepository(uow);
                _businessRepository = new BusinessRepository(uow);
                _imageRepository = new ImagesRepository(uow);
                _strategy = new FifoStrategy();
                _ordersRepository = new OrdersRepository(uow);
                _orderDetailRepository = new OrderDetailRepository(uow);
                _costRepository = new CostRepository(uow);
                var imgPath = string.Empty;

                uow.Begin();
                try
                {
                    var productId = await _productRepository.InsertUow(product);
                    if (product.Price != null && product.Price.price.HasValue)
                    {
                        await _priceRepository.InsertUow(new Price {price = product.Price.price, prod_id = productId});
                    }
                    else
                    {
                        await _priceRepository.InsertUow(new Price { prod_id = productId });
                    }

                    if (!string.IsNullOrEmpty(product.ImgBase64) && product.ImgMemoryStream != null)
                    {
                        var business = await _businessRepository.GetByIdUow(product.business_id.Value);
                        var imgParts = product.ImgBase64.Split('.');
                        imgPath = "/products/" + business.id + ". " + business.name + "/" +
                                  productId + "." + product.name + "." + imgParts.Last();
                        var imgUrl = await _dbBase.Upload(product.ImgMemoryStream, imgPath);
                        var img = new Image
                        {
                            img_url = imgUrl,
                            prod_id = productId,
                            img_name = product.name,
                            img_type = imgParts.LastOrDefault(),
                            img_url_temp = MakeTemporary(imgUrl),
                            img_path = product.Category
                        };
                        await _imageRepository.InsertUow(img);
                    }

                    if (product.Orders.Any())
                    {
                        foreach (var order in product.Orders)
                        {
                            var orderId = await _ordersRepository.InsertUow(order);
                            foreach (var orderDetail in order.OrderDetails)
                            {
                                orderDetail.prod_id = productId;
                                orderDetail.order_id = orderId;
                                await _orderDetailRepository.InsertUow(orderDetail);
                            }

                            var orderDal = await _ordersRepository.GetByIdWithMultiUow(orderId);
                            await _strategy.UpdateAverageCostUow(Direction.Order, orderDal, uow);
                        }
                    }
                    else if(product.Cost.Any())
                    {
                        var cost = product.Cost.FirstOrDefault();
                        if (cost != null && cost.value.HasValue)
                        {
                            cost.prod_id = productId;
                            await _costRepository.InsertUow(cost);
                        }
                        else
                        {
                            await _costRepository.InsertUow(new Cost { prod_id = productId });
                        }
                    }

                    uow.Commit();
                    return productId;
                }
                catch (Exception e)
                {
                    uow.RollBack();
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        await _dbBase.Delete(imgPath);
                    }
                    
                    throw e;
                }
            }
        }

        public async Task<bool> Update(Product product)
        {
            using (var session = new Session())
            {
                var uow = session.UnitOfWork;
                _productRepository = new ProductRepository(uow);
                _priceRepository = new PriceRepository(uow);
                _businessRepository = new BusinessRepository(uow);
                _imageRepository = new ImagesRepository(uow);
                _strategy = new FifoStrategy();
                _ordersRepository = new OrdersRepository(uow);
                _orderDetailRepository = new OrderDetailRepository(uow);
                _costRepository = new CostRepository(uow);
                var imgPath = string.Empty;

                uow.Begin();
                try
                {
                    var res = await _productRepository.UpdateUow(product);
                    if (product.Price != null && product.Price.price.HasValue)
                    {
                        await _priceRepository.UpdateUow(product.Price);
                    }
                    if (!string.IsNullOrEmpty(product.ImgBase64) && product.ImgMemoryStream != null)
                    {
                        var business = await _businessRepository.GetByIdUow(product.business_id.Value);
                        var imgParts = product.ImgBase64.Split('.');
                        imgPath = "/products/" + business.id + ". " + business.name + "/" +
                                  product.id + "." + product.name + "." + imgParts.Last();
                        var imgUrl = await _dbBase.Upload(product.ImgMemoryStream, imgPath);
                        Image img = new Image();

                        img.prod_id = product.id;
                        img.img_url = imgUrl;
                        img.img_name = product.name;
                        img.img_type = imgParts.LastOrDefault();
                        img.img_url_temp = MakeTemporary(imgUrl);
                        img.img_path = product.Category;
                        if (product.Image != null)
                        {
                            img.ROWGUID = product.Image.ROWGUID;
                            await _imageRepository.UpdateUow(img);
                        }
                        else
                        {
                            await _imageRepository.InsertUow(img);
                        }
                    }


                    uow.Commit();
                    return res;
                }
                catch (Exception e)
                {
                    uow.RollBack();
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        await _dbBase.Delete(imgPath);
                    }

                    throw e;
                }
            }
        }

        private static string MakeTemporary(string link)
        {
            return link.Replace("https://www", "https://dl").Replace("?dl=0", "?dl=1");
        }
    }
}
