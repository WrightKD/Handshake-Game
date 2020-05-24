use master;
go
alter database HandshakeDB set single_user with rollback immediate

DROP DATABASE HandshakeDB;
GO

CREATE DATABASE HandshakeDB;
GO





USE [HandshakeDB]
GO


create table dbo.Users(
	UserID int identity(1,1) not null,
	Username varchar(100) not null UNIQUE,
	Email varchar(255) not null,
	Handshakes int default 0 not null,
	IsAdmin bit default 0 not null,
	Latitude float,
	Longitude float,
	constraint PK_Users primary key clustered (
		UserID asc
	)
)
