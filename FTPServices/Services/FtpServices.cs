using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FTPServices.Models;
using Microsoft.Extensions.Options;

namespace FTPServices.Services
{
    public interface IFtpServices
    {
        int GetCalculatePower();
    }
    public class FtpServices : IFtpServices
    {
        private readonly IOptions<FtpSettings> _ftpOptions;

        public FtpServices(IOptions<FtpSettings> ftpOptions)
        {
            _ftpOptions = ftpOptions;
        }
        public int GetCalculatePower()
        {

            string fileName = GetFileName();

            FtpWebRequest ftpWebRequest = CreateFtpWebRequest(WebRequestMethods.Ftp.DownloadFile, fileName);
            FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string line = string.Empty;


            int columnIndex = -1;
            var value = new List<string>();
            foreach (string rows in reader.ReadToEnd().Split('\n').Skip(5))
            {
                if (!string.IsNullOrEmpty(rows))
                {
                    if (columnIndex == -1)
                    {
                        columnIndex = rows.Split(';').ToList().IndexOf("Current_Day_Energy");
                    }
                    else
                    {
                        var row = rows.Split(';');
                        if (row.Length > 10)
                            value.Add(row[columnIndex]);
                    }

                }
            }

            reader.Dispose();
            reader.Close();


            return calculateGeneratePower(value);
        }

        private string GetFileName()
        {
            FtpWebRequest ftpWebRequest = CreateFtpWebRequest(WebRequestMethods.Ftp.ListDirectory);
            FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            DateTime todayDateTime = DateTime.Now;
            // We need to minus with 1, because we don't have files created in 2022
            string year = (todayDateTime.Year - 1).ToString().Substring(2, 2);
            string month = todayDateTime.Month.ToString().PadLeft(2, '0');
            string day = todayDateTime.Day.ToString().PadLeft(2, '0');
            string hour = todayDateTime.Hour.ToString().PadLeft(2, '0');

            if (Convert.ToInt32(hour) < 6)
                hour = "06";

            if (Convert.ToInt32(hour) > 19)
                hour = "19";

            string date = new StringBuilder()
                .Append(year)
                .Append(month)
                .Append(day)
                .Append(hour)
                .ToString();

            string line = string.Empty;
            string filename = string.Empty;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(date))
                    filename = line;
            }
            reader.Close();
            response.Close();
            return filename;
        }

        private int calculateGeneratePower(List<string> powers)
        {
            return Convert.ToInt32(powers.Last()) - Convert.ToInt32(powers.First());
        }

        private FtpWebRequest CreateFtpWebRequest(string requestMethod, string filename = null)
        {
            string ftpServerUri = _ftpOptions.Value.FtpServerUri;

            if (filename != null)
            {
                ftpServerUri = $"{ftpServerUri}/{filename}";
            }

            string userName = _ftpOptions.Value.Username;
            string password = _ftpOptions.Value.Password;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerUri); ;
            request.Method = requestMethod;

            request.Credentials = new NetworkCredential(userName, password);

            return request;

        }
    }
}
