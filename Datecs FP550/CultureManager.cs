using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;

namespace SoftMarket.Devices.Printers.Datecs
{
	/// <summary>
	/// �������� �� ����������� ������.
	/// </summary>
	public class DatecsStrings
	{
		/// <summary>
		/// �������������� �������� ��������.
		/// </summary>
		static DatecsStrings()
		{
			string baseName = "SoftMarket.Devices.Printers.Datecs.DatecsStrings";
			LocRM = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
		}
		private static ResourceManager LocRM = null;
		/// <summary>
		/// ���������� ������ ��������� ��� �������� �����.
		/// </summary>
		/// <param name="msg">������������� ���������.</param>
		/// <returns>������ ���������.</returns>
		public static string GetString(int msg)
		{
			return LocRM.GetString(msg.ToString());
		}	
	}
}
