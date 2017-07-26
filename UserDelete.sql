DECLARE @name char(20) = 'kintsugicorp';  

delete from dbo.Transactions where Sender = @name OR Receiver = @name;
delete from dbo.GameEvents where [User] = @name;
delete from dbo.Payments where Employer = @name OR Receiver = @name;
delete from dbo.Loyalties where LoyalName = @name;
delete from dbo.AccountAccesses where Slave = @name OR [Master] = @name;
delete from dbo.Accounts where [Login] = @name;
GO