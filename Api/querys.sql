CREATE DATABASE [web]
USE web;

CREATE TABLE roles (
    Id int IDENTITY(1,1) PRIMARY KEY,
    nombre varchar(10)
);

INSERT INTO roles(nombre) VALUES ('admin'),('user');

CREATE TABLE ingreso(
	[ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[FECHA] [datetime] NOT NULL,
	[USER_ID] [int] NOT NULL,
);

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
);

ALTER TABLE ingreso ADD  CONSTRAINT [DF_ingreso_FECHA]  DEFAULT (getdate()) FOR [FECHA]

ALTER TABLE ingreso  WITH CHECK ADD  CONSTRAINT [user_id] FOREIGN KEY([USER_ID]) REFERENCES [dbo].[usuario] ([id])
ALTER TABLE usuario  WITH CHECK ADD  CONSTRAINT rol FOREIGN KEY(id) REFERENCES roles (Id);

