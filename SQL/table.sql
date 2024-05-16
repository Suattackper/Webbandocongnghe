create database ECOMMERCEadmin
go
USE ECOMMERCEadmin
GO

create database ECOMMERCE
go
USE ECOMMERCE
GO

-- QUYỀN
CREATE TABLE Roles (
	RoleID int identity(1,1) primary key,
	RoleName nvarchar(50)
)

-- TÀI KHOẢN 
CREATE TABLE Account (
	AccountCode int identity(1,1) primary key,
	Password varchar(255) COLLATE SQL_Latin1_General_CP1_CS_AS null,
	Email varchar(30) COLLATE SQL_Latin1_General_CP1_CS_AS null,
	PhoneNumber varchar(10) null,
	FirstName nvarchar(50) COLLATE SQL_Latin1_General_CP1_CS_AS null,
	LastName nvarchar(50) COLLATE SQL_Latin1_General_CP1_CS_AS null,
	Avatar image,
	-- 23/12/23 Nhu : Sửa độ dài của giá trị 
	RequestCode nvarchar(512) null,
	CreateAt datetime default getdate() null,
	RoleID int,
	-- Anh Tan 23/12/ 16:18 xóa cột status
	-- Huynh nhu 23/12 2:57 PM: Thêm cột 
	Update_By nvarchar(256),
	-- Anh Tan 23/12/ 16:18 xóa default getdate
	Update_At datetime
);

--ALTER TABLE Account ADD FOREIGN KEY (RoleID) REFERENCES Roles (RoleID)
--ALTER TABLE Account ADD Update_By nvarchar(256)
--ALTER TABLE Account ADD Update_At datetime default getdate() null
--ALTER TABLE Account ALTER COLUMN RequestCode nvarchar(512)
--ALTER TABLE Account DROP COLUMN Birthday xóa cột birthday 23/12


-- ĐỊA CHỈ CỦA TÀI KHOẢN
CREATE TABLE AccountAddress (
	AccountAddressCode int identity(1,1) primary key,
	AccountCode int null,
	--first name + last name
	FullName nvarchar(50) COLLATE SQL_Latin1_General_CP1_CS_AS null,
	PhoneNumber varchar(10) null,
	-- mã bưu chính
	PostalCode varchar(20) null,
	Country nvarchar(50) null,
	City nvarchar(50) null,
	Address nvarchar(50) null,
	foreign key (AccountCode) references Account(AccountCode)
);

-- THƯƠNG HIỆU 
CREATE TABLE Brand(
	BrandCode int identity(1,1) primary key,
	BrandName nvarchar(100), 
	Origin nvarchar(50)
)

-- DANH MỤC - LOẠI HÀNG HÓA 
CREATE TABLE Category (
	CategoryCode int identity(1,1) primary key,
	CategoryName nvarchar(100)
)

-- KHUYẾN MÃI
CREATE TABLE Promotion (
	PromotionCode char(10) primary key,
	-- Phần trăm khuyến mãi 
	PromotionPercentage int null,
	EndDate datetime null,
	StartDate datetime null,
	Quantity int default 1000 null
)

-- Nhu: 21/12
--ALTER TABLE Promotion ADD Quantity int default 1000 null;
--ALTER TABLE Promotion ADD StartDate datetime null;


-- SẢN PHẨM 
CREATE TABLE Product(
	ProductCode varchar(15) primary key,
	ProductName nvarchar(50),
	BrandCode int null,
	CategoryCode int null,
	ImageProduct image,
	Price decimal(18,2),
	PromotionCode char(10) null,
	-- Số lượng còn lại 
	Quantity int default 1000,
	-- Mô tả 
	Description nvarchar(max) null,
	-- Lượt xem 
	ViewCount int  default 0,
	Rate float,
	foreign key (CategoryCode) references Category(CategoryCode),
	foreign key (PromotionCode) references Promotion(PromotionCode),
	-- Anh Tan 23/12/ 18:10 add khóa ngoại brand
	foreign key (BrandCode) references Brand(BrandCode)
)

-- HUYNH NHU

-- HÌNH ẢNH SẢN PHẨM
CREATE TABLE ProductImg (
	ProductImgCode int identity(1,1) primary key,
	Img image null,
	ProductCode varchar(15) null,
	foreign key (ProductCode) references Product(ProductCode)
)



-- GIAO HÀNG
CREATE TABLE Delivery (
	DeliveryCode varchar(10) primary key,
	DeliveryName nvarchar(50) null,
	PriceTotal money null,
	-- Phí giao hàng
	DeliveryFee money null,
	Status bit default 0 null
);

-- PHƯƠNG THỨC THANH TOÁN 
CREATE TABLE Payment (
	PaymentCode int identity(1,1) primary key,
	--AccountCode int,
	PaymentType varchar(20) null,
	-- So tai khoan
	AccountNumer varchar(20) null,
	ExpiryDate date null,
	--foreign key (AccountCode) references Account(AccountCode)
)

