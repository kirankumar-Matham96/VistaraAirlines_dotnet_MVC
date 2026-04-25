/* TRIGGERS */

-- Trigger to update UpdatedAt field in Users table
CREATE TRIGGER tr_User_UpdatedAT
ON Users
AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- To avoid recursion
	IF TRIGGER_NESTLEVEL() > 1
	RETURN;

	-- Best practice to update the fields when actual data changes
	IF NOT UPDATE(UpdatedAt)
	BEGIN
		UPDATE u
		SET u.UpdatedAt = GETDATE()
		FROM Users u
		INNER JOIN INSERTED i ON u.UserId = i.UserId;
	END;
END;
