--File for the SQL stored procedures
-- AddJob, UpdateJob and DeleteJob
CREATE PROCEDURE dbo.AddJob
    @employerID INT,
    @jobName VARCHAR(55),
    @jobDescription VARCHAR(55),
    @jobLocation VARCHAR(55),
    @startDate DATETIME,
    @endDate DATETIME,
    @prisonersNeeded INT,
    @completed BIT,
    @durationPerDay INT
AS
BEGIN
    IF @employerID IS NULL
    BEGIN
        RAISERROR('EmployerID cannot be NULL', 16, 1);
        RETURN;
    END

    INSERT INTO JOB (EmployerID, JobName, JobDescription, JobLocation, StartDate, EndDate,
                     PrisonersNeeded, Completed, DurationPerDay)
    VALUES (@employerID, @jobName, @jobDescription, @jobLocation, @startDate, @endDate,
            @prisonersNeeded, @completed, @durationPerDay);
END
GO

--Update Job

CREATE PROCEDURE dbo.UpdateJob
    @jobID             INT,
    @employerID        INT,
    @jobName           VARCHAR(55),
    @jobDescription    VARCHAR(55),
    @jobLocation       VARCHAR(55),
    @startDate         DATETIME,
    @endDate           DATETIME,
    @prisonersNeeded   INT,
    @completed         BIT,
    @durationPerDay    INT
AS
BEGIN
    UPDATE JOB
    SET EmployerID      = @employerID,
        JobName         = @jobName,
        JobDescription  = @jobDescription,
        JobLocation     = @jobLocation,
        StartDate       = @startDate,
        EndDate         = @endDate,
        PrisonersNeeded = @prisonersNeeded,
        Completed       = @completed,
        DurationPerDay  = @durationPerDay
    WHERE JobID = @jobID;
END
GO

--Delete job

CREATE PROCEDURE dbo.DeleteJob
    @jobID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Delete from child tables first
    DELETE FROM PRISONER_ON_JOB
    WHERE JobID = @jobID;

    DELETE FROM SKILL_OF_JOB
    WHERE JobID = @jobID;

    -- Finally delete the job itself
    DELETE FROM JOB
    WHERE JobID = @jobID;
END
GO