-- ĐẶT HÀNG 
CREATE TABLE Orders(
	OrderCode int identity(1,1) primary key,
	PaymentCode int null,
	AccountCode int null,
	--Mã giảm giá
	PromotionCode char(10) null,
	-- Ngày đặt hàng 
	OrderDate date default getdate() null,
	-- Đã giao hay chưa 1-yes 0-no
	Delivered bit default 0 null,
	DeliveryDate date null,
	OrderTotal money null,
	-- Ghi chu don hang
	OrderNote nvarchar(100),
	AccountAddressCode int null,
	foreign key (PaymentCode) references Payment(PaymentCode),
	foreign key (AccountCode) references Account(AccountCode),
	foreign key (AccountAddressCode) references AccountAddress(AccountAddressCode),
	foreign key (PromotionCode) references Promotion(PromotionCode)
)

-- CHI TIẾT ĐẶT HÀNG
CREATE TABLE OrderDetail (
	OrderCode int,
	ProductCode varchar(15),
	Price money null,
	Quantity int null,
	Total money null,
	foreign key (ProductCode) references Product(ProductCode),
	primary key(ProductCode,OrderCode)
)


-- Huynh nhu 22/12 11:08 PM

ALTER TABLE OrderDetail ADD FOREIGN KEY (OrderCode) REFERENCES Orders (OrderCode)

-- COMMENT
CREATE TABLE Comment (
	CommentCode int identity(1,1) primary key,
	AccountCode int null,
	ProductCode varchar(15) null,
	CommentContent nvarchar(max) null,
	CommentTime smalldatetime default getdate() null,
	Rate tinyint default 0 null,
	foreign key (AccountCode) references Account(AccountCode),
	foreign key (ProductCode) references Product(ProductCode)
)
)

-- LIÊN HỆ
CREATE TABLE Contact (
	ContactCode int identity(1,1) primary key,
	FullName varchar(45) COLLATE SQL_Latin1_General_CP1_CS_AS null,
	Email varchar(30) COLLATE SQL_Latin1_General_CP1_CS_AS null,
	-- Anh Tan 25/12/ 00:13 add ContactDate, status, xóa phonenumber
	Status bit default 0 null,
	ContactDate datetime default getdate() null,
	Message varchar(1000) null,
	AccountCode int null,
	foreign key (AccountCode) references Account(AccountCode)
)


INSERT INTO Roles (RoleName) VALUES (N'Quản lý')
INSERT INTO Roles (RoleName) VALUES (N'Nhân viên')
INSERT INTO Roles (RoleName) VALUES (N'Khách hàng')

---- BẢNG QUẢN LÝ MENU 
--CREATE TABLE Menu (
--Code int identity(1,1),
--Name nvarchar(50) null,
--Link nvarchar(250) null,
--Target nvarchar(50) null,
--Position int null
--)


---- THÊM KHÓA NGOẠI 
--ALTER TABLE AccountAddress ADD FOREIGN KEY (AccountCode) REFERENCES Account (AccountCode)
--ALTER TABLE Product ADD FOREIGN KEY (BrandCode) REFERENCES Brand (BrandCode)
--ALTER TABLE Product ADD FOREIGN KEY (CategoryCode) REFERENCES Category (CategoryCode)
--ALTER TABLE Product ADD FOREIGN KEY (PromotionCode) REFERENCES Promotion (PromotionCode)
--ALTER TABLE ProductImg ADD FOREIGN KEY (ProductCode) REFERENCES Product (ProductCode)
--ALTER TABLE Orders ADD FOREIGN KEY (AccountCode) REFERENCES Account (AccountCode)
--ALTER TABLE Orders ADD FOREIGN KEY (PaymentCode) REFERENCES Payment (PaymentCode)
--ALTER TABLE Orders ADD FOREIGN KEY (PromotionCode) REFERENCES Promotion (PromotionCode)
--ALTER TABLE Orders ADD FOREIGN KEY (OrderAddressCode) REFERENCES AccountAddress (AccountAddressCode)

--ALTER TABLE OrderDetail ADD FOREIGN KEY (ProductCode) REFERENCES Product (ProductCode)
--ALTER TABLE Comment ADD FOREIGN KEY (AccountCode) REFERENCES Account (AccountCode)
--ALTER TABLE Comment ADD FOREIGN KEY (ProductCode) REFERENCES Product (ProductCode)
--ALTER TABLE Contact ADD FOREIGN KEY (AccountCode) REFERENCES Account (AccountCode)
--ALTER TABLE CustomerPaymentMethod ADD FOREIGN KEY (AccountCode) REFERENCES Account (AccountCode)

---- INDEX	
--CREATE NONCLUSTERED INDEX idx_BrandCode ON Product (BrandCode ASC)
--CREATE NONCLUSTERED INDEX idx_CategoryCode ON Product (CategoryCode ASC)


select * from Account