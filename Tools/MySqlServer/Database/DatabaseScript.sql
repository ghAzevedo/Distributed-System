
CREATE DATABASE IF NOT EXISTS `retailinmotion`;
USE `retailinmotion`;
CREATE USER 'retailinmotion' IDENTIFIED BY 'retailinmotion';
GRANT ALL PRIVILEGES ON * . * TO 'retailinmotion';

CREATE TABLE `product` (
	`productid` VARCHAR(36) NOT NULL,
	`name` VARCHAR(50) NOT NULL,
	`totalstock` INT NOT NULL,
	PRIMARY KEY (`productid`)
);

CREATE TABLE `order` (
	`orderid` VARCHAR(36) NOT NULL,
	`deleted` BOOLEAN NULL,
	`creationtimeutc` DATETIME NOT NULL,
	PRIMARY KEY (`orderid`)
);

CREATE TABLE `orderproducts` (
	`orderid` VARCHAR(36) NOT NULL,
	`productid` VARCHAR(36) NOT NULL,
	`quantity` INT NULL,
	PRIMARY KEY (`orderid`, `productid`)
);

CREATE TABLE `delivery` (
	`deliveryid` VARCHAR(36) NOT NULL,
	`country` VARCHAR(50) NULL,
	`city` VARCHAR(50) NULL,
	`street` VARCHAR(50) NULL,
	`deleted` BOOLEAN NULL,
	PRIMARY KEY (`deliveryid`)
);

INSERT INTO `retailinmotion`.`product` (`productid`, `name`, `totalstock`) VALUES ('2a6b0daf-5bb7-4905-be03-3886b8b9d91a', 'product 1', '20');
INSERT INTO `retailinmotion`.`product` (`productid`, `name`, `totalstock`) VALUES ('38a50377-3780-486f-a83c-51f5add4d394', 'product 2', '20');