INSERT INTO OUSettings ([Guid], OUID, Name, [Value], [Group], Inheritable, DateCreated, CreatedByID, TenantID, [Version], IsDeleted, ValueIsDefault)
select NEWID(), CV.OUID, CF.DisplayName, CV.Value, 2, 1, CF.DateCreated, CV.OwnerID, 0, 0, 0, 0
FROM OUCustomValues CV
INNER JOIN OUCustomFields CF
ON CV.CustomFieldID = CF.Guid AND CV.OUID = CF.OUID
WHERE CV.IsDeleted = 0
	 AND CF.IsDeleted = 0
