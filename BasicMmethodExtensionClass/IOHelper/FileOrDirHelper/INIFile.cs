using System.Runtime.InteropServices;
using System.Text;

namespace BasicMmethodExtensionClass.IOHelper.FileOrDirHelper
{
    /// <summary>
    /// INI�ļ���д�ࡣ
    /// </summary>
	public class IniFile
	{
		public string path;

		public IniFile(string iniPath)
		{
			path = iniPath;
		}

		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section,string key,string def, StringBuilder retVal,int size,string filePath);

	
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string defVal, System.Byte[] retVal, int size, string filePath);


		/// <summary>
		/// дINI�ļ�
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void IniWriteValue(string section,string key,string value)
		{
			WritePrivateProfileString(section,key,value,this.path);
		}

		/// <summary>
		/// ��ȡINI�ļ�
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public string IniReadValue(string section,string key)
		{
			StringBuilder temp = new StringBuilder(255);
			int i = GetPrivateProfileString(section,key,"",temp, 255, this.path);
			return temp.ToString();
		}
		public byte[] IniReadValues(string section, string key)
		{
			byte[] temp = new byte[255];
			int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
			return temp;

		}


		/// <summary>
		/// ɾ��ini�ļ������ж���
		/// </summary>
		public void ClearAllSection()
		{
			IniWriteValue(null,null,null);
		}
		/// <summary>
		/// ɾ��ini�ļ���personal�����µ����м�
		/// </summary>
		/// <param name="section"></param>
		public void ClearSection(string section)
		{
			IniWriteValue(section,null,null);
		}

	}


}
