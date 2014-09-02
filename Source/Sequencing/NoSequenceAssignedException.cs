namespace	Moq.Sequencing
{
	///	<summary>
	///	Thrown when	call order is	verified on	mock but no	valid	sequence is	assigned to	the	mock
	///	</summary>
	public class NoSequenceAssignedException : MockException
	{
		///	<summary>
		///	Constructor
		///	</summary>
		///	<param name="exceptionMessage"></param>
		public NoSequenceAssignedException(string	exceptionMessage)
			:	base(ExceptionReason.VerificationFailed, exceptionMessage)
		{
		}
	}
}