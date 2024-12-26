namespace Redellion.API
{
    public class SendReport : Report
    {
        internal override string RequestName => "Send";

        internal override object PostData => new { appId, sender, log };

        public string sender;
        public string log;

        public SendReport(string sender, string log)
        {
            this.sender = sender;
            this.log = log;
        }
    }
}
