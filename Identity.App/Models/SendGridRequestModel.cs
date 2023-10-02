using SendGrid.Helpers.Mail;

namespace Identity.App.Models
{
	public class SendGridRequestModel 
	{
		public  From _From { get; set; }
		public  Personalization _Personalization { get; set; }
		public  Root _Root { get; set; }
		public  To _To { get; set; }

        public SendGridRequestModel(From from, Personalization personalization, Root root, To to)
        {
			_From = from;
			_Personalization = personalization;
			_Root = root;
			_To = to;
		}
	}

	public class From
	{
		public string email { get; set; }
		public string name { get; set; }
	}

	public class Personalization
	{
		public List<To> to { get; set; }
		public string subject { get; set; }
		public Personalization personalization { get; set; }
    }
	
	public class Root
	{
		public List<Personalization> personalizations { get; set; }
		public From from { get; set; }
		public List<Content> content { get; set; }
    }
	
	public class To
	{
		public string email { get; set; }
		public string name { get; set; }
    }
}


