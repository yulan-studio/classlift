namespace Billing.Constants
{
    public static class InvoiceStatus
    {
        /// <summary>
        /// Invoice has been created but not yet paid.
        /// </summary>
        public const string Pending = "Pending";

        /// <summary>
        /// Invoice has been paid in full.
        /// </summary>
        public const string Paid = "Paid";

        /// <summary>
        /// Invoice is past its due date.
        /// </summary>
        public const string Overdue = "Overdue";

        /// <summary>
        /// Invoice has been cancelled.
        /// </summary>
        public const string Cancelled = "Cancelled";

        /// <summary>
        /// Invoice has been refunded.
        /// </summary>
        public const string Refunded = "Refunded";

        /// <summary>
        /// Invoice has been partially paid.
        /// </summary>
        public const string PartiallyPaid = "PartiallyPaid";
    }
}
