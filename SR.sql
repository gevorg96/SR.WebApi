create table Business (
    Id SERIAL PRIMARY KEY,
    Name CHARACTER VARYING(100) NOT NULL,
    Tel bigint
);

create table UserProfile (
    Id SERIAL PRIMARY KEY,
    Login CHARACTER VARYING(30) NOT NULL,
    FirstName CHARACTER VARYING(30) NOT NULL,
    LastName CHARACTER VARYING(30) NOT NULL,
    Email CHARACTER VARYING(100) NOT NULL,
    Password CHARACTER VARYING(1000) NOT NULL,
    BusinessId INTEGER REFERENCES Business (Id) NOT NULL
);

create table Shop (
    Id SERIAL PRIMARY KEY,
    Name CHARACTER VARYING(100) NOT NULL,
    Address CHARACTER VARYING(500) NOT NULL,
    BusinessId INTEGER REFERENCES Business (Id) NOT NULL
);

create table Unit (
    Id INTEGER PRIMARY KEY,
    Name CHARACTER VARYING(100) NOT NULL
);

create table Supplier (
    Id SERIAL PRIMARY KEY,
    Name CHARACTER VARYING(100) NOT NULL,
    Address CHARACTER VARYING(500) NOT NULL,
    OrgName CHARACTER VARYING(100) NOT NULL,
    Tel bigint
);

create table Cost (
    Id SERIAL PRIMARY KEY,
    ShopId INTEGER REFERENCES Business (Id) NOT NULL,
    Value money NOT NULL 
);

create table Price (
    Id SERIAL PRIMARY KEY,
    ShopId INTEGER REFERENCES Business (Id) NOT NULL,
    Value money NOT NULL
);

create table Folder (
    Id SERIAL PRIMARY KEY,
    Name CHARACTER VARYING(100) NOT NULL,
    ParentId INTEGER REFERENCES Folder (Id) NOT NULL,
    ShopId INTEGER REFERENCES Business (Id) NOT NULL
);

create table Product (
    Id SERIAL PRIMARY KEY,
    Name CHARACTER VARYING(100) NOT NULL,
    FolderId INTEGER REFERENCES Folder (Id) NOT NULL,
    Attributes jsonb
);

create table ProductShop (
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    ShopId INTEGER REFERENCES Shop (Id) NOT NULL,
    PRIMARY KEY (ProductId, ShopId)
);
create table ProductSupplier (
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    SupplierId INTEGER REFERENCES Supplier (Id) NOT NULL,
    PRIMARY KEY (ProductId, SupplierId)
);

create table ProductCost (
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    CostId INTEGER REFERENCES Cost (Id) NOT NULL,
    PRIMARY KEY (ProductId, CostId)
);

create table ProductPrice (
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    PriceId INTEGER REFERENCES Price (Id) NOT NULL,
    PRIMARY KEY (ProductId, PriceId)
);

create table ProductUnit (
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    UnitId INTEGER REFERENCES Unit (Id) NOT NULL,
    PRIMARY KEY (ProductId, UnitId)
);

create table ExpenseType (
    Id INTEGER PRIMARY KEY,
    Name CHARACTER VARYING(100) NOT NULL
);

create table Expense (
    Id INTEGER PRIMARY KEY,
    Sum MONEY NOT NULL,
    BusinessId INTEGER REFERENCES Business (Id) NOT NULL,
    ShopId INTEGER REFERENCES Shop (Id) NOT NULL,
    ReportDate timestamp with time zone  NOT NULL
);

create table ExpenseDetail (
    Id INTEGER PRIMARY KEY,
    ExpenseTypeId INTEGER REFERENCES ExpenseType (Id) NOT NULL,
    Sum MONEY NOT NULL
);

insert into expensetype (Id, Name)
values (1, 'Оплата труда'),
       (2, 'Доставка'),
       (3, 'Реклама'),
       (4, 'Коммунальные платежи'),
       (5, 'Налоги и сборы'),
       (6, 'Аренда'),
       (7, 'Прочее');

create table Bill (
    Id INTEGER PRIMARY KEY,
    ShopId INTEGER REFERENCES Shop (Id) NOT NULL,
    Sum MONEY NOT NULL,
    Number INTEGER NOT NULL,
    ReportDate timestamp with time zone  NOT NULL
);

create table BillDetail (
    Id INTEGER PRIMARY KEY,
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    BillId INTEGER REFERENCES Bill (Id) NOT NULL,
    Sum MONEY NOT NULL,
    Count NUMERIC(10, 2) NOT NULL,
    UnitId INTEGER REFERENCES Unit (Id) NOT NULL,
    CostId INTEGER REFERENCES Cost (Id) NOT NULL,
    Price MONEY NOT NULL,
    Profit MONEY NOT NULL
);

create table Stock (
    Id INTEGER PRIMARY KEY,
    ShopId INTEGER REFERENCES Shop (Id) NOT NULL,
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    Count NUMERIC(10, 2) NOT NULL
);

create table Invoice (
    Id INTEGER PRIMARY KEY,
    ShopId INTEGER REFERENCES Shop (Id) NOT NULL,
    ReportDate timestamp with time zone  NOT NULL,
    IsInvoice BOOLEAN NOT NULL
);

create table InvoiceDetail (
    Id INTEGER PRIMARY KEY,
    InvoiceId INTEGER REFERENCES Invoice (Id) NOT NULL,
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    CostId  INTEGER REFERENCES Cost (Id) NOT NULL,
    Count NUMERIC(10, 2) NOT NULL
);

create table InvoiceStock (
    Id INTEGER PRIMARY KEY,
    InvoiceId INTEGER REFERENCES Invoice (Id) NOT NULL,
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    ShopId INTEGER REFERENCES Shop (Id) NOT NULL,
    CurrStock NUMERIC(10, 2) NULL
);
       
create table Image (
    ProductId INTEGER REFERENCES Product (Id) NOT NULL,
    Type CHARACTER VARYING(255) NOT NULL,
    Name CHARACTER VARYING(255) NOT NULL,
    Url CHARACTER VARYING(4000) NOT NULL,
    UrlTemp CHARACTER VARYING(4000) NOT NULL,
    Path CHARACTER VARYING(255) NOT NULL
);
---------

--ACL

create table Role (
    Id INTEGER PRIMARY KEY,
    Name CHARACTER VARYING(100) NOT NULL
);

create table Permission (
    Id INTEGER PRIMARY KEY,
    Name CHARACTER VARYING(255) NOT NULL
);

create table RolePermission (
    RoleId INTEGER REFERENCES Role (Id) NOT NULL,
    PermissionId INTEGER REFERENCES Permission (Id) NOT NULL
);

create table UserRole (
    UserId INTEGER REFERENCES UserProfile (Id) NOT NULL,
    RoleId INTEGER REFERENCES Role (Id) NOT NULL,
    Context jsonb NULL
);

------



