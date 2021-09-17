using DesktopClient.Data;
using DesktopClient.Models;
using Microsoft.AspNetCore.SignalR.Client; // REMOVE AFTER TESTING
using Microsoft.EntityFrameworkCore;
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

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for CoursePage.xaml
    /// </summary>
    public partial class ClassPage : Window
    {
        Course _course;
        string _userid;
        Lecture _transcript = new Lecture();
        private HubConnection _hubConnection; // REMOVE AFTER TESTING

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken internalToken;
        public ClassPage(Course course, string userid)
        {
            _course = course;
            _userid = userid;
            InitializeComponent();
            coursenumber.Text = _course.CourseNum;
            // Need to add check so only teachers can start a transcript.

        }
        private async void CreateTranscript()
        {
            var rand = new Random();
            _transcript.Id = (rand.Next(100000, 1000000)).ToString();
            _transcript.HostId = _userid;
            _transcript.CourseId = _course.Id;
            _transcript.DateOf = "Date";
            _transcript.TimeStart = "Time1";
            _transcript.IsComplete = 0;

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
        private async void BeginSignalR(string transcriptid)
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44396/chathub")
            .Build();
            await _hubConnection.StartAsync();
        }
        private void btnTranscribe(object sender, RoutedEventArgs e)
        {
            /*
            CreateTranscript();
            
            var instance = new GoogleAPI.InfiniteStreaming();
            CancellationToken internalToken = tokenSource.Token;
            instance.StartTranslate(_transcript.Id, internalToken);
            
            BeginSignalR(_transcript.Id);
            */
            TranscriptPage win = new TranscriptPage(_course, _userid);
            win.Show();

        }

        private async void btnTestSignalR(object sender, RoutedEventArgs e)
        {
            using LectureDbContext db3 = new LectureDbContext();
            var trans = db3.tblLectures.FirstOrDefault(a => a.Id == _transcript.Id);
            trans.Transcript = " ";
            for (int i = 0; i < 500; i++)
            {
                string pp = i.ToString();
                await _hubConnection.SendAsync("SendGroupMessage", _transcript.Id, 1, pp);
                trans.Transcript = trans.Transcript + " " + pp;
                db3.SaveChanges();
            }
            await _hubConnection.SendAsync("SendGroupMessage", _transcript.Id, 1, "ALL DONE!!!");
            trans.Transcript = trans.Transcript + " " + "ALL DONE!!!";
            db3.SaveChanges();
            db3.Dispose();
        }

        private void btnStopTranscription(object sender, RoutedEventArgs e)
        {

        }
    }
}
