CREATE DATABASE [web]
USE web;

CREATE TABLE roles (
    Id int IDENTITY(1,1) PRIMARY KEY,
    nombre varchar(10)
);

INSERT INTO roles(nombre) VALUES ('admin'),('user');



CREATE TABLE usuario(
	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[username] [varchar](50) NOT NULL,
	[password] [varchar](151) NOT NULL,
	[Passwordhash] [varbinary](100) NULL,
	[passwordsalt] [varbinary](150) NULL,
	[refreshtoken] [varchar](100) NULL,
	[Tokencreated] [datetime] NULL,
	[Tokenexpires] [datetime] NULL,
	[rol] [int] NULL
	Foreign key ([rol])references roles(id)
);

CREATE TABLE ingreso(
	[ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[FECHA] [datetime] NOT NULL,
	[USER_ID] [int] NOT NULL,
	Foreign key ([USER_ID])references usuario(id) ON DELETE CASCADE
);

ALTER TABLE ingreso ADD  CONSTRAINT [DF_ingreso_FECHA]  DEFAULT (getdate()) FOR [FECHA]