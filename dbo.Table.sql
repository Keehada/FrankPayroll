CREATE TABLE [dbo].[Payroll] (
    [EmployeeId]   INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]    NVARCHAR (MAX) NULL,
    [LastName]     NVARCHAR (MAX) NULL,
    [PayrollError] INT            NOT NULL,
    CONSTRAINT [PK_Payroll] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);
