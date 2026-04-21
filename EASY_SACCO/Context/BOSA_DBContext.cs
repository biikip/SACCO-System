
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EASY_SACCO.Controllers;
using EASY_SACCO.Models;
using Microsoft.EntityFrameworkCore;

namespace EASY_SACCO.Context
{
    public class BOSA_DBContext : DbContext
    {
        public BOSA_DBContext(DbContextOptions<BOSA_DBContext> options) : base(options)
        {

        }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<EASY_SACCO.Models.RepayMentTable> REPAY { get; set; }
        public DbSet<EASY_SACCO.Models.Loanschd> LOANSCHD { get; set; }
        public DbSet<EASY_SACCO.Models.NumberSeries> NumberSeries { get; set; }
        public DbSet<EASY_SACCO.Models.Project> Projects { get; set; }
        public DbSet<EASY_SACCO.Models.MemberInvestment> MemberInvestments { get; set; }
        public DbSet<EASY_SACCO.Models.ProjectSubscription> ProjectSubscriptions { get; set; }
        public DbSet<EASY_SACCO.Models.FundDrive> FundDrives { get; set; }
        public DbSet<EASY_SACCO.Models.FundDriveAllocation> FundDriveAllocations { get; set; }
        public DbSet<EASY_SACCO.Models.Disbursement_Deduction> Disbursement_Deductions { get; set; }
        public DbSet<EASY_SACCO.Models.Customer_Balance> Customer_Balance { get; set; }
        public DbSet<EASY_SACCO.Models.Charges_Setup> Charges_Setups { get; set; }
        public DbSet<EASY_SACCO.Models.Contrib> CONTRIB { get; set; }
        public DbSet<EASY_SACCO.Models.Loanguar> Loanguars { get; set; }
        public DbSet<EASY_SACCO.Models.WithdrawalNotice> withdrawalNotices { get; set; }
        public DbSet<EASY_SACCO.Models.Session1> Session1 { get; set; }
        public DbSet<EASY_SACCO.Models.GL_Transaction> GL_Transactions { get; set; }
        public DbSet<EASY_SACCO.Models.Co_operative> co_Operatives { get; set; }
        public DbSet<EASY_SACCO.Models.Repayment_Logs> Repayment_Logs { get; set; }
        public DbSet<EASY_SACCO.Models.County> counties { get; set; }
        public DbSet<EASY_SACCO.Models.Sub_County> sub_Counties { get; set; }
        public DbSet<EASY_SACCO.Models.System_Users> system_Users { get; set; }
        public DbSet<EASY_SACCO.Models.User_Group> user_Groups { get; set; }
        public DbSet<EASY_SACCO.Models.Villages> villages { get; set; }
        public DbSet<EASY_SACCO.Models.Ward> wards { get; set; }
        public DbSet<EASY_SACCO.Models.Location> Location { get; set; }
        public DbSet<EASY_SACCO.Models.CIG_Croups> CIG_Croups { get; set; }
        public DbSet<EASY_SACCO.Models.Stations> stations { get; set; }
        public DbSet<EASY_SACCO.Models.Agents> Agents { get; set; }
        public DbSet<EASY_SACCO.Models.MembersRegistration> MembersRegistrations { get; set; }
        //public DbSet<EASY_SACCO.Models.MembersRegistration> MembersRegistrations2 { get; set; }
        public DbSet<EASY_SACCO.Models.MEMBERS> MEMBERS { get; set; }
        public DbSet<EASY_SACCO.Models.Gender> Gender { get; set; }
        public DbSet<EASY_SACCO.Models.Member_Type> Member_Type { get; set; }
        public DbSet<EASY_SACCO.Models.Marital_Status> Marital_Status { get; set; }
        public DbSet<EASY_SACCO.Models.Active_Status> Active_Statuses { get; set; }
        public DbSet<EASY_SACCO.Models.LoanTYpes> loanTYpes { get; set; }
        public DbSet<EASY_SACCO.Models.ShareType> shareTypes { get; set; }
        public DbSet<EASY_SACCO.Models.Collaterals> collaterals { get; set; }
        public DbSet<EASY_SACCO.Models.WhatToChange> whatToChanges { get; set; }

        public DbSet<EASY_SACCO.Models.LoginIPS> loginIPs { get; set; }
        public DbSet<EASY_SACCO.Models.Rate> Rates { get; set; }
        public DbSet<EASY_SACCO.Models.RepaymentMethod> RepaymentMethods { get; set; }
        public DbSet<EASY_SACCO.Models.Branches> Branches { get; set; }
        public DbSet<EASY_SACCO.Models.SYSPARAM> SYSPARAM { get; set; }
        public DbSet<EASY_SACCO.Models.AccountType> accountTypes { get; set; }
        public DbSet<EASY_SACCO.Models.AccountType1> AccountType1s { get; set; }
        public DbSet<EASY_SACCO.Models.AccountType1s> accountType1S { get; set; }
        public DbSet<EASY_SACCO.Models.AccountSubGroup> accountSubGroups { get; set; }
        public DbSet<EASY_SACCO.Models.Currency> Currencies { get; set; }

        public DbSet<EASY_SACCO.Models.AccountCategory> accountCategories { get; set; }
        public DbSet<EASY_SACCO.Models.AccountGroup> accountGroups { get; set; }
        public DbSet<EASY_SACCO.Models.AccountSubCategory> accountSubCategories { get; set; }
        public DbSet<EASY_SACCO.Models.AccountSetupGL> accountSetupGLs { get; set; }
        public DbSet<EASY_SACCO.Models.MembersDefaults> membersDefaults { get; set; }
        public DbSet<EASY_SACCO.Models.BanksSetup> banksSetups { get; set; }
        public DbSet<EASY_SACCO.Models.AccountCodes> accountCodes { get; set; }
        public DbSet<EASY_SACCO.Models.Beneficiaries> beneficiaries { get; set; }
        public DbSet<EASY_SACCO.Models.RelationShip> relationShips { get; set; }
        public DbSet<Glsetup> Glsetups { get; set; }
        public DbSet<AccountBooksOfAccounts> AccountBooksOfAccounts { get; set; }
        public DbSet<ProgrationProcessingOptions> ProgrationProcessingOptions { get; set; }
        public DbSet<ProgrationProcessing> ProgrationProcessing { get; set; }
        public DbSet<EASY_SACCO.Models.NormalBalance> normalBalances { get; set; }
        public DbSet<EASY_SACCO.Models.ShareTransaction> shareTransactions { get; set; }
        public DbSet<EASY_SACCO.Models.GLTransaction> gLTransactions { get; set; }
        public DbSet<EASY_SACCO.Models.ShareBalance> shareBalances { get; set; }
        public DbSet<EASY_SACCO.Models.ShareTransactionss> ShareTransaction { get; set; }
        public DbSet<EASY_SACCO.Models.LoanAplication> loanAplications { get; set; }
        public DbSet<EASY_SACCO.Models.RepaymentLogs_Table> RepaymentLogs_Tables { get; set; }

        public DbSet<EASY_SACCO.Models.Charges> Charges { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MembersRegistration>()
                .HasIndex(m => m.strMemberNo)
                .IsUnique(); // Just a Unique Index, allows NULLs

           
        }

    }
}
