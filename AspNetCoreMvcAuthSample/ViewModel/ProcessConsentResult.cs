using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMvcAuthSample.ViewModel
{
	public class ProcessConsentResult
	{
		public string RedireUrl { get; set; }

		public bool IsRedirect => RedireUrl != null;

		public string ValidationError { get; set; }

		public ConsentViewModel ViewModel { get; set; }
	}
}
