namespace Allinone.Domain.DS.Transactions
{
    public class DSYearExpenses
    {
        public DSYearExpenses()
        {
            DSYearDetails = new List<DSYearDetails>();
        }

        public List<string> DSItemNames { get; set; }
        public List<DSYearDetails> DSYearDetails { get; set; }
    }

    public class DSYearDetails
    {
        public DSYearDetails()
        {
            Amount = new List<decimal>();
        }

        public string YearMonth { get; set; }
        public List<decimal> Amount { get; set; }
    }

    public class DSYearCreditDebitDiff
    {
        public string YearMonth { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Diff { get; set; }
    }

    public class DSDebitStat
    {
        public string DSItemName { get; set; }
        public decimal Amount { get; set; }
    }

    public class DSMonthlyExpenses
    {
        public List<DSMonthlyExpensesItem> Items { get; set; }
        public List<DSMonthlyExpensesItem> ItemsOther { get; set; }
    }

    public class DSMonthlyExpensesItem
    {
        public int ItemID { get; set; }
        public int ItemSubID { get; set; }
        public string ItemName { get; set; }
        public string Desc { get; set; }
        public decimal Amount { get; set; }
    }

    public class DSMonthlyPeriodCreditDebit
    {
        public DateTime YearMonthDatetime { get; set; }
        public string YearMonth { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Remain { get; set; }
        public string Usage { get; set; }
        public string CreditCompare { get; set; }
        public string DebitCompare { get; set; }
        public string UsageCompare { get; set; }
    }

    public class DSMonthlyItemExpenses
    {
        public DateTime YearMonthDatetime { get; set; }
        public List<DSMonthlyItem> DSMonthlyItems { get; set; }
        public List<DSMonthlyItem> DSMonthlySubItems { get; set; }
    }

    public class DSMonthlyItem
    {
        public string ItemName { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountLast { get; set; }
        public decimal Diff { get; set; }
        public decimal DiffPercentageNumber { get; set; }
        public string AmountComparePercentage { get; set; }
        public List<DSMonthlyExpensesItem> ItemsDetail { get; set; }
    }
}
