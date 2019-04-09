using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoscamCleanup
{
    class Program
    {

        static void Main(string[] args)
        {
            string filesDir = @"F:\R2_00626E8B547C\snap";
            string videosDir = @"F:\R2_00626E8B547C\record";
            string scheduleDir = @"F:\R2_00626E8B547C\Scheduled\";
            string alarmDir = @"F:\R2_00626E8B547C\Alarm images\";
            string alarmVideoDir = @"F:\R2_00626E8B547C\Alarm videos\";


            //string filesDir = @"C:\Users\Robbe\Documents\foscam snaps";
            //string videosDir = @"C:\Users\Robbe\Documents\foscam videos";
            //string scheduleDir = @"C:\Users\Robbe\Documents\foscam results\Scheduled\";
            //string alarmDir = @"C:\Users\Robbe\Documents\foscam results\Alarms\";
            //string alarmVideoDir = @"C:\Users\Robbe\Documents\foscam results\Alarm videos\";





            string currentDay = DateTime.Now.Date.ToString("yyyyMMdd");
            var files = Directory.GetFiles(filesDir);


            //Schedule files
            Console.WriteLine("SCHEDULE FILES -------------------------------------------------------");
            
            var scheduleFiles = files.Where(x => x.Substring(x.LastIndexOf('\\') + 1).StartsWith("Schedule")).Select(x => new { day = x.Substring(x.LastIndexOf('_') + 1, 8), fileName = x}).ToList();
            var dayGroups = scheduleFiles.GroupBy(x => x.day).Where(x => x.Key != currentDay).ToList();
            Console.WriteLine($"Found {dayGroups.Count} days");

            foreach (var dayGroup in dayGroups)
            {
                DateTime date = new DateTime();
                DateTime.TryParseExact(dayGroup.Key, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
    System.Globalization.DateTimeStyles.None, out date);
                string folderName = date.ToString("yyyy-MM-dd");


                if (!Directory.Exists(scheduleDir + folderName))
                {
                    Directory.CreateDirectory(scheduleDir + folderName);

                    //Move files to destination
                    foreach (var item in dayGroup.Select(x => x.fileName))
                    {
                        string fileName = item.Substring(item.LastIndexOf('\\') + 10);
                        File.Move(item, scheduleDir + folderName + "\\" + fileName);
                        Console.WriteLine(alarmDir + folderName + "\\" + fileName);

                    }
                }
            }


            //MDAlarm files
            Console.WriteLine("MDALARM FILES -------------------------------------------------------");

            var MDAlarmFiles = files.Where(x => x.Substring(x.LastIndexOf('\\') + 1).StartsWith("MDAlarm")).Select(x => new { day = x.Substring(x.LastIndexOf('_') + 1, 8), fileName = x }).ToList();
            dayGroups = MDAlarmFiles.GroupBy(x => x.day).Where(x => x.Key != currentDay).ToList();
            Console.WriteLine($"Found {dayGroups.Count} days");

            foreach (var dayGroup in dayGroups)
            {
                DateTime date = new DateTime();
                DateTime.TryParseExact(dayGroup.Key, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
    System.Globalization.DateTimeStyles.None, out date);
                string folderName = date.ToString("yyyy-MM-dd");

                if(!Directory.Exists(alarmDir + folderName))
                {
                    Directory.CreateDirectory(alarmDir + folderName);

                    //Move files to destination
                    foreach (var item in dayGroup.Select(x => x.fileName))
                    {
                        string fileName = item.Substring(item.LastIndexOf('\\') + 9);
                        File.Move(item, alarmDir + folderName + "\\" + fileName);
                        Console.WriteLine(alarmDir + folderName + "\\" + fileName);

                    }
                }
            }

            //MDAlarm files
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
                DateTime.TryParseExact(dayGroup.Key, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
    System.Globalization.DateTimeStyles.None, out date);
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
        }

        private static bool ZippyWippy(string[] files, string zipName, string zipDir)
        {
            try
            {
                FileStream fsOut = File.Create(zipDir + zipName + ".zip");
                ZipOutputStream zipStream = new ZipOutputStream(fsOut);

                zipStream.SetLevel(9);

                int percent = 0;
                for(int i = 0; i < files.Length; i++)
                {
                    string filename = files[i];

                    int previousPercent = percent;
                    percent = (int)(((double)(i + 1) / (double)files.Length) * 100);
                    if (percent > previousPercent)
                    {
                        Console.WriteLine($"     - {percent}% ({i+1}/{files.Length})");
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }

                    FileInfo fi = new FileInfo(filename);

                    string entryName = filename.Substring(filename.LastIndexOf('\\') + 1);
                    entryName = ZipEntry.CleanName(entryName);
                    ZipEntry newEntry = new ZipEntry(entryName);
                    newEntry.DateTime = fi.LastWriteTime;
                    newEntry.Size = fi.Length;

                    zipStream.PutNextEntry(newEntry);
                    byte[] buffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(filename))
                    {
                        StreamUtils.Copy(streamReader, zipStream, buffer);
                    }
                    zipStream.CloseEntry();
                }

                zipStream.IsStreamOwner = true;
                zipStream.Close();

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
