namespace Bussiness.Error
{
	public class ErrorCode
	{
		//System and account
		public const string TOKEN_IS_MISSING = "Token is missing";
		public const string MISSING_PERMISSON = "Missing permission";
		public const string USER_NOT_FOUND = "Account not existed";
		public const string ACCOUNT_INVALID = "Username or password word not match";
		public const string UNAUTHOR = "Not have permission";
		public const string DOCTOR_NOT_FOUND = "Doctor not found";
		public const string STAFF_NOT_FOUND = "Staff not found";

		//Booking
		public const string BOOKING_NOT_FOUND = "Booking not found";
		public const string BOOKING_DETAIL_NOT_FOUND = "Booking details not found";
		public const string STATUS_NOT_ACCEPT = "Status not accept";
		public const string NOT_PAYMENT = "Booking is not payment";
		public const string NOT_CONFIRM = "Booking payment not confirm";
		public const string BOOKING_EXPIRED = "Booking is expired";
		public const string BOOKING_ALREADY_ASSIGNED = "Booking is already assgined before";
		public const string BOOKING_DETAIL_ALREADY_ASSIGNED = "Booking detail is already assgined before";
		public const string BOOKING_WAITING_FOR_CONFIRM = "Booking is waiting for confirm";
		public const string DOCTOR_BUSY = "Doctor is busy in this time";

		//Time
		public const string TIME_START_ERROR = "The time start must be > 7am";
		public const string TIME_END_ERROR = "The time end must be < 5pm";
	}
}
