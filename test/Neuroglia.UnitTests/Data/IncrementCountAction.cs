namespace Neuroglia.UnitTests.Data
{

    public class IncrementCountAction
    {

        public IncrementCountAction(int amount)
        {
            this.Amount = amount;
        }

        public int Amount { get; }

    }

}
