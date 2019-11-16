# SR.WebApi

Projects:
  - DAL - Dapper
  - DAL.BLL - UoW in complex operations with Db
  - DAL.DropBox - working with pictures for products
  - Web - RESTful API for manipulation with products, sales, orders, cancellation and expenses


DAL.BLL:
  - FifoStrategy-class implements strategy for recalculation the average cost of product.
  - FoldersDataService-class realize a virtual folder tree and give a functionality for working with them.


Web:
  - Each service realize a wrapper over DataService/Repository and should prepare data for them.
  - ViewModels are the interface for communicating with clients.
