using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net.FtpClient;
using System.Net;

namespace TSD_Slider.Communication
{
    public class FTP
    {
        private string IPAddress;
        private string Username;
        private string Password;
        private FtpClient client;
        ManualResetEvent m_reset = new ManualResetEvent(false);

        public FTP(string ipAddress, string username, string password)
        {
            IPAddress = ipAddress;
            Username = username;
            Password = password;
        }

        private bool connect()
        {
            try
            {
                client = new FtpClient();
                client.Host = IPAddress;
                client.Credentials = new NetworkCredential(Username, Password);
                client.Connect();

                Trace.WriteLine("Connection fully connected using : " + IPAddress + " " + Username + " " + Password);
                Trace.WriteLine("Working Directory : " + client.GetWorkingDirectory());
                return true;
                //client.Connect();
            }
            catch (ObjectDisposedException notConnected)
            {
                Trace.WriteLine(notConnected.Message);
                return false;
            }
        }
       
        /// <summary>
        /// Downloads the designated file into pcFilePath
        /// </summary>
        /// <param name="robotFilePath"></param>
        /// <param name="robotFileName"></param>
        /// <param name="pcFilePath">//pcfile path should include the / or \(network) in the end</param>
        public void Download(string robotFilePath, string robotFileName, string pcFilePath)
        {
            FtpWebRequest reqFTP;

            //filePath = <<The full path where the file is to be created. the>>, 
            //fileName = <<Name of the file to be createdNeed not 
            //name on FTP server. name name()>>
            try
            {
                //pcfile path should include the / or \(network) in the end
                if (pcFilePath == "")
                    pcFilePath = Environment.CurrentDirectory;

                FileStream outputStream = new FileStream(pcFilePath +
                                robotFileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" +
                                IPAddress + "/" + robotFilePath + "/" + robotFileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Username,
                                                            Password);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }

        }

        public List<string> GetFileList(string filepath)
        {
            List<string> resultedFiles;
            FtpWebRequest reqFTP;
            try
            {
                resultedFiles = new List<string>();
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(
                          "ftp://" + IPAddress + "/" + filepath));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Username,
                                                           Password);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response
                                                .GetResponseStream());

                string line = reader.ReadLine();
                while (line != null)
                {
                    resultedFiles.Add(line);
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                return resultedFiles;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                resultedFiles = null;
                return resultedFiles;
            }
        }

        public void getTimeOfFile(string fileName)
        {
            if (connect())
            {
                DateTime modify = client.GetModifiedTime(fileName);
                Trace.WriteLine(modify.Date.ToString() + "_" + modify.Hour.ToString() + "_" + modify.Minute.ToString());
            }
            else
                Trace.WriteLine("Could not connect to host!");
        }

        public void GetNameListingWithModifiedTimes(string filepath)
        {
            connect();
            client.SetWorkingDirectory(filepath);
            foreach (string s in client.GetNameListing())
            {
                DateTime modify = client.GetModifiedTime(s);
                Trace.WriteLine(s + "  Size:" + client.GetFileSize(s).ToString() +
                    " Time: " + modify.Date.Year +
                    "_" + modify.Date.Month.ToString() +
                    "_" + modify.Date.Day +
                    " -- " + modify.Date.Hour +
                    ":" + modify.Date.Minute +
                    ":" + modify.Date.Second);

                
            }
            client.Disconnect();
        }

    }
}
