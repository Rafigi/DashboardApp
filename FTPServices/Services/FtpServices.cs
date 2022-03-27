using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FTPServices.Models;
using Microsoft.Extensions.Options;

namespace FTPServices.Services
{
    public interface IFtpServices
    {
        Task<SolarPanel> GetCalculateSolarPower();
    }
    public class FtpServices : IFtpServices
    {
        private readonly IOptions<FtpSettings> _ftpOptions;

        public FtpServices(IOptions<FtpSettings> ftpOptions)
        {
            _ftpOptions = ftpOptions;
        }

        public async Task<SolarPanel> GetCalculateSolarPower()
        {
            string fileName = GetFileName();

            FtpWebRequest ftpWebRequest = CreateFtpWebRequest(WebRequestMethods.Ftp.DownloadFile, fileName);
            FtpWebResponse response = (FtpWebResponse)(await ftpWebRequest.GetResponseAsync());

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string line = string.Empty;

            int columnIndex = -1;
            var value = new List<int>();
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

                        if (row.Length >= columnIndex + 1)
                        {
                            if (int.TryParse(row[columnIndex], out int result))
                                value.Add(result);
                        }
                    }
                }
            }

            reader.Dispose();
            reader.Close();

            return new SolarPanel()
            {
                Energy = calculateGeneratePower(value),
                Date = DateTime.Now
            };
        }

        private string GetFileName()
        {
            FtpWebRequest ftpWebRequest = CreateFtpWebRequest(WebRequestMethods.Ftp.ListDirectory);
            FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();

            Stream responseStream = response.GetResponseStream();
            MemoryStream destination = new MemoryStream();
            responseStream.CopyTo(destination);


            string filename = TryLookUpFileName(destination, DateTime.Now);

            destination.Dispose();
            response.Close();
            return filename;
        }

        private string TryLookUpFileName(Stream stream, DateTime date)
        {
            string searchDate = DateBuilder(date);
            string filename = null;


            string line = string.Empty;
            StreamReader sr = new StreamReader(stream);
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains(searchDate))
                    filename = line;

            }

            stream.Position = 0;

            if (filename == null)
                return TryLookUpFileName(stream, date.AddHours(-1));

            return filename;
        }

        private string DateBuilder(DateTime dateTime)
        {
            DateTime todayDateTime = dateTime;
            // We need to minus with 1, because we don't have files created in 2022
            string year = (todayDateTime.Year - 1).ToString().Substring(2, 2);
            string month = todayDateTime.Month.ToString().PadLeft(2, '0');
            string day = todayDateTime.Day.ToString().PadLeft(2, '0');
            string hour = todayDateTime.Hour.ToString().PadLeft(2, '0');

            return new StringBuilder()
                .Append(year)
                .Append(month)
                .Append(day)
                .Append(hour)
                .ToString();
        }

        private int calculateGeneratePower(List<int> powers)
        {
            return powers.Last() - powers.First();
        }

        private FtpWebRequest CreateFtpWebRequest(string requestMethod, string filename = null)
        {
            string ftpServerUri = _ftpOptions.Value.FtpServerUri;
            try
            {
                if (filename != null)
                {
                    ftpServerUri = $"{ftpServerUri}/{filename}";
                }

                string userName = _ftpOptions.Value.Username;
                string password = _ftpOptions.Value.Password;

                var request = WebRequest.Create(ftpServerUri); ;
                request.Method = requestMethod;

                request.Credentials = new NetworkCredential(userName, password);

                return (FtpWebRequest)request;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
    }
}
