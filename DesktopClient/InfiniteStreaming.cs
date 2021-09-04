using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Google.Protobuf;
using NAudio.Wave;
using System.ComponentModel;
using System.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using DesktopClient.Data;
using DesktopClient.Models;

/*
 * System.Diagnostics.Debug.WriteLine
 */

namespace GoogleAPI
{
    public class InfiniteStreaming
    {
        private string _transcriptid;
        Lecture trans { get; set; }

        private const int SampleRate = 16000;
        private const int ChannelCount = 1;
        private const int BytesPerSample = 2;
        private const int BytesPerSecond = SampleRate * ChannelCount * BytesPerSample;
        private static readonly TimeSpan s_streamTimeLimit = TimeSpan.FromSeconds(290);

        private readonly SpeechClient _client;

        private BackgroundWorker worker = null;
        /// <summary>
        /// Microphone chunks that haven't yet been processed at all.
        /// </summary>
        private readonly BlockingCollection<ByteString> _microphoneBuffer = new BlockingCollection<ByteString>();

        /// <summary>
        /// Chunks that have been sent to Cloud Speech, but not yet finalized.
        /// </summary>
        private readonly LinkedList<ByteString> _processingBuffer = new LinkedList<ByteString>();

        /// <summary>
        /// The start time of the processing buffer, in relation to the start of the stream.
        /// </summary>
        private TimeSpan _processingBufferStart;

        /// <summary>
        /// The current RPC stream, if any.
        /// </summary>
        private SpeechClient.StreamingRecognizeStream _rpcStream;

        /// <summary>
        /// The deadline for when we should stop the current stream.
        /// </summary>
        private DateTime _rpcStreamDeadline;

        private HubConnection _hubConnection;
        
        LectureDbContext db3 = new LectureDbContext();

        /// <summary>
        /// The task indicating when the next response is ready, or when we've
        /// reached the end of the stream. (The task will complete in either case, with a result
        /// of True if it's moved to another response, or False at the end of the stream.)
        /// </summary>
        private ValueTask<bool> _serverResponseAvailableTask;


        public InfiniteStreaming()
        {
            _client = SpeechClient.Create();
        }


        private async Task Connect()
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44396/chathub")
            .Build();
            await _hubConnection.StartAsync();
        }


        /// <summary>
        /// Runs the main loop until "exit" or "quit" is heard.
        /// </summary>
        private async Task RunAsync(CancellationToken cancelToken)
        {
            using (var microphone = StartListening())
            {
                _ = Connect();

                while (!cancelToken.IsCancellationRequested)
                {

                    await MaybeStartStreamAsync();
                    // ProcessResponses will return false if it hears "exit" or "quit".
                    if (!ProcessResponses())
                    {
                        return;
                    }
                    await TransferMicrophoneChunkAsync();
                }

            }
        }

        /// <summary>
        /// Starts a new RPC streaming call if necessary. This will be if either it's the first call
        /// (so we don't have a current request) or if the current request will time out soon.
        /// In the latter case, after starting the new request, we copy any chunks we'd already sent
        /// in the previous request which hadn't been included in a "final result".
        /// </summary>
        private async Task MaybeStartStreamAsync()
        {
            var now = DateTime.UtcNow;
            if (_rpcStream != null && now >= _rpcStreamDeadline)
            {
                Console.WriteLine($"Closing stream before it times out");
                await _rpcStream.WriteCompleteAsync();
                _rpcStream.GrpcCall.Dispose();
                _rpcStream = null;
            }

            // If we have a valid stream at this point, we're fine.
            if (_rpcStream != null)
            {
                return;
            }
            // We need to create a new stream, either because we're just starting or because we've just closed the previous one.
            _rpcStream = _client.StreamingRecognize();
            _rpcStreamDeadline = now + s_streamTimeLimit;
            _processingBufferStart = TimeSpan.Zero;
            _serverResponseAvailableTask = _rpcStream.GetResponseStream().MoveNextAsync();
            var context = new SpeechContext { Phrases = { "threads", "canonical", "is", "to", "gather", "in", "race", "data" } };

            await _rpcStream.WriteAsync(new StreamingRecognizeRequest
            {
                StreamingConfig = new StreamingRecognitionConfig
                {
                    Config = new RecognitionConfig
                    {
                        Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                        SampleRateHertz = SampleRate,
                        LanguageCode = "en-US",
                        EnableAutomaticPunctuation = true,
                        MaxAlternatives = 1,
                        SpeechContexts = { context }
                    },
                    InterimResults = true,
                }
            }); ;

            Console.WriteLine($"Writing {_processingBuffer.Count} chunks into the new stream.");
            foreach (var chunk in _processingBuffer)
            {
                await WriteAudioChunk(chunk);
            }
        }


        private void ReturnTranslate()
        {
            return;
        }


