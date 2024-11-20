IF EXISTS (SELECT * FROM sys.databases WHERE name = 'NolowaDatabase')
BEGIN
    PRINT 'Database NolowaDatabase already exists.';
    RETURN;
END

CREATE DATABASE [NolowaDatabase];
GO

USE [NolowaDatabase];

-- Create ProfileImage Table
CREATE TABLE ProfileImage (
    id BIGINT PRIMARY KEY IDENTITY,
    fileHash VARCHAR(255) NOT NULL,
    url VARCHAR(255) NOT NULL
);

-- Create ProfileInfo Table
CREATE TABLE ProfileInfo (
    id BIGINT PRIMARY KEY IDENTITY,
    profileImgId BIGINT,
    backgroundImgId BIGINT,
    message VARCHAR(MAX),
    FOREIGN KEY (profileImgId) REFERENCES ProfileImage(id),
    FOREIGN KEY (backgroundImgId) REFERENCES ProfileImage(id)
);

-- Create Account Table
CREATE TABLE Account (
    id BIGINT PRIMARY KEY IDENTITY,
    userId VARCHAR(255) NOT NULL,
    accountName VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    passwordHash VARBINARY(MAX) NOT NULL,
    passwordSalt VARBINARY(MAX) NOT NULL,
    profileInfoId BIGINT,
    FOREIGN KEY (profileInfoId) REFERENCES ProfileInfo(id)
);

-- Create Follower Table
CREATE TABLE Follower (
    id BIGINT PRIMARY KEY IDENTITY,
    followerId BIGINT NOT NULL,
    followeeId BIGINT NOT NULL,
    FOREIGN KEY (followerId) REFERENCES Account(id),
    FOREIGN KEY (followeeId) REFERENCES Account(id)
);

-- Create SearchHistory Table
CREATE TABLE SearchHistory (
    id BIGINT PRIMARY KEY IDENTITY,
    accountId BIGINT NOT NULL,
    keyword VARCHAR(255) NOT NULL,
    FOREIGN KEY (accountId) REFERENCES Account(id)
);

-- Create Post Table
CREATE TABLE Post (
    id BIGINT PRIMARY KEY IDENTITY,
    accountId BIGINT NOT NULL,
    message VARCHAR(MAX),
    insertTime DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (accountId) REFERENCES Account(id)
);

-- Create DirectMessage Table
CREATE TABLE DirectMessage (
    id BIGINT PRIMARY KEY IDENTITY,
    senderId BIGINT NOT NULL,
    receiverId BIGINT NOT NULL,
    message VARCHAR(MAX),
    insetTime DATETIME NOT NULL DEFAULT GETDATE(),
    isRead BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (senderId) REFERENCES Account(id),
    FOREIGN KEY (receiverId) REFERENCES Account(id)
);

GO

-- 로그 남기기
PRINT 'Database and table creation script executed successfully.';