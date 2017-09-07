using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;

namespace SoftMarket.Devices.Printers.Datecs
{
	/// <summary>
	/// Отвечает за локализацию сборки.
	/// </summary>
	public class DatecsStrings
	{
		/// <summary>
		/// Инициализирует менеджер ресурсов.
		/// </summary>
		static DatecsStrings()
		{
			string baseName = "SoftMarket.Devices.Printers.Datecs.DatecsStrings";
			LocRM = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
		}
		private static ResourceManager LocRM = null;
		/// <summary>
		/// Возвращает строку сообщения для текущего языка.
		/// </summary>
		/// <param name="msg">Идентификатор сообщения.</param>
		/// <returns>Строка сообщения.</returns>
		public static string GetString(int msg)
		{
			return LocRM.GetString(msg.ToString());
		}	
	}
}
