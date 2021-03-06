 CREATE SCHEMA PIZZABOXDB;
USE PIZZABOXDB;
CREATE TABLE USER
(
	Username	CHAR(30)	NOT NULL UNIQUE,
    Pass		CHAR(30)	NOT NULL,
    FullName	CHAR(60)	NOT NULL,	
    SessionLive	TINYINT		NOT NULL, -- Session state
    CONSTRAINT	USER_PK		PRIMARY KEY(Username)
);
CREATE TABLE STORE
(
	StoreID		MEDIUMINT		NOT NULL UNIQUE AUTO_INCREMENT,
    City		CHAR(255)		NOT NULL,
    State		CHAR(2)			NOT NULL,
    Zip			CHAR(10)		NOT NULL,
    CONSTRAINT 	Location 		UNIQUE (City, State),
    CONSTRAINT	STORE_PK		PRIMARY KEY(StoreID)
);
CREATE TABLE INVENTORY
(
	StoreID		MEDIUMINT		NOT NULL UNIQUE,
    Preset		MEDIUMINT		NOT NULL,
    Custom		MEDIUMINT		NOT NULL,
    CONSTRAINT	INV_PK			PRIMARY KEY(StoreID),
    CONSTRAINT	INV_STORE_FK	FOREIGN KEY(StoreID)
	REFERENCES	STORE(StoreID)
    ON UPDATE NO ACTION
    ON DELETE CASCADE
);
CREATE TABLE ORDERS 
(
  OrderID 		MEDIUMINT 		NOT NULL AUTO_INCREMENT,
  StoreID 		MEDIUMINT 		NOT NULL,
  Username 		CHAR(30) 		NOT NULL,
  CONSTRAINT	ORDERS_PK		PRIMARY KEY(OrderID),
  CONSTRAINT	ORDERS_STORE_FK	FOREIGN KEY(StoreID)
  REFERENCES    STORE(StoreID)
  ON UPDATE	CASCADE
  ON DELETE	NO ACTION,
  CONSTRAINT	ORDERS_USER_FK	FOREIGN KEY(Username)
  REFERENCES 	`USER`(Username)
  ON UPDATE CASCADE
  ON DELETE NO ACTION
);
CREATE TABLE ORDERTYPE 
(
  OrderID		MEDIUMINT		 NOT NULL,
  Preset 		CHAR(50) 		 NOT NULL,
  Custom 		CHAR(50) 		 NOT NULL,
  Dt 			VARCHAR(50) 	 NOT NULL,
  Tm 			VARCHAR(50) 	 NOT NULL,
  Cost			DECIMAL(16,2)	 NOT NULL,
  CONSTRAINT	OT_PK			 PRIMARY KEY(OrderID),
  CONSTRAINT	OT_FK			 FOREIGN KEY(OrderID)
  REFERENCES	ORDERS(OrderID)
  ON UPDATE CASCADE
  ON DELETE CASCADE
);