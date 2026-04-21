using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class User_Group
    {
        [Key]
        public int strId { get; set; }
        public string GroupId { get; set; }
        [Required]
        [DisplayName("Group Name")]
        public string strGroupName { get; set; }        
        public string CompanyCode { get; set; }
        [DisplayName("Set up")]
        public bool Setup { get; set; }
        public bool Records { get; set; }
        public bool Transactions { get; set; }
        [DisplayName("Credit Management")]
        public bool CreditManagement { get; set; }
        public bool Accounting { get; set; }
        public bool Enquiries { get; set; }
        public bool Reports { get; set; }
        [DisplayName("Add System Users")]
        public bool SystemUsers { get; set; }
        //public string UserGroups { get; set; }
        [DisplayName("Add Loan Types")]
        public bool LoanTypes { get; set; }
        [DisplayName("Add Share Types")]
        public bool ShareTypes { get; set; }
        [DisplayName("Cig Registration")]
        public bool CigRegistration { get; set; }
        [DisplayName("Bank Setup")]
        public bool BankSetup { get; set; }
        [DisplayName("Charges Setup")]
        public bool ChargesSetup { get; set; }
        [DisplayName("Membership Registration")]
        public bool MembershipRegistration { get; set; }
        [DisplayName("Sacco Registration")]
        public bool SaccoRegistration { get; set; }
        [DisplayName("Recruitment Agents")]
        public bool RecruitmentAgents { get; set; }
        public bool Beneficiaries { get; set; }
        [DisplayName("Member Withdrawal")]
        public bool MemberWithdrawal { get; set; }
        public bool Receipting { get; set; }
        [DisplayName("Journal Posting")]
        public bool JournalPosting { get; set; }
        [DisplayName("Loan Applications")]
        public bool LoanApplications { get; set; }
        public bool Appraisals { get; set; }
        public bool Endorsements { get; set; }
        public bool LoanSchedule { get; set; }
        [DisplayName("Receipt Posting")]
        public bool ReceiptPosting { get; set; }
        [DisplayName("Reprint Member Receipt")]
        public bool ReprintMemberReceipt { get; set; }
        [DisplayName("Transfers And Offsettings")]
        public bool TransfersAndOffsettings { get; set; }
        [DisplayName("Share Transfer")]
        public bool ShareTransfer { get; set; }
        [DisplayName("Share To Loan Offsetting")]
        public bool ShareToLoanOffsetting { get; set; }
        [DisplayName("Dividends Processing")]
        public bool DividendsProcessing { get; set; }
        [DisplayName("Dividends Payment")]
        public bool DividendsPayment { get; set; }
        [DisplayName("Transactions Management")]
        public bool TransactionsManagement { get; set; }
        [DisplayName("Account Setup")]
        public bool AccountSetup { get; set; }
        [DisplayName("GL Inquiry")]
        public bool GLInquiry { get; set; }
        [DisplayName("Books Of Accounts")]
        public bool BookOfAccounts { get; set; }
        [DisplayName("Shares Inquiry")]
        public bool SharesInquiry { get; set; }
        [DisplayName("Loans Inquiry")]
        public bool LoansInquiry { get; set; }
        [DisplayName("Cigs Per Sacco")]
        public bool CigsPerSacco { get; set; }
        [DisplayName("Members Per Sacco")]
        public bool MembersPerSacco { get; set; }
        [DisplayName("Members Per County")]
        public bool MembersPerCounty { get; set; }
        [DisplayName("Project And Investment Management")]
        public bool ProjectAndInvestmentManagement { get; set; }
        public bool ProjectAndInvestments { get; set; }
        public bool FundDrives { get; set; }
        public bool SubscribeToProject { get; set; }
       

    }
}
