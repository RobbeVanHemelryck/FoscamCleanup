using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Timers;

namespace FoscamCleanup
{
    class Program
    {
        public static void FileUploadSFTP()
        {
            //var host = "taltiko.stackstorage.com";
            //var port = 22;
            //var username = "taltiko@taltiko.stackstorage.com";
            //var password = "UucY6wuv";

            //byte[] file = File.ReadAllBytes(@"C:\Users\Robbe\Documents\Fitness.xlsx");

            //using (var client = new SftpClient(host, port, username, password))
            //{
            //    client.Connect();
            //    if (client.IsConnected)
            //    {
            //        Debug.WriteLine("I'm connected to the client");

            //        using (var ms = new MemoryStream(file))
            //        {
            //            client.BufferSize = (uint)ms.Length; // bypass Payload error large files
            //            client.CreateDirectory(ms, "Fitness.xlsx"); // hier zo
            //        }
            //    }
            //    else
            //    {
            //        Debug.WriteLine("I couldn't connect");
            //    }
            //}
        }

        static void Main(string[] args)
        {
            //FileUploadSFTP();
            //return;
            string filesDir = @"F:\R2_00626E8B547C\snap";
            string videosDir = @"F:\R2_00626E8B547C\record";
            string scheduleDir = @"F:\R2_00626E8B547C\Scheduled\";
            string alarmVideoDir = @"F:\R2_00626E8B547C\Alarm videos\";

            string currentDay = DateTime.Now.Date.ToString("yyyyMMdd");
            var files = Directory.GetFiles(filesDir);

            //Schedule files
            Console.WriteLine("SCHEDULE FILES -------------------------------------------------------");

            var scheduleFiles = files.Where(x => x.Substring(x.LastIndexOf('\\') + 1).StartsWith("Schedule")).Select(x => new { day = x.Substring(x.LastIndexOf('_') + 1, 8), fileName = x }).ToList();
            var dayGroups = scheduleFiles.GroupBy(x => x.day).Where(x => x.Key != currentDay).ToList();
            Console.WriteLine($"Found {dayGroups.Count} days");

            foreach (var dayGroup in dayGroups)
            {
                DateTime date = new DateTime();
                DateTime.TryParseExact(dayGroup.Key, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date);
                string folderName = date.ToString("yyyy-MM-dd");


                //If the directory doesnt exist - create it and move files
                if (!Directory.Exists(scheduleDir + folderName))
                {
                    Directory.CreateDirectory(scheduleDir + folderName);

                    //Move files to destination
                    foreach (var item in dayGroup.Select(x => x.fileName))
                    {
                        string fileName = item.Substring(item.LastIndexOf('\\') + 10);
                        File.Move(item, scheduleDir + folderName + "\\" + fileName);
                        Console.WriteLine(scheduleDir + folderName + "\\" + fileName);
                    }
                }
            }

            Console.WriteLine("MDALARM VIDEO FILES -------------------------------------------------------");

            var MDAlarmVideoFiles = Directory.GetFiles(videosDir).Select(x => new { day = x.Substring(x.LastIndexOf('\\') + 9, 8), fileName = x }).ToList();
            dayGroups = MDAlarmVideoFiles.GroupBy(x => x.day).Where(x => x.Key != currentDay).ToList();
            Console.WriteLine($"Found {dayGroups.Count} days");

            foreach (var dayGroup in dayGroups)
            {
                for (int i = 0; i < dayGroup.Count(); i++)
                {
                    Console.WriteLine(dayGroup.Select(x => x.fileName).ToList()[i]);
                }
                Console.WriteLine(dayGroup.Key);
                DateTime date = new DateTime();
                DateTime.TryParseExact(dayGroup.Key, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date);
                string folderName = date.ToString("yyyy-MM-dd");

                if (!Directory.Exists(alarmVideoDir + folderName))
                {
                    Directory.CreateDirectory(alarmVideoDir + folderName);

                    //Move files to destination
                    foreach (var item in dayGroup.Select(x => x.fileName))
                    {
                        string fileName = item.Substring(item.LastIndexOf('\\') + 9);
                        File.Move(item, alarmVideoDir + folderName + "\\" + fileName);
                        Console.WriteLine(alarmVideoDir + folderName + "\\" + fileName);
                    }
                }
            }

            //Delete folders older than 6 months
            Console.WriteLine("DELETING OLD FOLDERS -------------------------------------------------------");
            List<string> scheduleDirs = Directory.GetDirectories(scheduleDir).ToList();
            List<string> alarmVideoDirs = Directory.GetDirectories(alarmVideoDir).ToList();
            List<string> concat = scheduleDirs.Concat(alarmVideoDirs).ToList();

            Console.WriteLine($"Found {scheduleDirs.Count} directories in {scheduleDir}");
            Console.WriteLine($"Found {alarmVideoDirs.Count} directories in {alarmVideoDir}");
            
            foreach (string path in concat)
            {
                string dirName = Path.GetFileName(path);
                DateTime date = DateTime.ParseExact(dirName, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);

                if ((DateTime.Now - date).TotalDays > 360)
                {
                    int seconds = 0;
                    Timer timer = new Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += (x,y) => Console.Write("\rDeleting " + path + ": " + ++seconds + "s");
                    timer.Start();

                    Directory.Delete(path, true);
                    timer.Stop();
                    Console.WriteLine();
                }
            }
        }
    }

}