        /// <summary>
        /// Processes responses received so far from the server,
        /// returning whether "exit" or "quit" have been heard.
        /// </summary>
        private bool ProcessResponses()
        {

            // maybe try putting an instance variable here before the while loop.

            // Take the while loop and make a new function or task for it. Have it in a scope(method) that u can actually call it and edit it.

            while (_serverResponseAvailableTask.IsCompleted && _serverResponseAvailableTask.Result)
            {
                // Use this for debugging to see how "be" is printed only when words are being processed.
                //Console.WriteLine("be");

                var response = _rpcStream.GetResponseStream().Current;
                _serverResponseAvailableTask = _rpcStream.GetResponseStream().MoveNextAsync();


                /// MY EDITS  ////
                var wine = response.Results.First();
                string grape = wine.Alternatives[0].Transcript;

                string message = wine.Alternatives[0].Transcript;
                //Console.WriteLine($"Transcr: {grape}");

                // Uncomment this to see the details of interim results.
                //Console.WriteLine($"Response: {response}");
                //Console.WriteLine(response);

                //string cuppa = response.ToString();
                //response.Results

                /*
                if(translation)
                {
                
                    var answerz = client.TranslateText(
                        text: grape,
                        targetLanguage: "en",  // Russian
                        sourceLanguage: "es");  // English
                                                //Console.WriteLine(response.TranslatedText);
                                                //return response.TranslatedText;
                    grape = answerz.TranslatedText;
                }
                */

                // Sets up SQL function parameters
                // Runs SQL update_translate function, with the argument grape
                /*
                sql = @"select * from update_translate(:_textdata)";
                cmd = new NpgsqlCommand(sql, dbcon);
                cmd.Parameters.AddWithValue("_textdata", grape);
                int syncedtext = (int)cmd.ExecuteScalar();
                */

                // Get final result transcript
                var finalResult = response.Results.FirstOrDefault(r => r.IsFinal);

                int isfinal;


                if (wine.IsFinal)
                {
                    // If isFinal is true, append the new sentence to the transcript.
                    System.Diagnostics.Debug.WriteLine("DONE IS FINAL!!!!!");
                    /* Signal R Stuff */
                    isfinal = 1;
                    _hubConnection.SendAsync("SendGroupMessage", _transcriptid, isfinal, message);
                    trans.Transcript = trans.Transcript + " " + message;
                    db3.SaveChanges();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(grape);
                    isfinal = 0;
                    _hubConnection.SendAsync("SendGroupMessage", _transcriptid, isfinal, message);
                }






                // See if one of the results is a "final result". If so, we trim our
                // processing buffer.
                // var finalResult = response.Results.FirstOrDefault(r => r.IsFinal);



                if (finalResult != null)
                {
                    //string transcript = finalResult.Alternatives[0].Transcript;
                    /*
                    // Runs every time a word is translated. Console.WriteLine("11111");
                    string transcript = finalResult.Alternatives[0].Transcript;

                    // If isFinal is true, append the new sentence to the transcript.
                    sqll = @"select * from sync_transcript(:_tran)";
                    cmd1 = new NpgsqlCommand(sqll, dbcon);
                    cmd1.Parameters.AddWithValue("_tran", transcript);
                    int syncedtextt = (int)cmd1.ExecuteScalar();
                    */


                    /*
                    //Console.WriteLine($"Transcript: {transcript}");
                    if (transcript.ToLowerInvariant().Contains("exit") ||
                        transcript.ToLowerInvariant().Contains("quit"))
                    {
                        return false;
                    }
                    */

                    TimeSpan resultEndTime = finalResult.ResultEndTime.ToTimeSpan();

                    // Rather than explicitly iterate over the list, we just always deal with the first
                    // element, either removing it or stopping.
                    int removed = 0;
                    while (_processingBuffer.First != null)
                    {
                        var sampleDuration = TimeSpan.FromSeconds(_processingBuffer.First.Value.Length / (double)BytesPerSecond);
                        var sampleEnd = _processingBufferStart + sampleDuration;

                        // If the first sample in the buffer ends after the result ended, stop.
                        // Note that part of the sample might have been included in the result, but the samples
                        // are short enough that this shouldn't cause problems.
                        if (sampleEnd > resultEndTime)
                        {
                            break;
                        }
                        _processingBufferStart = sampleEnd;
                        _processingBuffer.RemoveFirst();
                        removed++;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Takes a single sample chunk from the microphone buffer, keeps a local copy
        /// (in case we need to send it again in a new request) and sends it to the server.
        /// </summary>
        /// <returns></returns>
        private async Task TransferMicrophoneChunkAsync()
        {
            // This will block - but only for ~100ms, unless something's really broken.
            var chunk = _microphoneBuffer.Take();
            _processingBuffer.AddLast(chunk);
            await WriteAudioChunk(chunk);
        }

        /// <summary>
        /// Writes a single chunk to the RPC stream.
        /// </summary>
        private Task WriteAudioChunk(ByteString chunk) =>
            _rpcStream.WriteAsync(new StreamingRecognizeRequest { AudioContent = chunk });

        /// <summary>
        /// Starts listening to input device 0, and adds an event handler which simply adds
        /// the sample to the microphone buffer. The returned <see cref="WaveInEvent"/> must
        /// be disposed after we've finished with it.
        /// </summary>
        private WaveInEvent StartListening()
        {

            var waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(SampleRate, ChannelCount)
            };

            //waveIn = new WaveIn();
            //waveIn.WaveFormat = new WaveFormat(SampleRate, ChannelCount);


            waveIn.DataAvailable += (sender, args) =>
            _microphoneBuffer.Add(ByteString.CopyFrom(args.Buffer, 0, args.BytesRecorded));
            waveIn.StartRecording();




            return waveIn;
        }




        /// <summary>
        /// Note: the calling code expects a Task with a return value for consistency with
        /// other samples.
        /// We'll always return a task which eventually faults or returns 0.
        /// </summary>
        public static async Task<int> RecognizeAsync(CancellationToken cancelToken)
        {
            //var instance = new InfiniteStreaming();
            //await instance.RunAsync(cancelToken);
            return 0;
        }


        public async void StartTranslate(CancellationToken cancelToken, string transcriptid)
        {

            cancelToken.ThrowIfCancellationRequested();
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += delegate
            {
                RecognizeAsync(cancelToken);
            };
            worker.RunWorkerAsync();

            _transcriptid = transcriptid;
            _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44396/chathub")
            .Build();
            await _hubConnection.StartAsync();

            trans = db3.tblLectures.FirstOrDefault(a => a.Id == _transcriptid);
            trans.Transcript = " ";

        }

    }
}
