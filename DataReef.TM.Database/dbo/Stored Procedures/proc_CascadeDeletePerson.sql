
CREATE PROCEDURE proc_CascadeDeletePerson
    @PersonID uniqueidentifier
AS 

delete from  dbo.OUAssociations      where PersonID = @PersonID;
delete from  dbo.AccountAssociations where PersonID = @PersonID;
delete from  dbo.PersonSettings      where PersonID = @PersonID;
delete from  dbo.PhoneNumbers        where PersonID = @PersonID;
delete from  dbo.Attachments		 where Person_Guid = @PersonID;
delete from  dbo.Identifications     where PersonID = @PersonID;
delete from  dbo.Notes				 where PersonID = @PersonID;
delete from  dbo.Reminders			 where PersonID = @PersonID;
delete from  dbo.PersonalConnections where FromPersonID = @PersonID or ToPersonID = @PersonID;
delete from  dbo.UserInvitations     where FromPersonID = @PersonID or ToPersonID = @PersonID;
delete from  dbo.UserDevices         where UserID   = @PersonID;
delete from  dbo.Credentials	     where UserID   = @PersonID;
delete from  dbo.Assignments		 where PersonID = @PersonID;
delete from  dbo.Inquiries		     where PersonID = @PersonID;
delete from  dbo.CurrentLocations	 where PersonID = @PersonID;
delete from  dbo.Users               where PersonID = @PersonID;
delete from  dbo.People              where Guid     = @PersonID;  