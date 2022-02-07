using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace CnxCacheMonitoring
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				new Program().Run(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		void Run(string[] args)
		{
			const string progId = "D7ConnectionCacheServerSX.D7ConnectionCacheServerSX.1";
			Type comType = Type.GetTypeFromProgID(progId);
			D7DCCXLib.ID7ConnectionCacheServer cnxCache = Activator.CreateInstance(comType) as D7DCCXLib.ID7ConnectionCacheServer;
			TestMonitor(cnxCache);
		}
		
		void TestMonitor(D7DCCXLib.ID7ConnectionCacheServer cnxCache)
		{
			var I = cnxCache as D7DCCXLib.ID7ConnectionCacheServer3;
			while (true)
			{
				string xml = I.GetCacheInfo("Secured SQL Server", null);
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);
				//Console.WriteLine(xml);
				var node = doc.SelectSingleNode("/Document/ConnectionsInCache");
				Console.WriteLine("# of cnx in cache=" + node.InnerText);
				Thread.Sleep(1000);
			}
		}

		void TestGetData(D7DCCXLib.ID7ConnectionCacheServer cnxCache)
		{
			var I = cnxCache.GetConnection(10000, "Local SQL Server", "", "");
			bool b = I.FirstData("select * from datasetreport.demo.salesman");
			if (b)
			{
				short fieldCount = I.GetFieldCount();
				for (short i = 0; i < fieldCount; i++)
				{
					int type;
					int len;
					short dec;
					int nullable;
					string s = I.Describe((short)(i + 1), out type, out len, out dec, out nullable);
					Console.Write(s + "\t");
				}
				Console.WriteLine();
				while (b)
				{
					for (short i = 0; i < fieldCount; i++)
					{
						string s = I.GetData((short)(i + 1));
						Console.Write(s + "\t");
					}
					Console.WriteLine();
					b = I.NextData();
				}
			}
		}
	}
}
