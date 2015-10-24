using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace InputProcessing
{
	public abstract class Field<T> : Service
	{
		public override string Description {
			get {
				return string.Format ("{0} {1}-field", 
					(IsRequired ? "required" : "optional"),
					typeof(T).ToString ());
			}
		}

		[Instruction("Default value for this field")]
		public T Default {
			get;
			set;
		}

		[Instruction("Is this field required?")]
		public bool IsRequired {
			get;
			set;
		}

		protected override void Initialize (Settings settings)
		{
			this.IsRequired = settings.GetBool ("required", false);
		}

		protected abstract Service GetFeedbackForInput (object rawInput, out T value);

		protected Service Failure {
			get {
				return Branches.Get ("failure");
			}
		}

		protected Service View {
			get {
				return Branches.Get ("view");
			}
		}

		protected Service BadFormat {
			get {
				return Branches.Get ("badformat", this.Failure);
			}
		}

		protected Service Missing {
			get {
				return Branches.Get("missing", this.Failure);
			}
		}

		protected Service Successful {
			get {
				return Branches.Get ("successful", this.View);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;
			IInteraction formInteractionCandidate;

			Service feedback = this.View;

			if (parameters.TryGetClosest (
				typeof(IRawInputInteraction), 
				out formInteractionCandidate)
			) {
				IRawInputInteraction formInteraction = 
					(IRawInputInteraction)formInteractionCandidate;

				if (formInteraction.HasValuesAvailable) {
					T processedValue;

					feedback = GetFeedbackForInput (
						formInteraction.ReadInput (), 
						out processedValue);

					successful = feedback == this.Successful;

					if (successful) {
						formInteraction.SetProcessedValue (processedValue);
					}
				} else if (IsRequired) {
					feedback = this.Missing;
					successful = false;
				}

				formInteraction.Feedback [formInteraction.CurrentName] = feedback;
			}

			return successful;
		}
	}

}
