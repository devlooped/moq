namespace	Moq.Sequencing.Extensibility
{
	///	<summary>
	///	Single step	of verification	of call	order
	///	</summary>
	public interface IVerificationStep
	{
		///	<summary>
		///	Verifies the step. Throws	MockException	on failure
		///	</summary>
		void Verify();

		///	<summary>
		///	Allows assigning a call	sequence that	records
		///	calls	that were	made and allows	verification
		///	of the order of	the	calls
		///	</summary>
		CallSequence CallSequence	{	get; }
	}
}