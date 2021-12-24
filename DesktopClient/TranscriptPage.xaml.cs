using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DesktopClient.Data;
using DesktopClient.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for TranscriptPage.xaml
    /// </summary>
    public partial class TranscriptPage : Window
    {
        Course _course;
        string _userid;
        Lecture _transcript = new Lecture();
        InfiniteStreaming translate;
        public TranscriptPage(Course course, string userid)
        {
            translate = new InfiniteStreaming(this);
            _course = course;
            _userid = userid;
            InitializeComponent();
        }
        private async Task CreateTranscript()
        {
            var rand = new Random();
            _transcript.Id = (rand.Next(100000, 1000000)).ToString();
            _transcript.HostId = _userid;
            _transcript.CourseId = _course.Id;
            _transcript.DateOf = DateTime.Now.ToShortDateString();
            _transcript.TimeStart = DateTime.Now.ToShortTimeString();
            _transcript.IsComplete = 0;
            _transcript.Transcript = " ";

            using LectureDbContext db = new LectureDbContext();
            db.Add(_transcript);
            await db.SaveChangesAsync();
            db.Dispose();


            using CourseDbContext db2 = new CourseDbContext();
            if (String.IsNullOrEmpty(_course.Transcripts))
            {
                _course.Transcripts = _transcript.Id;
                db2.Entry(_course).State = EntityState.Modified;
                await db2.SaveChangesAsync();
                db2.Dispose();
            }
            else
            {
                _course.Transcripts = _course.Transcripts + " " + _transcript.Id;
                db2.Entry(_course).State = EntityState.Modified;
                await db2.SaveChangesAsync();
            }
        }
        private async void btnStart(object sender, RoutedEventArgs e)
        {
            await CreateTranscript();
            //translate.isRunning = true;
            var task = Task.Run(() =>
            {
                // System.Diagnostics.Debug.WriteLine("inside of task");
                translate.BeginTranslate(_transcript.Id);
            });
            /*
            var instance = new GoogleAPI.InfiniteStreaming();
            CancellationToken internalToken = tokenSource.Token;
            instance.StartTranslate(internalToken, _transcript.Id);
            */
        }

        private async void btnStop(object sender, RoutedEventArgs e)
        {
            //translate.isRunning = false;

            translate.EndProcess();

        }
    }
}
