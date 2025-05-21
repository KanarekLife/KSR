using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Server
{
	[ServiceContract]
	public interface IService
	{

		[OperationContract]
		bool Create(string login, string password);

		[OperationContract]
		Guid Login(string login, string password);

        [OperationContract]
		bool Logout(string login);

		[OperationContract]
		bool Put(string name, string content, Guid sessionId);

        [OperationContract]
        string Get(string name, Guid sessionId);
    }
}
