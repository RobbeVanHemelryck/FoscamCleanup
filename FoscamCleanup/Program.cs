using Newtonsoft.Json;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Timers;

namespace FoscamCleanup
{
    class Program
    {
        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return objects;
        }
        
        static void Main(string[] args)
        {
            int totalCounter = 0;
            Stopwatch timer = new Stopwatch();
            Stopwatch groupTimer = new Stopwatch();
            timer.Start();

            //Create logging file
            string logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", $"{DateTime.Now.ToString("yyyy-MM-dd HHumm")}.txt");
            using (TextWriter log = File.CreateText(logPath))
            {
                try
                {
                    timer.Start();

                    //Create all instances of the fileGroupers
                    List<FileGrouper> fileGroupClasses = GetEnumerableOfType<FileGrouper>().ToList();

                    //Populate the fileGroupers with the JSON data
                    List<FileGrouper> fileGroupers = new List<FileGrouper>();
                    using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "settings.json")))
                    {
                        IEnumerable<dynamic> json = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(reader.ReadToEnd());
                        foreach (var fgJson in json)
                        {
                            FileGrouper fg = fileGroupClasses.Single(x => x.AppliesTo(fgJson.name.ToString()));
                            fg.Source = fgJson.source;
                            fg.Destination = fgJson.destination;
                            fileGroupers.Add(fg);
                        }
                    }

                    //Execute the grouping for each grouper
                    foreach (FileGrouper fileGrouper in fileGroupers)
                    {
                        int counter = 0;
                        try
                        {
                            log.WriteLine($"---------------------------------------- {fileGrouper.Name} ----------------------------------------");
                            groupTimer.Restart();

                            //Get all relevant files from the source directory
                            IEnumerable<string> sourceFiles = Directory.GetFiles(fileGrouper.Source).Where(x => fileGrouper.IsRelevantFile(x));

                            //Group files per DateTime, which is parsed from the file name
                            var fileGroups = sourceFiles.GroupBy(x => fileGrouper.GetDateFromFileName(x)).ToList();

                            //Remove files from today
                            fileGroups.RemoveAll(x => x.Key.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"));

                            //Create a folder in the destination per group and move the files over there
                            foreach (var group in fileGroups)
                            {
                                string groupDestination = Path.Combine(fileGrouper.Destination, group.Key.ToString("yyyy-MM-dd"));
                                Directory.CreateDirectory(groupDestination);
                                foreach (string file in group)
                                {
                                    counter++;
                                    string destination = Path.Combine(groupDestination, Path.GetFileName(file));
                                    File.Move(file, destination);
                                    log.WriteLine($"{counter.ToString().PadRight(6)} {file}   ->   {destination}");
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            log.WriteLine($"An exception occured: {e.Message}");
                            log.WriteLine();
                        }
                        finally
                        {
                            totalCounter += counter;
                            log.WriteLine($"Processed {counter} files in {groupTimer.Elapsed.Seconds}.{groupTimer.Elapsed.Milliseconds}s");
                            log.WriteLine();
                            log.WriteLine();
                        }
                    }
                    log.WriteLine($"Processed {totalCounter} total files in {timer.Elapsed.Seconds}.{timer.Elapsed.Milliseconds}s");
                    timer.Stop();
                    groupTimer.Stop();
                }
                catch(Exception e)
                {
                    //Log the exception
                    log.WriteLine();
                    log.WriteLine();
                    log.WriteLine($"An exception occured: {e.Message}");
                    log.WriteLine();
                    log.WriteLine();
                    log.WriteLine($"Processed {totalCounter} total files in {timer.Elapsed.Seconds}.{timer.Elapsed.Milliseconds}s");
                    timer.Stop();
                    groupTimer.Stop();
                }
            }
                

                ////Delete folders older than 6 months
                //Console.WriteLine("DELETING OLD FOLDERS -------------------------------------------------------");
                //List<string> snapDestinations = Directory.GetDirectories(snapDestination).ToList();
                //List<string> recordDestinations = Directory.GetDirectories(videoDestination).ToList();
                //List<string> concat = snapDestinations.Concat(recordDestinations).ToList();

                //Console.WriteLine($"Found {snapDestinations.Count} directories in {snapDestination}");
                //Console.WriteLine($"Found {recordDestinations.Count} directories in {videoDestination}");
            
                //foreach (string path in concat)
                //{
                //    string dirName = Path.GetFileName(path);
                //    DateTime date = DateTime.ParseExact(dirName, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);

                //    if ((DateTime.Now - date).TotalDays > 360)
                //    {
                //        int seconds = 0;
                //        Timer timer = new Timer();
                //        timer.Interval = 1000;
                //        timer.Elapsed += (x,y) => Console.Write("\rDeleting " + path + ": " + ++seconds + "s");
                //        timer.Start();

                //        Directory.Delete(path, true);
                //        timer.Stop();
                //        Console.WriteLine();
                //    }
                //}
            }
        }
    }