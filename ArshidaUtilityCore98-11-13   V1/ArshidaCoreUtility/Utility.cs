using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using RestSharp;
using  System.Drawing.Imaging;

namespace ArshidaCoreUtility
{
    public enum SQLInjectionkind { Normal, Name, Email };
    public enum ExpressionType { Email, Name, SocialSecurityNumber, PhoneNumber, URL, ZIPCOde, Password, NoneNegativeInteger, Currency, NoneNegativeCurrency };
    public enum OperationState { Insert, Update, Delete, SelectInsert, Print, Select, Select1, Select2, Select3, Select4, Select5, OtherOperation1, OtherOperation2, OtherOperation3, OtherOperation4, OtherOperation5, OtherOperation6, OtherOperation7, OtherOperation8, OtherOperation9, OtherOperation10, Report1, Report2, Report3, Report4, Report5, Report6, Report7, Report8, Report9, Report10 }
    public enum SQLAction { Execute, Insert, Update, Delete, Print };
    public enum WebPictureValidity { InvalidSize, InvalidFormat, Valid }
    public static class Utility
    {
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        public static string SendNoFilteredMessage(string apiKey, string ReciptorNumber, string TemplateName, string Param1, string Param2, string Param3)
        {
            try
            {
                var client = new RestClient("http://api.smsapp.ir/v2/send/verify");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("apikey", apiKey);
                if (Param2.Length != 0 && Param3.Length != 0)
                    request.AddParameter("application/x-www-form-urlencoded", "type=1&param1=" + Param1 + "&param2=" + Param2 + "&param3=" + Param3 + "&receptor=" + ReciptorNumber + "&template=" + TemplateName, ParameterType.RequestBody);
                else if (Param2.Length != 0 && Param3.Length == 0)
                    request.AddParameter("application/x-www-form-urlencoded", "type=1&param1=" + Param1 + "&param2=" + Param2 + "&receptor=" + ReciptorNumber + "&template=" + TemplateName, ParameterType.RequestBody);
                else
                    request.AddParameter("application/x-www-form-urlencoded", "type=1&param1=" + Param1 + "&receptor=" + ReciptorNumber + "&template=" + TemplateName, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                return response.StatusCode.ToString();
            }
            catch (Exception ex) { return ex.Message; }
            //            کد خطا  توضیحات
            //1   نام کاربری یا رمز عبور معتبر نمی باشد .
            //2   آرایه ها خالی می باشد.
            //3   طول آرایه بیشتر از 100 می باشد .
            //4   طول آرایه ی فرستنده و گیرنده و متن پیام با یکدیگر تطابق ندارد.
            //5   امکان گرفتن پیام جدید وجود ندارد .
            //6 - حساب کاربری غیر فعال می باشد.
            //- نام کاربری و یا رمز عبور خود را به درستی وارد نمی کنید .
            //-در صورتی که به تازگی وب سرویس را فعال کرده اید از منوی تنظیمات _رمز عبور ، رمز عبور وب سرویس خود را مجدد ست کنید.
            //7   امکان دسترسی به خط مورد نظر وجود ندارد .
            //8   شماره گیرنده نامعتبر است .
            //9   حساب اعتبار ریالی مورد نیاز را دارا نمی باشد.
            //10  خطایی در سیستم رخ داده است . دوباره سعی کنید.
            //11  نامعتبر می باشد.IP
            //20  شماره مخاطب فیلتر شده می باشد .
            //21  ارتباط با سرویس دهنده قطع می باشد.
        }
        public static DataSet ConvertListToDataSet<T>(this IList<T> list)
        {

            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);
            try
            {
                foreach (var propInfo in elementType.GetProperties())
                {
                    try
                    {
                        //System.Reflection.PropertyInfo pi;
                        //pi.PropertyType = new Type() { }
                        t.Columns.Add(propInfo.Name, propInfo.PropertyType);
                        // Type tt;
                        //tt = 
                        //t.Columns.Add(propInfo.Name, tt);
                    }
                    catch (Exception ex) { }
                }
                foreach (T item in list)
                {
                    DataRow row = t.NewRow();
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        try
                        {
                            row[propInfo.Name] = propInfo.GetValue(item, null);
                        }
                        catch (Exception ex) { }
                    }
                    t.Rows.Add(row);
                }
            }
            catch (Exception ex) { }
            return ds;
        }
        public static DataTable ConvertListToDataSet2<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        public static string UpLoadFileInHost(string Server, string User, string Password, string FileName, string FullName)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", Server, FileName)));

                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(User, Password);
                Stream ftpStream = request.GetRequestStream();
                FileStream fs = File.OpenRead(FullName);
                byte[] buffer = new byte[1024];
                double total = (double)fs.Length;
                int byteRead = 0;
                double read = 0;
                do
                {
                    byteRead = fs.Read(buffer, 0, 1024);
                    ftpStream.Write(buffer, 0, byteRead);
                    read += (double)byteRead;
                }
                while (byteRead != 0);
                fs.Close();
                ftpStream.Close();
                return "OK";
            }
            catch (Exception ex) { return ex.Message; }
        }
        public static string CreateDirectoryWithFtpInHost(string Server, string DirectoryName, string User, string password)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", Server, DirectoryName)));

            string currentDir = string.Format("{0}", Server);

            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir + "/" + DirectoryName);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(User, password);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static bool DirectoryExistsInHost(string directoryPath, string username, string password)
        {
            bool IsExists = true;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryPath);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException ex)
            {
                IsExists = false;
            }
            return IsExists;

        }
        public static string DeleteFileFromHost(string ftpURL, string UserName, string Password, string ftpDirectory, string FileName)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpURL + "/" + ftpDirectory + "/" + FileName);
                ftpRequest.Credentials = new NetworkCredential(UserName, Password);
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse responseFileDelete = (FtpWebResponse)ftpRequest.GetResponse();
                responseFileDelete.Close();
                return "OK";
            }
            catch (Exception ex) { return ex.Message; }
        }
        public static string ConvertHexToBinary(string st)
        {
            switch (st)
            {
                case "0": return "0000";
                case "1": return "0001";
                case "2": return "0010";
                case "3": return "0011";
                case "4": return "0100";
                case "5": return "0101";
                case "6": return "0110";
                case "7": return "0111";
                case "8": return "1000";
                case "9": return "1001";
                case "a": return "1010";
                case "b": return "1011";
                case "c": return "1100";
                case "d": return "1101";
                case "e": return "1110";
                case "f": return "1101";
                default: return "0000";
            }
        }
        //public static System.Drawing.Imaging GenerateQRCode(string webSiteUrl, string with, string height)
        //{
        //    try
        //    {
        //        var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}", webSiteUrl, with, height);
        //        WebResponse response = default(WebResponse);
        //        Stream remoteStream = default(Stream);
        //        StreamReader readStream = default(StreamReader);
        //        WebRequest request = WebRequest.Create(url);
        //        response = request.GetResponse();
        //        remoteStream = response.GetResponseStream();
        //        readStream = new StreamReader(remoteStream);
        //        System.Drawing.Imaging img = System.Drawing.Imaging.FromStream(remoteStream);
        //        // img.Save("D:/QRCode/" + txtCode.Text + ".png");
        //        response.Close();
        //        remoteStream.Close();
        //        readStream.Close();

        //        return img;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return null;
        //}
        public static string GenerateRandomUniqueString(int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
        public static string GenerateRandomCode(int size)
        {
            try
            {
                ArrayList charList = new ArrayList();
                charList.Add('a'); charList.Add('b'); charList.Add('c'); charList.Add('d'); charList.Add('e'); charList.Add('f'); charList.Add('g'); charList.Add('h');
                charList.Add('i'); charList.Add('j'); charList.Add('k'); charList.Add('l'); charList.Add('m'); charList.Add('n'); charList.Add('o'); charList.Add('p');
                charList.Add('q'); charList.Add('r'); charList.Add('s'); charList.Add('t'); charList.Add('u'); charList.Add('v'); charList.Add('w'); charList.Add('x');
                charList.Add('y'); charList.Add('z'); charList.Add('1'); charList.Add('2'); charList.Add('3'); charList.Add('4'); charList.Add('5'); charList.Add('6');
                charList.Add('7'); charList.Add('8'); charList.Add('9'); charList.Add('0');
                string st = string.Empty;
                Random rand = new Random();
                for (int i = 0; i < size; i++)
                {
                    int r = rand.Next(0, charList.Count);
                    st += charList[r];
                }
                return st;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static double ConvertKilobytesToMegaBytes(long kilobytes)
        {
            try
            {
                return kilobytes / 1024f;
            }
            catch { return -1; }
        }
        public static double ConvertBytesToMegaBytes(long kilobytes)
        {
            try
            {
                return kilobytes / 1024f / 1024f;
            }
            catch { return -1; }
        }
        public static double ConvertBytesToKiloBytes(long bytes)
        {
            try
            {
                return bytes / 1024f;
            }
            catch { return -1; }
        }
        public static long GetDirectorySize(DirectoryInfo dirInfo)
        {
            try
            {
                long size = 0;
                // Add file sizes.
                FileInfo[] fis = dirInfo.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    size += fi.Length;
                }
                // Add subdirectory sizes.
                DirectoryInfo[] dis = dirInfo.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    size += GetDirectorySize(di);
                }
                return size;
            }
            catch { return -1; }
        }
        public static string GetHexa(string st)
        {
            switch (int.Parse(st) % 16)
            {
                case 0: return "0";
                case 1: return "1";
                case 2: return "2";
                case 3: return "3";
                case 4: return "4";
                case 5: return "5";
                case 6: return "6";
                case 7: return "7";
                case 8: return "8";
                case 9: return "9";
                case 10: return "A";
                case 11: return "B";
                case 12: return "C";
                case 13: return "D";
                case 14: return "E";
                case 15: return "F";
                default: return "0";
            }
        }
        //public static Region GetRoundedPictureBox(int Width, int Height)
        //{
        //    System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
        //    gp.AddEllipse(0, 0, Width - 3, Height - 3);
        //    Region rg = new Region(gp);
        //    return rg;
        //}
        public static Guid GuidToString(string g)
        {
            Guid result;
            result = new Guid(g);
            return result;
        }
        public static void RunConsolCommand(string anyCommand)
        {
            var proc1 = new ProcessStartInfo();
            proc1.UseShellExecute = true;

            proc1.WorkingDirectory = @"C:\Windows\System32";

            proc1.FileName = @"C:\Windows\System32\cmd.exe";
            proc1.Verb = "runas";
            proc1.Arguments = "/c " + anyCommand;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc1);
        }
        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }
        public static T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
        public static string LoadStringFromFile(string fileName)
        {
            try
            {
                FileStream fs;
                if (!File.Exists(fileName))
                {
                    fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
                    fs.Close();
                    return string.Empty;
                }
                else
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                return GetString(buffer);
            }
            catch (Exception ex)
            {
                return "***";
            }
        }
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        public static string SaveStringInFile(string fileName, string content)
        {
            try
            {
                FileStream fs;
                if (!File.Exists(fileName))
                {
                    fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                    fs.Close();
                    return string.Empty;
                }
                else
                {
                    File.Create(fileName).Close();
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Write);

                }
                byte[] buffer = GetBytes(content);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
                return "";
            }
            catch (Exception ex)
            {
                return "***";
            }
        }
        //public static string GetClientIPAddress(System.Web.HttpRequest httpRequest)
        //{
        //    string OriginalIP = string.Empty;

        //    string RemoteIP = string.Empty;

        //    OriginalIP = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"]; //original IP will be updated by Proxy/Load Balancer.

        //    RemoteIP = httpRequest.ServerVariables["REMOTE_ADDR"]; //Proxy/Load Balancer IP or original IP if no proxy was used

        //    if (OriginalIP != null && OriginalIP.Trim().Length > 0)
        //    {

        //        return OriginalIP + "(" + RemoteIP + ")"; //Lets return both the IPs.

        //    }
        //    return RemoteIP;

        //}
        //public static string GetMACAddress()
        //{
        //    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        //    ManagementObjectCollection moc = mc.GetInstances();
        //    string MACAddress = String.Empty;
        //    foreach (ManagementObject mo in moc)
        //    {
        //        if (MACAddress == String.Empty)
        //        {
        //            if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
        //        }
        //        mo.Dispose();
        //    }

        //    MACAddress = MACAddress.Replace(":", "");
        //    return MACAddress;
        //}
        public static string GetClientMacByNbtstat(string _sClientIP)
        {
            string _sMacAddress = null;
            try
            {
                ProcessStartInfo _proStart = new ProcessStartInfo();
                Process _proc = new Process();
                StringBuilder sb = new StringBuilder();
                _proStart.FileName = "nbtstat";
                _proStart.RedirectStandardInput = false;
                _proStart.RedirectStandardOutput = true;
                _proStart.UseShellExecute = false;
                _proStart.Arguments = "-A " + _sClientIP;
                _proc = Process.Start(_proStart);
                using (StreamReader _srRead = _proc.StandardOutput)
                {
                    while (!_srRead.EndOfStream)
                    {
                        _sMacAddress = _srRead.ReadLine();
                        if (_sMacAddress.ToUpper().IndexOf("MAC") != -1)
                        {
                            _sMacAddress = _sMacAddress.Substring(_sMacAddress.LastIndexOf("=") + 1);
                            break;
                        }
                    }
                    _proc.WaitForExit();
                }
                _proc.Dispose();
                _proc.Close();
            }
            catch
            {
                _sMacAddress = string.Empty;
            }
            return _sMacAddress;
        }
        public static bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        //public static bool IsFormAlreadyOpen(Type FormType, FormCollection CurrentFormCollection)
        //{
        //    try
        //    {
        //        foreach (Form OpenForm in CurrentFormCollection)
        //        {
        //            if (OpenForm.GetType() == FormType)
        //                return true;
        //        }
        //        return false;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        public static bool CheckValidMail(string CurrentMailForCheck)
        {
            if (Regex.IsMatch(CurrentMailForCheck, @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"))
                return true;
            else
                return false;
        }
        public static bool NationalCode(string Code)
        {
            try
            {
                char[] tempch = Code.ToCharArray();
                int[] numbers = new int[Code.Length];
                for (int i = 0; i < Code.Length; i++)
                {
                    numbers[i] = (int)char.GetNumericValue(tempch[i]);
                }
                int a = numbers[9];
                switch (Code)
                {
                    case "000000000":
                    case "111111111":
                    case "222222222":
                    case "333333333":
                    case "444444444":
                    case "555555555":
                    case "666666666":
                    case "777777777":
                    case "888888888":
                    case "999999999":
                        return false;
                }
                int b = (((((((((numbers[0] * 10) + (numbers[1] * 9)) + (numbers[2] * 8)) + (numbers[3] * 7)) + (numbers[4] * 6)) + (numbers[5] * 5)) + (numbers[6] * 4)) + (numbers[7] * 3)) + (numbers[8] * 2));
                int c = b - ((b / 11) * 11);
                if ((((c == 0) && (a == c)) || ((c == 1) && (a == 1))) || ((c > 1) && (a == Math.Abs((int)c - 11))))
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }
        /*
        public static DataSet ImportFromExcelNewAlgorithm(string FileName)
        {
            DataSet ds = new DataSet();

            try
            {
                string ConStr = "";
                //Extantion of the file upload control saving into ext because   
                //there are two types of extation .xls and .xlsx of Excel   
                // string ext = Path.GetExtension(FileUpload1.FileName).ToLower();  
                //getting the path of the file   
                string path = FileName;
                //saving the file inside the MyFolder of the server  
                //FileUpload1.SaveAs(path);  
                //  Label1.Text = FileUpload1.FileName + "\'s Data showing into the GridView";  
                //checking that extantion is .xls or .xlsx  
                //if (ext.Trim() == ".xls")  
                {
                    //connection string for that file which extantion is .xls  
                    ConStr = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"", path);
                }
                //else if (ext.Trim() == ".xlsx")  
                //{  
                //    //connection string for that file which extantion is .xlsx  
                //    ConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";  
                //}  
                //making query  
                string query = "SELECT * FROM [Sheet1$]";
                //Providing connection  
                OleDbConnection conn = new OleDbConnection(ConStr);
                //checking that connection state is closed or not if closed the   
                //open the connection  
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                //create command object  
                OleDbCommand cmd = new OleDbCommand(query, conn);
                // create a data adapter and get the data into dataadapter  
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                //fill the Excel data to data set  
                da.Fill(ds);
                //set data source of the grid view  
                // gvExcelFile.DataSource = ds.Tables[0];
                //binding the gridview  
                //gvExcelFile.DataBind();
                //close the connection  
                conn.Close();
            }
            catch (Exception ex) { ds.Prefix = ex.Message; }
            return ds;
        }
 
        public static DataSet ImportFromExcel(string FileName)
        {
            DataSet ResultDataSet = new DataSet();

            try
            {
                // creating Excel Application 

                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

                // creating new WorkBook within Excel application 

                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Open(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // creating new Excelsheet in workbook 

                Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

                // see the excel sheet behind the program 

                app.Visible = true;

                // get the reference of first sheet. By default its name is Sheet1. 

                // store its reference to worksheet 

                worksheet = workbook.Sheets["Sheet1"];

                worksheet = workbook.ActiveSheet;

                // changing the name of active sheet 
                worksheet.Name = "Sheet1";

                //Find count of rows and columns
                int rows = 0, columns = 0;
                for (int i = 1; i < worksheet.Rows.Count; i++)
                {
                    try
                    {
                        if (worksheet.Cells[1, i].Value.ToString().Length != 0)
                            rows++;
                    }
                    catch
                    {
                        break;
                    }
                }

                for (int i = 1; i < 1000000; i++)
                {
                    try
                    {
                        if (worksheet.Cells[i, 1].Value.ToString().Length != 0)
                            columns++;
                    }
                    catch
                    {
                        break;
                    }
                }

                // storing header part in Excel 
                ResultDataSet.Tables.Add("Table");
                for (int i = 1; i <= rows; i++)
                {
                    // ResultDataSet.Tables["Table"].Columns.Add(i.ToString(), worksheet.Cells[1, i].Value);
                    ResultDataSet.Tables["Table"].Columns.Add(worksheet.Cells[1, i].Value.ToString());

                }
                object[] obj = new object[rows];
                // storing Each row and column value to excel sheet 
                // for (int i = 0; i < columns - 1; i++) dataGridView1.Rows.Add();
                for (int i = 0; i < columns - 1; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        obj[j] = worksheet.Cells[i + 2, j + 1].Value;
                    }
                    ResultDataSet.Tables["Table"].Rows.Add(obj);
                }

                // save the application 
                // workbook.SaveAs("c:\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // Exit from the application 
                app.Quit();
                return ResultDataSet;
            }
            catch (Exception ex)
            {
                ResultDataSet.Prefix = ex.Message;
                return ResultDataSet;
            }
        }
        public static bool ExportToExcel(string FileName, DataSet ds)
        {
            try
            {
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

                // creating new WorkBook within Excel application 

                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);

                // creating new Excelsheet in workbook 

                Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

                // see the excel sheet behind the program 

                app.Visible = true;

                // get the reference of first sheet. By default its name is Sheet1. 

                // store its reference to worksheet 

                worksheet = workbook.Sheets["Sheet1"];

                worksheet = workbook.ActiveSheet;

                // changing the name of active sheet 
                worksheet.Name = "Sheet1";

                // storing header part in Excel 
                for (int i = 1; i < ds.Tables[0].Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = ds.Tables[0].Columns[i - 1].Caption;
                }

                // storing Each row and column value to excel sheet 
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = ds.Tables[0].Rows[i][j].ToString();
                    }
                }

                // save the application 
                // workbook.SaveAs("c:\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // Exit from the application 
                app.Quit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool SaveToExcel(string FileName, DataSet ds)
        {
            try
            {
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

                // creating new WorkBook within Excel application 

                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);

                // creating new Excelsheet in workbook 

                Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

                // see the excel sheet behind the program 

                app.Visible = true;

                // get the reference of first sheet. By default its name is Sheet1. 

                // store its reference to worksheet 

                worksheet = workbook.Sheets["Sheet1"];

                worksheet = workbook.ActiveSheet;

                // changing the name of active sheet 
                worksheet.Name = "Sheet1";

                // storing header part in Excel 
                for (int i = 1; i < ds.Tables[0].Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = ds.Tables[0].Columns[i - 1].Caption;
                }

                // storing Each row and column value to excel sheet 
                for (int i = 0; i < ds.Tables[0].Rows.Count - 1; i++)
                {
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = ds.Tables[0].Rows[i][j].ToString();
                    }
                }

                // save the application 
                workbook.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // Exit from the application 
                app.Quit();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static DataTable ReadExcel(string fileName, string fileExt)
        {
            string conn = string.Empty;
            DataTable dtexcel = new DataTable();
            if (fileExt.CompareTo(".xls") == 0)
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
            else
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Sheet1$]", con); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable  
                }
                catch { }
            }
            return dtexcel;
        }
        public static void OpenExcelFile(string FileName)
        {
            try
            {
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Open(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

                app.Visible = true;

                worksheet = workbook.Sheets["Sheet1"];

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "Sheet1";

                int rows = 0, columns = 0;
                for (int i = 1; i < worksheet.Rows.Count; i++)
                {
                    try
                    {
                        if (worksheet.Cells[1, i].Value.ToString().Length != 0)
                            rows++;
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
               */
        public static bool IsDigit(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] < '0' || str[i] > '9') return false;
            return true;
        }
        public static bool IsDigit(char ch)
        {
            if (ch >= '0' && ch <= '9') return true;
            else return false;
        }
        public static string CheckSQLInjection(string strInput, SQLInjectionkind tmpKind)
        {
            try
            {
                string result = string.Empty;
                strInput.Replace("--", "/").Replace('|', '/').Replace("||", "/").Replace('&', '/')
                             .Replace("&&", "/").Replace("union", "uuuuuuu").Replace("Delete", "ddddd")
                             .Replace("add", "aaaaa").Replace("select", "ssssss").Replace("script", "sssssss")
                             .Replace('!', '/').Replace('^', 'u').Replace('#', 'u').Replace('*', 'u').Replace('(', 'u')
                             .Replace(')', 'u').Replace('%', 'u').Replace("or", "oooo").Replace("and", "aaaa").Replace("drop", "dddd")
                             .Replace("create", "ccccc").Replace("Table", "ttttt");

                switch (tmpKind)
                {
                    case SQLInjectionkind.Normal:
                        strInput.Replace('-', '/').Replace('@', 'u');
                        break;
                    case SQLInjectionkind.Name:
                        strInput.Replace('@', 'u');
                        break;
                    case SQLInjectionkind.Email:

                        break;
                }

                result = strInput;
                return result;
            }
            catch { return String.Empty; }
        }
        public static string GetExperssions(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Email: return (@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
                case ExpressionType.Currency: return (@"^(-)?\d+(\.\d\d)?$");
                case ExpressionType.Name: return (@"^[a-zA-Z''-'\s]{1,40}$");
                case ExpressionType.NoneNegativeInteger: return (@"^\d+$");
                case ExpressionType.Password: return (@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,10})$");
                case ExpressionType.PhoneNumber: return (@"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$");
                case ExpressionType.SocialSecurityNumber: return (@"^\d{3}-\d{2}-\d{4}$");
                case ExpressionType.URL: return (@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$");
                case ExpressionType.ZIPCOde: return (@"^(\d{5}-\d{4}|\d{5}|\d{9})$|^([a-zA-Z]\d[a-zA-Z] \d[a-zA-Z]\d)$");
                case ExpressionType.NoneNegativeCurrency: return (@"^(-)?\d+(\.\d\d)?$");

                default: return (@"");
            }
        }
        //public static DialogResult ShowMessage(MessageType messageType, string Name)
        //{
        //    try
        //    {
        //        switch (messageType)
        //        {
        //            case MessageType.TextInput: return (FarsiMessegeBox.Show(Name + " را وارد نمایید ", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Exclamtion));
        //            case MessageType.SelectInput: return (FarsiMessegeBox.Show(Name + " را انتخاب نمایید ", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Exclamtion));
        //            case MessageType.ErrorDelete: return (FarsiMessegeBox.Show(Name + " را انتخاب نمایید ", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Exclamtion));
        //            case MessageType.ErrorInsert: return (FarsiMessegeBox.Show("عملیات ذخیره سازی با شکست همراه بود، داده های ورودی را چک کرده و دوباره سعی نمایید.", "اخطار", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Error));
        //            case MessageType.ErrorUpdate: return (FarsiMessegeBox.Show("عملیات به روزرسانی با شکست همراه بود، داده های ورودی را چک کرده و دوباره سعی نمایید.", "اخطار", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Error));
        //            case MessageType.SuccessfullDelete: return (FarsiMessegeBox.Show("عملیات حذف اطلاعات با موفقیت انجام شد.", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Information));
        //            case MessageType.SuccessfullInsert: return (FarsiMessegeBox.Show("عملیات ذخیره سازی اطلاعات با موفقیت انجام شد.", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Information));
        //            case MessageType.SuccessfullUpdate: return (FarsiMessegeBox.Show("عملیات به روزرسانی اطلاعات با موفقیت انجام شد.", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Information));
        //            case MessageType.DeleleQuestion: return (FarsiMessegeBox.Show("آیا از حذف داده انتخاب شده مطمئن هستید؟", "سوال", FMessegeBoxButtons.YesNo, FMessegeBoxIcons.Question));
        //            case MessageType.SelectForDelete: return (FarsiMessegeBox.Show("لطفا " + Name + " که می خواهید حذف کنید را انتخاب نمایید", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Exclamtion));
        //            case MessageType.ErrorOperation: return (FarsiMessegeBox.Show("عملیات با شکست همراه بود، داده های ورودی را چک کرده و دوباره سعی نمایید.", "اخطار", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Error));
        //            case MessageType.RepeatedName: return (FarsiMessegeBox.Show(Name + " وارد شده تکراری است و قبلا ثبت شده است ", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Exclamtion));
        //            case MessageType.RepeatedNameQuestion: return (FarsiMessegeBox.Show(Name + " وارد شده تکراری است و قبلا ثبت شده است، آیا تشابه اسمی وجود دارد؟", "توجه", FMessegeBoxButtons.YesNo, FMessegeBoxIcons.Question));
        //            case MessageType.SaveOnExitQuestion: return (FarsiMessegeBox.Show("آیا می خواهید قبل از خروج تغییرات را ذخیره نمایید؟", "سوال", FMessegeBoxButtons.YesNo, FMessegeBoxIcons.Question));

        //        }
        //        return DialogResult.None;
        //    }
        //    catch
        //    {
        //        return DialogResult.None;
        //    }
        //}
        //public static void ShowMessage(MessageType messageType, string EnterString, out string ShowMessage, out Color fontColor)
        //{
        //    try
        //    {
        //        switch (messageType)
        //        {
        //            //case MessageType.TextInput: ;
        //            //case MessageType.SelectInput: return (FarsiMessegeBox.Show(messageType + " را انتخاب نمایید ", "توجه", FMessegeBoxButtons.Ok, FMessegeBoxIcons.Exclamtion));
        //            case MessageType.ErrorDelete: fontColor = Color.Red; ShowMessage = "عملیات حذف اطلاعات با شکست همراه بود دوباره تلاش نمایید"; break;
        //            case MessageType.ErrorInsert: fontColor = Color.Red; ShowMessage = "عملیات ذخیره اطلاعات با شکست همراه بود دوباره تلاش نمایید"; break;
        //            case MessageType.ErrorUpdate: fontColor = Color.Red; ShowMessage = "عملیات به روزرسانی اطلاعات با شکست همراه بود دوباره تلاش نمایید"; break;
        //            case MessageType.SuccessfullDelete: fontColor = Color.Green; ShowMessage = "حذف اطلاعات با موفقیت انجام شد"; break;
        //            case MessageType.SuccessfullInsert: fontColor = Color.Green; ShowMessage = "ذخیره اطلاعات با موفقیت انجام شد"; break;
        //            case MessageType.SuccessfullUpdate: fontColor = Color.Green; ShowMessage = "به روزرسانی اطلاعات با موفقیت انجام شد"; break;
        //            case MessageType.DeleleQuestion: fontColor = Color.Green; ShowMessage = "حذف اطلاعات با موفقیت انجام شد"; break;
        //            case MessageType.SelectForDelete: fontColor = Color.Green; ShowMessage = "حذف اطلاعات با موفقیت انجام شد"; break;
        //            case MessageType.ErrorOperation: fontColor = Color.Red; ShowMessage = "عملیات با شکست همراه بود، دوباره تلاش نمایید"; break;
        //            case MessageType.RepeatedName: fontColor = Color.Red; ShowMessage = "نام وارد شده تکراری است و قبلا ثبت شده است"; break;
        //            case MessageType.RepeatedNameQuestion: fontColor = Color.Green; ShowMessage = "حذف اطلاعات با موفقیت انجام شد"; break;
        //            case MessageType.SaveOnExitQuestion: fontColor = Color.Green; ShowMessage = "حذف اطلاعات با موفقیت انجام شد"; break;
        //            case MessageType.OldPasswordError: fontColor = Color.Red; ShowMessage = "کلمه عبور فعلی نادرست است"; break;
        //            case MessageType.ChangePasswordError: fontColor = Color.Red; ShowMessage = "تغییر کلمه عبور با شکست مواجه شد"; break;
        //            case MessageType.ChangePasswordSuccessfully: fontColor = Color.Green; ShowMessage = "کلمه عبور با موفقیت تغییر یافت"; break;
        //            case MessageType.IncorectCode: fontColor = Color.Red; ShowMessage = "کد وارد شده صحیح نمی باشد"; break;
        //            case MessageType.PasswordConfirmError: fontColor = Color.Red; ShowMessage = "تکرار کلمه عبور مطابقت ندارد"; break;
        //        }
        //        fontColor = Color.White; ShowMessage = "";
        //    }
        //    catch
        //    {
        //        fontColor = Color.White; ShowMessage = "";
        //    }
        //}
        //public static WebPictureValidity CheckImage(FileUpload fu)
        //{
        //    string[] validext = { ".jpg", ".png", ".gif", ".bmp" };
        //    string ext = Path.GetExtension(fu.PostedFile.FileName);
        //    float size = fu.PostedFile.ContentLength / 1024;
        //    if (Array.IndexOf(validext, ext.ToLower()) < 0)
        //    {
        //        return WebPictureValidity.InvalidFormat;
        //    }
        //    else if (size > 500)
        //    {
        //        return WebPictureValidity.InvalidSize;
        //    }
        //    else
        //    {
        //        return WebPictureValidity.Valid;
        //    }
        //}
        //public static string GetFileExtension(FileUpload fu)
        //{
        //    return (Path.GetExtension(fu.PostedFile.FileName));
        //}
        //public static Bitmap GetPictureOfForm(Form frm)
        //{
        //    Bitmap bmp = new Bitmap(frm.Width, frm.Height);
        //    try
        //    {
        //        bmp = new Bitmap(frm.Width, frm.Height);
        //        Rectangle r = new Rectangle(0, 0, frm.Width, frm.Height);
        //        frm.DrawToBitmap(bmp, r);
        //        return bmp;
        //    }
        //    catch { return bmp; }
        //}
        //public static Bitmap GetPictureOfForm(Form frm, int x, int y, int width, int height)
        //{
        //    Bitmap bmp = new Bitmap(10, 10);
        //    try
        //    {
        //        bmp = new Bitmap(width, height);
        //        Rectangle r = new Rectangle(x, y, width, height);
        //        frm.DrawToBitmap(bmp, r);
        //        return bmp;
        //    }
        //    catch { return bmp; }
        //}
        //public static Bitmap CropImage(Bitmap img, Rectangle cropArea)
        //{
        //    var resized = new Bitmap(10, 10);
        //    //Bitmap bmpCrop = new Bitmap(10, 10);
        //    try
        //    {
        //        bmpCrop = img.Clone(cropArea, img.PixelFormat);
        //        return bmpCrop;
        //    }
        //    catch { return bmpCrop; }
        //}
    }
    public struct CurrencyUnit
    {
        public static string SoftwareCurrencyUnit = "ریال";
        public static long ClearNumberFormat(string number)
        {
            try
            {
                if (number.Length == 0) return 0;
                bool HasMines = false;
                if (number[0] == '-') HasMines = true;
                long result = 0;
                for (int i = 0; i < number.Length; i++)
                {
                    if (number[i] >= '0' && number[i] <= '9')
                        result = result * 10 + int.Parse(number[i].ToString());
                }
                if (HasMines) result *= (-1);
                return result;
            }
            catch
            {
                return 0;
            }
        }
        public static long ClearNumberFormat(object obj)
        {
            try
            {
                if (obj.ToString().Length == 0) return 0;
                string number = obj.ToString();
                bool HasMines = false;
                if (number[0] == '-') HasMines = true;
                long result = 0;
                for (int i = 0; i < number.Length; i++)
                {
                    if (number[i] >= '0' && number[i] <= '9')
                        result = result * 10 + int.Parse(number[i].ToString());
                }
                if (HasMines) result *= (-1);
                return result;
            }
            catch
            {
                return 0;
            }
        }
        public static string CreateNumberFormat(int number)
        {
            string result = "", FinalResult = "";
            bool HasMines = false;
            if (number < 0)
            {
                HasMines = true;
                number = (-1) * number;
            }
            string num = number.ToString();
            int j = 0;
            for (int p = num.Length - 1; p >= 0; j++)
            {
                if ((j % 3 == 0) && (j > 0))
                {
                    result += ",";
                    j = -1;
                }
                else
                {
                    result += num[p--];
                }
            }
            for (int i = result.Length - 1; i >= 0; i--) FinalResult += result[i];
            if (HasMines) FinalResult = "-" + FinalResult;
            return (FinalResult + " " + SoftwareCurrencyUnit);
        }
        public static string CreateNumberFormat(long number)
        {
            string result = "", FinalResult = "";
            bool HasMines = false;
            if (number < 0)
            {
                HasMines = true;
                number = (-1) * number;
            }
            string num = number.ToString();
            int j = 0;
            for (int p = num.Length - 1; p >= 0; j++)
            {
                if ((j % 3 == 0) && (j > 0))
                {
                    result += ",";
                    j = -1;
                }
                else
                {
                    result += num[p--];
                }
            }
            for (int i = result.Length - 1; i >= 0; i--) FinalResult += result[i];
            if (HasMines) FinalResult = "-" + FinalResult;

            return (FinalResult + " " + SoftwareCurrencyUnit);
        }
        public static string CreateNumberFormat(double number)
        {
            string result = "", FinalResult = "";
            bool HasMines = false;
            if (number < 0)
            {
                HasMines = true;
                number = (-1) * number;
            }
            long TempNumber = (long)number;
            string num = TempNumber.ToString();
            int j = 0;
            for (int p = num.Length - 1; p >= 0; j++)
            {
                if ((j % 3 == 0) && (j > 0))
                {
                    result += ",";
                    j = -1;
                }
                else
                {
                    result += num[p--];
                }
            }
            for (int i = result.Length - 1; i >= 0; i--) FinalResult += result[i];
            if (HasMines) FinalResult = "-" + FinalResult;

            return (FinalResult + " " + SoftwareCurrencyUnit);
        }
        public static string CreateNumberFormatWithoutUnit(int number)
        {
            string result = "", FinalResult = "";
            bool HasMines = false;
            if (number < 0)
            {
                HasMines = true;
                number = (-1) * number;
            }
            string num = number.ToString();
            int j = 0;
            for (int p = num.Length - 1; p >= 0; j++)
            {
                if ((j % 3 == 0) && (j > 0))
                {
                    result += ",";
                    j = -1;
                }
                else
                {
                    result += num[p--];
                }
            }
            for (int i = result.Length - 1; i >= 0; i--) FinalResult += result[i];
            if (HasMines) FinalResult = "-" + FinalResult;
            return (FinalResult);
        }
        public static string CreateNumberFormatWithoutUnit(long number)
        {
            string result = "", FinalResult = "";
            bool HasMines = false;
            if (number < 0)
            {
                HasMines = true;
                number = (-1) * number;
            }
            string num = number.ToString();
            int j = 0;
            for (int p = num.Length - 1; p >= 0; j++)
            {
                if ((j % 3 == 0) && (j > 0))
                {
                    result += ",";
                    j = -1;
                }
                else
                {
                    result += num[p--];
                }
            }
            for (int i = result.Length - 1; i >= 0; i--) FinalResult += result[i];
            if (HasMines) FinalResult = "-" + FinalResult;

            return (FinalResult);
        }
        public static string CreateNumberFormatWithoutUnit(double number)
        {
            string result = "", FinalResult = "";
            bool HasMines = false;
            if (number < 0)
            {
                HasMines = true;
                number = (-1) * number;
            }
            long TempNumber = (long)number;
            string num = TempNumber.ToString();
            int j = 0;
            for (int p = num.Length - 1; p >= 0; j++)
            {
                if ((j % 3 == 0) && (j > 0))
                {
                    result += ",";
                    j = -1;
                }
                else
                {
                    result += num[p--];
                }
            }
            for (int i = result.Length - 1; i >= 0; i--) FinalResult += result[i];
            if (HasMines) FinalResult = "-" + FinalResult;

            return (FinalResult);
        }
        public static string ConvertLatinNumberToFarsi(string num)
        {
            string result = string.Empty;
            foreach (char c in num.ToCharArray())
            {
                switch (c)
                {
                    case '0':
                        result += "٠";
                        break;
                    case '1':
                        result += "١";
                        break;
                    case '2':
                        result += "٢";
                        break;
                    case '3':
                        result += "٣";
                        break;
                    case '4':
                        result += "۴";
                        break;
                    case '5':
                        result += "۵";
                        break;
                    case '6':
                        result += "۶";
                        break;
                    case '7':
                        result += "٧";
                        break;
                    case '8':
                        result += "٨";
                        break;
                    case '9':
                        result += "٩";
                        break;
                    default:
                        result += c;
                        break;

                }
            }
            return result;
        }
        public static string ConvertFarsiNumberToLatin(string num)
        {
            string result = string.Empty;
            foreach (char c in num.ToCharArray())
            {
                switch (c)
                {
                    case '٠':
                        result += "0";
                        break;
                    case '١':
                        result += "1";
                        break;
                    case '٢':
                        result += "2";
                        break;
                    case '٣':
                        result += "3";
                        break;
                    case '۴':
                        result += "4";
                        break;
                    case '۵':
                        result += "5";
                        break;
                    case '۶':
                        result += "6";
                        break;
                    case '٧':
                        result += "7";
                        break;
                    case '٨':
                        result += "8";
                        break;
                    case '٩':
                        result += "9";
                        break;
                    default:
                        result += c;
                        break;
                }
            }
            return result;
        }
        #region NumToString
        private static string[] yakan = new string[10] { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        private static string[] dahgan = new string[10] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static string[] dahyek = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static string[] sadgan = new string[10] { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static string[] basex = new string[5] { "", "هزار", "میلیون", "میلیارد", "تریلیون" };
        private static string getnum3(long num3)
        {
            string s = "";
            long d3, d12;
            d12 = num3 % 100;
            d3 = num3 / 100;
            if (d3 != 0)
                s = sadgan[d3] + " و ";
            if ((d12 >= 10) && (d12 <= 19))
            {
                s = s + dahyek[d12 - 10];
            }
            else
            {
                long d2 = d12 / 10;
                if (d2 != 0)
                    s = s + dahgan[d2] + " و ";
                long d1 = d12 % 10;
                if (d1 != 0)
                    s = s + yakan[d1] + " و ";
                s = s.Substring(0, s.Length - 3);
            };
            return s;
        }
        public static string NumberToPersianString(long number)
        {
            string snum = number.ToString();
            string stotal = "";
            if (snum == "") return "صفر";
            if (snum == "0")
            {
                return yakan[0];
            }
            else
            {
                snum = snum.PadLeft(((snum.Length - 1) / 3 + 1) * 3, '0');
                int L = snum.Length / 3 - 1;
                for (int i = 0; i <= L; i++)
                {
                    long b = int.Parse(snum.Substring(i * 3, 3));
                    if (b != 0)
                        stotal = stotal + getnum3(b) + " " + basex[L - i] + " و ";
                }
                stotal = stotal.Substring(0, stotal.Length - 3);
            }
            return stotal;
        }
        public static string NumberToPersianStringMix(long number)
        {
            string result = string.Empty;
            long temp = number;
            while (temp > 1000)
            {

                if (temp < 1000000)
                    result += temp / 1000 + " هزار ";
                else result += temp / 1000 + " میلیون ";
                temp /= 1000000;
                if (temp > 1000) result += " و ";
            }
            result += temp.ToString();
            result += " " + SoftwareCurrencyUnit;
            //  result = result.Substring(2,result.Length-2);
            return result;
        }
        #endregion
    }
    public struct PersianDate
    {
        private int _Day;
        public int Day
        {
            get { return _Day; }

            set { if (value < 32 && value > 0) _Day = value; }
        }
        public static double GetUnixTimestamp()
        {
            return (TimeZoneInfo.ConvertTimeToUtc(DateTime.Now) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
        public static double ConvertGetUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
        public static PersianDate ConvertUnixTimeStampToDate(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            PersianDate pd = PersianDate.ConvertMiladiToPersianDate(dtDateTime.Date);
            return pd;
        }
        public static PersianTime ConvertUnixTimeStampToTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            PersianTime pd = PersianTime.SetTime(dtDateTime.Hour - 1, dtDateTime.Minute, dtDateTime.Second);
            return pd;
        }
        public static string GetFullDateString(PersianDate pd)
        {
            string result = string.Empty;
            result = GetDayName(pd) + "، " + pd.Day.ToString() + " " + GetMonthName(pd) + " " + pd.Year;
            return result;
        }
        private int _Month;
        public int Month
        {
            get { return _Month; }
            set { if (value < 13 && value > 0) _Month = value; }
        }

        private int _year;
        public int Year
        {
            get { return _year; }
            set { if (value >= 1300 && value <= 1500) _year = value; else _year = 1300; }
        }
        public static int GetNumberOfDayInMonth(PersianDate pd)
        {
            if (pd.Month < 7) return 31;
            if (pd.Month < 12) return 30;
            if (IsKabise(pd.Year)) return 30;
            else return 29;
        }
        public static PersianDate GetHijriDate()
        {
            HijriCalendar hc = new HijriCalendar();
            int year, month, day;
            year = hc.GetYear(DateTime.Now);
            month = hc.GetMonth(DateTime.Now);
            day = hc.GetDayOfMonth(DateTime.Now);
            string d = year.ToString() + "-" + month.ToString() + "-" + day.ToString();
            PersianDate pdt = new PersianDate();
            pdt.Day = day; pdt.Month = month; pdt.Year = year;

            return (pdt);
        }
        public static PersianDate ConvertMiladiToPersianDate(DateTime dt)
        {
            PersianCalendar pc = new PersianCalendar();
            int year, month, day;
            year = pc.GetYear(dt);
            month = pc.GetMonth(dt);
            day = pc.GetDayOfMonth(dt);
            string d = year.ToString() + "-" + month.ToString() + "-" + day.ToString();
            PersianDate pdt = new PersianDate();
            pdt.Day = day; pdt.Month = month; pdt.Year = year;

            return (pdt);
        }
        public static DateTime ConvertPersianToMiladiDate(PersianDate pd)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dtresult = pc.ToDateTime(pd.Year, pd.Month, pd.Day, 0, 0, 0, 0);
            return dtresult;
        }
        public static PersianDate ConvertPersianToHijriDate(PersianDate pd)
        {
            PersianCalendar pc = new PersianCalendar();
            HijriCalendar hc = new HijriCalendar();
            DateTime dt = pc.ToDateTime(pd.Year, pd.Month, pd.Day, 0, 0, 0, 0);
            PersianDate ptemp = new PersianDate();
            ptemp.Year = hc.GetYear(dt); ptemp.Month = hc.GetMonth(dt); ptemp.Day = hc.GetDayOfMonth(dt);
            return ptemp;
        }
        public static PersianDate ConvertHijriToPersianDate(PersianDate pd)
        {
            PersianCalendar pc = new PersianCalendar();
            HijriCalendar hc = new HijriCalendar();
            DateTime dt = hc.ToDateTime(pd.Year, pd.Month, pd.Day, 0, 0, 0, 0);
            PersianDate ptemp = new PersianDate();
            ptemp.Year = pc.GetYear(dt); ptemp.Month = pc.GetMonth(dt); ptemp.Day = pc.GetDayOfMonth(dt);
            return ptemp;
        }
        public static PersianDate GetStartDate()
        {
            PersianDate pt = new PersianDate();
            pt.Year = 1380;
            pt.Month = 1;
            pt.Day = 1;
            return pt;
        }
        public static PersianDate GetEndDate()
        {
            PersianDate pt = new PersianDate();
            pt.Year = 1500;
            pt.Month = 1;
            pt.Day = 1;
            return pt;
        }
        public static PersianDate Now()
        {
            PersianCalendar pc = new PersianCalendar();
            int year, month, day;
            year = pc.GetYear(DateTime.Now);
            month = pc.GetMonth(DateTime.Now);
            day = pc.GetDayOfMonth(DateTime.Now);
            string d = String.Format("{0}-{1}-{2}", year, month, day);
            PersianDate pdt = new PersianDate();
            pdt.Day = day; pdt.Month = month; pdt.Year = year;

            return (pdt);
        }
        public static string NowString()
        {
            PersianCalendar pc = new PersianCalendar();
            string year, month, day;
            year = pc.GetYear(DateTime.Now).ToString();

            month = pc.GetMonth(DateTime.Now).ToString();
            if (month.Length == 1) month = "0" + month;
            day = pc.GetDayOfMonth(DateTime.Now).ToString();
            if (day.Length == 1) day = "0" + day;
            string d = String.Format("{0}-{1}-{2}", year, month, day);
            return (d);
        }
        public static string NowStringPersianCharFormat()
        {
            PersianCalendar pc = new PersianCalendar();
            string year, month, day;
            year = pc.GetYear(DateTime.Now).ToString();

            month = pc.GetMonth(DateTime.Now).ToString();
            if (month.Length == 1) month = "0" + month;
            day = pc.GetDayOfMonth(DateTime.Now).ToString();
            if (day.Length == 1) day = "0" + day;
            year = CurrencyUnit.ConvertLatinNumberToFarsi(year.ToString());
            month = CurrencyUnit.ConvertLatinNumberToFarsi(month.ToString());
            day = CurrencyUnit.ConvertLatinNumberToFarsi(day.ToString());

            string d = String.Format("{0}-{1}-{2}", year, month, day);
            return (d);
        }
        public static string ConvertToPersianCharFormat(PersianDate pd)
        {

            string year, month, day;
            year = pd.Year.ToString();

            month = pd.Month.ToString();
            if (month.Length == 1) month = "0" + month;
            day = pd.Day.ToString();
            if (day.Length == 1) day = "0" + day;
            year = CurrencyUnit.ConvertLatinNumberToFarsi(year[0].ToString()) + CurrencyUnit.ConvertLatinNumberToFarsi(year[1].ToString()) + CurrencyUnit.ConvertLatinNumberToFarsi(year[2].ToString()) + CurrencyUnit.ConvertLatinNumberToFarsi(year[3].ToString());
            month = CurrencyUnit.ConvertLatinNumberToFarsi(month[0].ToString()) + CurrencyUnit.ConvertLatinNumberToFarsi(month[1].ToString());
            day = CurrencyUnit.ConvertLatinNumberToFarsi(day[0].ToString()) + CurrencyUnit.ConvertLatinNumberToFarsi(day[1].ToString());


            string d = String.Format("{0}-{1}-{2}", year, month, day);
            return (d);
        }
        public static PersianDate Parse(string date)
        {
            date = date.Trim();
            PersianDate pd = new PersianDate();
            if (date.Length != 0)
            {
                char[] chSeperator = new char[7];
                chSeperator[0] = ':'; chSeperator[1] = '-'; chSeperator[2] = '/'; chSeperator[3] = ','; chSeperator[4] = '.'; chSeperator[5] = '_'; chSeperator[6] = ' ';
                string[] st = date.Split(chSeperator);

                if (int.Parse(st[0]) <= 31)
                {
                    pd.Year = int.Parse(st[2]);
                    pd.Month = int.Parse(st[0]);
                    pd.Day = int.Parse(st[1]);
                }
                else
                {
                    pd.Year = int.Parse(st[0]);
                    pd.Month = int.Parse(st[1]);
                    pd.Day = int.Parse(st[2]);
                }
            }
            else
            {
                pd.Year = 0;
                pd.Month = 0;
                pd.Day = 0;
            }
            return (pd);
        }
        public static PersianDate Parse(object date)
        {
            string d = date.ToString();
            char[] chSeperator = new char[6];
            chSeperator[0] = ':'; chSeperator[1] = '-'; chSeperator[2] = '/'; chSeperator[3] = ','; chSeperator[4] = '.'; chSeperator[5] = '_';
            string[] st = d.Split(chSeperator);
            PersianDate pd = new PersianDate();
            pd.Year = int.Parse(st[0]);
            pd.Month = int.Parse(st[1]);
            pd.Day = int.Parse(st[2]);
            return (pd);
        }
        public override string ToString()
        {
            string year, month, day;
            year = Year.ToString();
            month = Month.ToString();
            if (month.Length == 1) month = "0" + month;
            day = Day.ToString();
            if (day.Length == 1) day = "0" + day;
            return (String.Format("{0}-{1}-{2}", year, month, day));
        }
        public string ToReverseString()
        {
            string month, day;
            string year = Year.ToString();
            month = Month.ToString();
            if (month.Length == 1) month = "0" + month;
            day = Day.ToString();
            if (day.Length == 1) day = "0" + day;
            return (String.Format("{0}-{1}-{2}", day, month, year));
        }
        public static string GetMonthName(int index)
        {
            switch (index)
            {
                case 1: return "فروردین";
                case 2: return "اردیبهشت";
                case 3: return "خرداد";
                case 4: return "تیر";
                case 5: return "مرداد";
                case 6: return "شهریور";
                case 7: return "مهر";
                case 8: return "آبان";
                case 9: return "آذر";
                case 10: return "دی";
                case 11: return "بهمن";
                case 12: return "اسفند";
            }
            return string.Empty;
        }
        public static string GetMonthName(PersianDate pd)
        {
            return (PersianDate.GetMonthName(pd.Month));
        }
        public static string GetWeekName(int index)
        {
            switch (index)
            {
                case 1: return "شنبه";
                case 2: return "یک شنبه";
                case 3: return "دو شنبه";
                case 4: return "سه شنبه";
                case 5: return "چهار شنبه";
                case 6: return "پنج شنبه";
                case 7: return "جمعه";
            }
            return string.Empty;
        }
        public static int GetWeekDayIndex(string dayName)
        {
            switch (dayName)
            {
                case "شنبه": return 1;
                case "یک شنبه": return 2;
                case "دوشنبه":
                case "دو شنبه": return 3;
                case "سه شنبه": return 4;
                case "چهارشنبه":
                case "چهار شنبه": return 5;
                case "پنج شنبه": return 6;
                case "جمعه": return 7;
            }
            return 0;
        }
        public static bool IsBigger(PersianDate d1, PersianDate d2)
        {
            if (d1.Year < d2.Year) return false;
            if (d1.Year > d2.Year) return true;
            if (d1.Month < d2.Month) return false;
            if (d1.Month > d2.Month) return true;
            if (d1.Day < d2.Day) return false;
            return true;
        }
        public static bool IsBiggerEqual(PersianDate d1, PersianDate d2)
        {
            if (DateTime.Compare(PersianDate.ConvertPersianToMiladiDate(d1), PersianDate.ConvertPersianToMiladiDate(d2)) >= 0)
                return true;

            return false;
            //if (d1.Year < d2.Year) return false;
            //if (d1.Year > d2.Year) return true;
            //if (d1.Month < d2.Month) return false;
            //if (d1.Month > d2.Month) return true;
            //if (d1.Day < d2.Day) return false;

            //return true;
        }
        public static bool IsEqual(PersianDate d1, PersianDate d2)
        {
            if ((d1.Year == d2.Year) && (d1.Month == d2.Month) && (d1.Day == d2.Day)) return true;
            return false;
        }
        public static PersianDate NMonthAfter(PersianDate pd, int n)
        {
            for (int i = 0; i < n; i++)
                pd = NextMonth(pd);

            return pd;
        }
        public static PersianDate NextMonth(PersianDate pd)
        {
            ArrayList KabiseList = GetKabiseYears();
            if (pd.Month < 12) pd.Month++;
            else { pd.Month = 1; pd.Year++; }

            if (pd.Month > 6 && pd.Day == 31) pd.Day = 30;
            if (pd.Month == 12 && pd.Day == 30 && !KabiseList.Contains(pd.Year)) pd.Day = 29;
            return pd;
        }
        public static PersianDate NMonthBefore(PersianDate pd, int n)
        {
            for (int i = 0; i < n; i++)
                pd = LastMonth(pd);

            return pd;
        }
        public static PersianDate LastMonth(PersianDate pd)
        {
            ArrayList KabiseList = GetKabiseYears();

            if (pd.Month > 1) pd.Month--;
            else { pd.Month = 12; pd.Year--; }

            if (pd.Month > 6 && pd.Day == 31) pd.Day = 30;
            if (pd.Month == 12 && pd.Day == 30 && !KabiseList.Contains(pd.Year)) pd.Day = 29;

            return pd;
        }
        public static PersianDate NDayAfter(PersianDate pd, int n)
        {
            for (int i = 0; i < n; i++)
                pd = GetNextDay(pd);

            return pd;
        }
        public static PersianDate NDayBefore(PersianDate pd, int n)
        {
            for (int i = 0; i < n; i++)
                pd = GetPreviousDay(pd);

            return pd;
        }
        public static PersianDate GetNextDay(PersianDate pd)
        {
            ArrayList KabiseList = GetKabiseYears();

            if (pd.Day < 29) { pd.Day++; return pd; }
            if (pd.Day == 29 && pd.Month < 12) { pd.Day++; return pd; }
            if (pd.Day == 29 && pd.Month == 12 && KabiseList.Contains(pd.Year)) { pd.Day++; return pd; }
            if (pd.Day == 29 && pd.Month == 12)
            {
                pd.Day = 1;
                if (pd.Month < 12) { pd.Month++; return pd; }
                else { pd.Month = 1; pd.Year++; return pd; }
            }
            if (pd.Day == 30 && pd.Month == 12)
            {
                pd.Day = 1;
                pd.Month = 1; pd.Year++; return pd;
            }
            if (pd.Day == 30 && pd.Month > 6) { pd.Day = 1; pd.Month++; return pd; }
            if (pd.Day == 30 && pd.Month < 7) { pd.Day++; return pd; }
            if (pd.Day == 31) { pd.Day = 1; pd.Month++; return pd; }
            return pd;
        }
        public static ArrayList GetKabiseYears()
        {
            ArrayList KabiseList = new ArrayList();
            KabiseList.Add(1391); KabiseList.Add(1395); KabiseList.Add(1399); KabiseList.Add(1404); KabiseList.Add(1408); KabiseList.Add(1412); KabiseList.Add(1416);
            KabiseList.Add(1420); KabiseList.Add(1424); KabiseList.Add(1428); KabiseList.Add(1432); KabiseList.Add(1437); KabiseList.Add(1441); KabiseList.Add(1445);
            KabiseList.Add(1449); KabiseList.Add(1453); KabiseList.Add(1457); KabiseList.Add(1461); KabiseList.Add(1465); KabiseList.Add(1470); KabiseList.Add(1474);
            KabiseList.Add(1478); KabiseList.Add(1482); KabiseList.Add(1486); KabiseList.Add(1490); KabiseList.Add(1494); KabiseList.Add(1498);
            return KabiseList;
        }
        public static PersianDate GetPreviousDay(PersianDate pd)
        {
            ArrayList KabiseList = GetKabiseYears();

            if (pd.Day > 1) { pd.Day--; return pd; }
            if (pd.Day == 1 && pd.Month > 6) { pd.Day = 30; pd.Month--; return pd; }
            if (pd.Day == 1 && pd.Month > 1) { pd.Day = 31; pd.Month--; return pd; }
            if (pd.Day == 1 && pd.Month == 1)
            {
                if (KabiseList.Contains(pd.Year - 1))
                {
                    pd.Day = 30;
                    pd.Month = 12;
                    pd.Year--;
                    return pd;
                }
                else
                {
                    pd.Day = 29;
                    pd.Month = 12;
                    pd.Year--;
                    return pd;
                }
            }
            return pd;
        }
        public static int GetDayOfYear(PersianDate pd)
        {
            if (pd.Month < 8) return ((pd.Month - 1) * 31 + pd.Day);
            return (186 + (pd.Month - 7) * 30 + pd.Day);
        }
        public static int GetDiffrentDay(PersianDate pd1, PersianDate pd2)
        {
            if (!IsBigger(pd1, pd2))
            {
                PersianDate temp = pd1;
                pd1 = pd2;
                pd2 = temp;
            }
            int different = 0;
            if (pd1.Year == pd2.Year) return (GetDayOfYear(pd1) - GetDayOfYear(pd2));
            else if (pd1.Year == (pd2.Year + 1))
            {
                if (IsKabise(pd2.Year))
                    return (GetDayOfYear(pd1) + (366 - GetDayOfYear(pd2)));
                else
                    return (GetDayOfYear(pd1) + (365 - GetDayOfYear(pd2)));
            }
            else
            {
                for (int i = pd2.Year + 1; i < pd1.Year; i++)
                    if (IsKabise(i))
                        different += 366;
                    else
                        different += 365;
                if (IsKabise(pd2.Year))
                    return (GetDayOfYear(pd1) + (366 - GetDayOfYear(pd2)) + different);
                else
                    return (GetDayOfYear(pd1) + (365 - GetDayOfYear(pd2)) + different);
            }
        }
        public static string GetDayName(PersianDate pd)
        {
            PersianDate baseDate = PersianDate.Parse("1392-07-20");

            int diff = GetDiffrentDay(PersianDate.Parse("1390-01-06"), pd);
            switch (diff % 7)
            {
                case 0: return "شنبه";
                case 1: return "یک شنبه";
                case 2: return "دو شنبه";
                case 3: return "سه شنبه";
                case 4: return "چهار شنبه";
                case 5: return "پنج شنبه";
                case 6: return "جمعه";
            }
            return "Error";
        }
        public static DayOfWeek GetDayOfWeekName(PersianDate pd)
        {
            PersianDate baseDate = PersianDate.Parse("1392-07-20");

            int diff = GetDiffrentDay(PersianDate.Parse("1390-01-06"), pd);
            switch (diff % 7)
            {
                case 0: return DayOfWeek.Saturday;
                case 1: return DayOfWeek.Sunday;
                case 2: return DayOfWeek.Monday;
                case 3: return DayOfWeek.Tuesday;
                case 4: return DayOfWeek.Wednesday;
                case 5: return DayOfWeek.Thursday;
                case 6: return DayOfWeek.Friday;

            }
            return DayOfWeek.Friday;
        }
        public static PersianDate GetFirstDayOfMonth(PersianDate pd)
        {
            pd.Day = 1;
            return pd;
        }
        public static PersianDate GetFirstDayOfYear(PersianDate pd)
        {
            pd.Day = 1;
            pd.Month = 1;
            return pd;
        }
        public static PersianDate GetFirstDayOfWeek(PersianDate pd)
        {
            string dayName = GetDayName(pd);
            switch (dayName)
            {
                case "شنبه": return (pd);
                case "یک شنبه": return (PersianDate.NDayBefore(pd, 1));
                case "دو شنبه": return (PersianDate.NDayBefore(pd, 2));
                case "سه شنبه": return (PersianDate.NDayBefore(pd, 3));
                case "چهار شنبه": return (PersianDate.NDayBefore(pd, 4));
                case "پنج شنبه": return (PersianDate.NDayBefore(pd, 5));
                case "جمعه": return (PersianDate.NDayBefore(pd, 6));
            }
            return (pd);
        }
        public static PersianDate GetEndDayOfWeek(PersianDate pd)
        {
            string dayName = GetDayName(pd);
            switch (dayName)
            {
                case "شنبه": return (PersianDate.NDayAfter(pd, 6));
                case "یک شنبه": return (PersianDate.NDayAfter(pd, 5));
                case "دو شنبه": return (PersianDate.NDayAfter(pd, 4));
                case "سه شنبه": return (PersianDate.NDayAfter(pd, 3));
                case "چهار شنبه": return (PersianDate.NDayAfter(pd, 2));
                case "پنج شنبه": return (PersianDate.NDayAfter(pd, 1));
                case "جمعه": return (pd);
            }
            return (pd);
        }
        public static PersianDate GetEndDayOfMonth(PersianDate pd)
        {
            if (pd.Month < 7) { pd.Day = 31; return pd; }
            if (pd.Month < 12) { pd.Day = 30; return pd; }
            if (pd.Month == 12 && IsKabise(pd.Year)) { pd.Day = 30; return pd; }
            else { pd.Day = 29; return pd; }
        }
        public static PersianDate GetEndDayOfYear(PersianDate pd)
        {
            pd.Month = 12;
            if (IsKabise(pd.Year)) { pd.Day = 30; return pd; }
            else { pd.Day = 29; return pd; }
        }
        public static bool IsKabise(int year)
        {
            ArrayList KabiseList = GetKabiseYears();
            if (KabiseList.Contains(year)) return true;
            else return false;
        }
        public static PersianDate CorectFormat(string str)
        {
            str = str.Trim();
            PersianDate pd = new PersianDate();
            if (Utility.IsDigit(str[0]) && Utility.IsDigit(str[1]) && Utility.IsDigit(str[2]) && Utility.IsDigit(str[3]))
                pd.Year = int.Parse((str[0] + str[1] + str[2] + str[3]).ToString());
            if (Utility.IsDigit(str[5]) && Utility.IsDigit(str[6]))
                pd.Month = int.Parse((str[5] + str[6]).ToString());
            if (Utility.IsDigit(str[5]) && !Utility.IsDigit(str[6]))
                pd.Month = int.Parse(('0' + str[5]).ToString());

            return pd;
        }
        public static PersianDate SetDate(int year, int month, int day)
        {
            PersianDate pd = new PersianDate();
            pd.Year = year; pd.Month = month; pd.Day = day;
            return (pd);
        }
        public int ConvertToMounth()
        {
            int Mounth = 0;
            Mounth = _year * 60 + _Month;
            return Mounth;
        }
        public static string GetDateInStringFormat(PersianDate pd)
        {
            string year = string.Empty;
            string month = string.Empty;
            string day = string.Empty;
            year = CurrencyUnit.NumberToPersianString(pd.Year);
            month = GetMonthName(pd.Month);
            day = CurrencyUnit.NumberToPersianString(pd.Day);

            return (day + " " + month + " ماه " + year);
        }
        public static bool IsBetween(PersianDate betweenDate, PersianDate startDate, PersianDate EndDate)
        {
            if (PersianDate.IsBigger(betweenDate, startDate) && PersianDate.IsBigger(EndDate, betweenDate)) return true;
            else return false;
        }
        public static bool IsBetween(PersianDate betweenStartDate, PersianDate betweenEndDate, PersianDate startDate, PersianDate EndDate)
        {
            if (PersianDate.IsBigger(betweenStartDate, startDate) && PersianDate.IsBigger(EndDate, betweenEndDate)) return true;
            else return false;
        }

    }
    public struct PersianTime
    {
        private int _second;
        public int Second
        {
            get { return _second; }

            set { if (value < 60 && value >= 0) _second = value; }
        }

        private int _minute;
        public int Minute
        {
            get { return _minute; }
            set { if (value < 60 && value >= 0) _minute = value; else _minute = 1390; }
        }

        private int _hour;
        public int Hour
        {
            get { return _hour; }
            set { if (value < 24 && value >= 0) _hour = value; }
        }
        private int _day;
        public int Day
        {
            get { return _day; }
            set { if (value >= 0) _day = value; }
        }
        public static PersianTime ConvertSecondToPersianTime(int second)
        {
            PersianTime pt = new PersianTime();
            pt.Day = second / 86400;
            second = second % 86400;
            pt.Hour = second / 3600;
            int temp = second % 3600;
            pt.Minute = temp / 60;
            pt.Second = temp % 60;
            return pt;
        }
        public static PersianTime GetStartTime()
        {
            PersianTime pt = new PersianTime();
            pt.Hour = 0;
            pt.Minute = 0;
            pt.Second = 0;
            return pt;
        }
        public static PersianTime GetEndTime()
        {
            PersianTime pt = new PersianTime();
            pt.Hour = 23;
            pt.Minute = 59;
            pt.Second = 59;
            return pt;
        }
        public override string ToString()
        {
            string hour, minute, second;
            hour = Hour.ToString();
            if (hour.Length == 1) hour = "0" + hour;
            minute = Minute.ToString();
            if (minute.Length == 1) minute = "0" + minute;
            second = Second.ToString();
            if (second.Length == 1) second = "0" + second;
            return (hour + ":" + minute + ":" + second);
        }
        public static string ToStringWithDash()
        {
            string Hour, Minute, Second;
            Hour = DateTime.Now.Hour.ToString();
            if (Hour.Length == 1) Hour = "0" + Hour;
            Minute = DateTime.Now.Minute.ToString();
            if (Minute.Length == 1) Minute = "0" + Minute;
            Second = DateTime.Now.Second.ToString();
            if (Second.Length == 1) Second = "0" + Second;
            return (String.Format("{0}-{1}-{2}", Hour, Minute, Second));

        }
        public static PersianTime Now()
        {
            PersianTime pt = new PersianTime();
            pt.Second = DateTime.Now.Second;
            pt.Minute = DateTime.Now.Minute;
            pt.Hour = DateTime.Now.Hour;
            return (pt);
        }
        public static string NowString()
        {
            string Hour, Minute, Second;
            Hour = DateTime.Now.Hour.ToString();
            if (Hour.Length == 1) Hour = "0" + Hour;
            Minute = DateTime.Now.Minute.ToString();
            if (Minute.Length == 1) Minute = "0" + Minute;
            Second = DateTime.Now.Second.ToString();
            if (Second.Length == 1) Second = "0" + Second;
            return (Hour + ":" + Minute + ":" + Second);
        }
        public static string NowStringInPersianCharFormat()
        {
            string Hour, Minute, Second;
            Hour = DateTime.Now.Hour.ToString();
            if (Hour.Length == 1) Hour = "0" + Hour;
            Minute = DateTime.Now.Minute.ToString();
            if (Minute.Length == 1) Minute = "0" + Minute;
            Second = DateTime.Now.Second.ToString();
            if (Second.Length == 1) Second = "0" + Second;
            Hour = CurrencyUnit.ConvertLatinNumberToFarsi(Hour);
            Minute = CurrencyUnit.ConvertLatinNumberToFarsi(Minute);
            Second = CurrencyUnit.ConvertLatinNumberToFarsi(Second);

            return (Hour + ":" + Minute + ":" + Second);
        }
        public static string ConvertToPersianCharFormat(PersianTime pt)
        {
            string Hour, Minute, Second;
            Hour = pt.Hour.ToString();
            if (Hour.Length == 1) Hour = "0" + Hour;
            Minute = pt.Minute.ToString();
            if (Minute.Length == 1) Minute = "0" + Minute;
            Second = pt.Second.ToString();
            if (Second.Length == 1) Second = "0" + Second;
            Hour = CurrencyUnit.ConvertLatinNumberToFarsi(Hour);
            Minute = CurrencyUnit.ConvertLatinNumberToFarsi(Minute);
            Second = CurrencyUnit.ConvertLatinNumberToFarsi(Second);

            return (Hour + ":" + Minute + ":" + Second);
        }
        public static PersianTime Parse(string time)
        {
            time = time.Trim();
            PersianTime pt = new PersianTime();
            try
            {
                char[] chSeperator = new char[1];
                chSeperator[0] = ':';
                string[] st = time.Split(chSeperator);
                pt.Hour = int.Parse(st[0].ToString());
                pt.Minute = int.Parse(st[1].ToString());
                pt.Second = int.Parse(st[2].ToString());
                return (pt);
            }
            catch
            {
                return (pt);
            }
        }
        public static string GetTimeSpecial(PersianTime pt)
        {
            try
            {
                string result = "";
                if (pt._hour > 12)
                {
                    result += (pt._hour - 12).ToString();
                    result += ":" + pt._minute;
                    result += " PM";
                }
                else
                {
                    result += pt._hour.ToString();
                    result += ":" + pt._minute;
                    result += " AM";
                }

                return result;
            }
            catch (Exception)
            {
                return pt.ToString();
            }
        }
        public static PersianTime ParseSpecial(string time)
        {
            time = time.Trim();
            PersianTime pt = new PersianTime();
            string[] arr;
            try
            {
                if (time.Contains("AM"))
                {
                    time.Replace("AM", "");
                    arr = time.Split(new string[] { ":" }, StringSplitOptions.None);
                    pt.Hour = int.Parse(arr[0]) + 12;
                }
                else
                {
                    time.Replace("PM", "");
                    arr = time.Split(new string[] { ":" }, StringSplitOptions.None);
                    pt.Hour = int.Parse(arr[0]);
                }
                pt.Minute = int.Parse(arr[1]);
                pt.Second = 0;
                return (pt);
            }
            catch
            {
                return (pt);
            }
        }
        public static PersianTime Parse(object time)
        {
            string t = time.ToString();
            char[] chSeperator = new char[1];
            chSeperator[0] = ':';
            string[] st = t.Split(chSeperator);
            PersianTime pt = new PersianTime();
            pt.Hour = int.Parse(st[0].ToString());
            pt.Minute = int.Parse(st[1].ToString());
            pt.Second = int.Parse(st[2].ToString());
            return (pt);
        }
        public static PersianTime TimeDiffrent(PersianTime time1, PersianTime time2)
        {
            PersianTime timeResult = new PersianTime();
            int barrow = 0;
            if ((time1.Hour < time2.Hour) || (time1.Hour == time2.Hour && time1.Minute < time2.Minute) || (time1.Hour == time2.Hour && time1.Minute == time2.Minute && time1.Second < time2.Second))
            {
                timeResult = time1;
                time1 = time2;
                time2 = timeResult;
            }
            timeResult.Second = time1.Second - time2.Second;
            if (timeResult.Second < 0)
            {
                barrow = 1;
                timeResult.Second += 60;
            }
            timeResult.Minute = time1.Minute - time2.Minute - barrow;
            if (timeResult.Minute < 0)
            {
                barrow = 1;
                timeResult.Minute += 60;
            }
            timeResult.Hour = (time1.Hour - time2.Hour);
            return timeResult;
        }
        public static PersianTime ConvertMinuteToPersainTime(int minute)
        {
            int hour, min;
            hour = minute / 60;
            min = minute % 60;
            PersianTime pt = new PersianTime();
            pt.Hour = hour;
            pt.Minute = min;
            return (pt);
        }
        public static bool IsBigger(PersianTime time1, PersianTime time2)
        {
            if (time1.Hour > time2.Hour)
                return true;
            if ((time1.Hour == time2.Hour) && time1.Minute > time2.Minute)
                return true;
            if ((time1.Hour == time2.Hour) && (time1.Minute == time2.Minute) && (time1.Second > time2.Second))
                return true;
            return false;
        }
        public static bool IsEqualBigger(PersianTime time1, PersianTime time2)
        {

            if (time1.Hour > time2.Hour)
                return true;
            if ((time1.Hour == time2.Hour) && time1.Minute > time2.Minute)
                return true;
            if ((time1.Hour == time2.Hour) && (time1.Minute == time2.Minute) && (time1.Second > time2.Second))
                return true;
            if ((time1.Hour == time2.Hour) && (time1.Minute == time2.Minute) && (time1.Second == time2.Second))
                return true;
            return false;
        }
        public static bool IsEqual(PersianTime time1, PersianTime time2)
        {
            if ((time1.Hour == time2.Hour) && (time1.Minute == time2.Minute) && (time1.Second == time2.Second))
                return true;
            return false;
        }
        public static PersianTime SetTime(int hour, int minute, int second)
        {
            PersianTime pt = new PersianTime();
            pt.Hour = hour; pt.Minute = minute; pt.Second = second;
            return (pt);
        }
        public int ConvertToMinute()
        {
            int minutes = 0;
            minutes = _hour * 60 + _minute;
            return minutes;
        }
        public int ConvertToSecond()
        {
            int Seconds = 0;
            Seconds = _hour * 60 * 60 + _minute * 60 + _second;
            return Seconds;
        }
        public static bool IsBetween(PersianTime betweenTime, PersianTime startTime, PersianTime EndTime)
        {
            if (PersianTime.IsEqualBigger(betweenTime, startTime) && PersianTime.IsEqualBigger(EndTime, betweenTime)) return true;
            else return false;
        }
        public static bool IsBetween(PersianTime betweenStartTime, PersianTime betweenEndTime, PersianTime startTime, PersianTime EndTime)
        {
            if (PersianTime.IsEqualBigger(betweenStartTime, startTime) && PersianTime.IsEqualBigger(EndTime, betweenEndTime)) return true;
            else return false;
        }
        public static PersianTime Add(PersianTime pTime1, PersianTime pTime2)
        {
            int hour = 0, min = 0, sec = 0, day = 0;
            sec = pTime1.Second + pTime2.Second;
            min = pTime1.Minute + pTime2.Minute;
            hour = pTime1.Hour + pTime2.Hour;
            day = pTime1.Day + pTime2.Day;
            if (sec >= 60)
            {
                sec -= 60;
                min++;
            }
            if (min >= 60)
            {
                min -= 60;
                hour++;
            }
            if (hour >= 24)
            {
                hour -= 24;
                day++;
            }
            PersianTime pt = PersianTime.SetTime(hour, min, sec);
            pt.Day = day;
            return pt;
        }
        public static PersianTime Subtract(PersianTime pTime1, PersianTime pTime2)
        {
            int hour = 0, min = 0, sec = 0;
            sec = pTime1.Second - pTime2.Second;
            min = pTime1.Minute - pTime2.Minute;
            hour = pTime1.Hour - pTime2.Hour;
            if (sec < 0)
            {
                sec += 60;
                min--;
            }
            if (min < 0)
            {
                min += 60;
                hour--;
            }
            if (hour < 0)
            {
                hour += 24;

            }
            PersianTime pt = PersianTime.SetTime(hour, min, sec);
            return pt;
        }
        public string PersianStringCustomDate(DateTime date)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(date)}-{pc.GetMonth(date):00.##}-{pc.GetDayOfMonth(date):00.##}";
        }
    }

}
