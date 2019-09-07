/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------

DESKTOP-6H01030\SQLEXPRESS
*/

--insert into [DressBookingDB1].[dbo].[Size] ([Id], [Name]) values (0, 'XS');
--insert into [DressBookingDB1].[dbo].[Size] ([Id], [Name]) values (1, 'S');
--insert into [DressBookingDB1].[dbo].[Size] ([Id], [Name]) values (2, 'M');
--insert into [DressBookingDB1].[dbo].[Size] ([Id], [Name]) values (3, 'L');
--insert into [DressBookingDB1].[dbo].[Size] ([Id], [Name]) values (4, 'XL');
--insert into [DressBookingDB1].[dbo].[Size] ([Id], [Name]) values (5, 'XXL');
--insert into [DressBookingDB1].[dbo].[Size] ([Id], [Name]) values (6, 'XXXL');

--insert into [AspNetRoles] ([Id],[Name]) values ('1', 'Admin');
--insert into [AspNetRoles] ([Id],[Name]) values ('2', 'User');

--INSERT [dbo].[AspNetUsers] (
--[Id], 
--[Email], 
--[EmailConfirmed], 
--[PasswordHash], 
--[SecurityStamp], 
--[PhoneNumber], 
--[PhoneNumberConfirmed], 
--[TwoFactorEnabled], 
--[LockoutEndDateUtc], 
--[LockoutEnabled], 
--[AccessFailedCount], 
--[UserName]) 
--VALUES (N'e434e10f-d36b-4347-bbe8-a88127a3cad8',
--N'admin1@mail.com', 
--0,
--N'AFPjxCaTZqe843G85tU0tn8orDfmC5QYCKkof/jbP1arsxngCk9ZgBC9nSENahRW3w==', 
--N'bfadd533-960e-44f0-8864-fd13fe41acc8', NULL, 0, 0, NULL, 1, 0, N'admin1@mail.com');

--INSERT [dbo].[AspNetUsers] (
--[Id], 
--[Email], 
--[EmailConfirmed], 
--[PasswordHash], 
--[SecurityStamp], 
--[PhoneNumber], 
--[PhoneNumberConfirmed], 
--[TwoFactorEnabled], 
--[LockoutEndDateUtc], 
--[LockoutEnabled], 
--[AccessFailedCount], 
--[UserName]) 
--VALUES (N'54EDBA9A-DDB7-4FCC-8C0C-6CE22FE5FB8F',
--N'user1@mail.com', 
--0,
--N'AFPjxCaTZqe843G85tU0tn8orDfmC5QYCKkof/jbP1arsxngCk9ZgBC9nSENahRW3w==', 
--N'bfadd533-960e-44f0-8864-fd13fe41acc8', NULL, 0, 0, NULL, 1, 0, N'user1@mail.com');

--INSERT INTO [dbo].[AspNetUserRoles]
--           ([UserId]
--           ,[RoleId])
--     VALUES
--           ('e434e10f-d36b-4347-bbe8-a88127a3cad8'
--           ,'1');

--INSERT INTO [dbo].[AspNetUserRoles]
--           ([UserId]
--           ,[RoleId])
--     VALUES
--           ('54EDBA9A-DDB7-4FCC-8C0C-6CE22FE5FB8F'
--           ,'2');

GO