﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.Threading;

namespace Bonsai.Vision
{
    public class CvCameraCaptureSource : Source<IplImage>
    {
        CvCapture capture;
        Thread captureThread;
        volatile bool running;
        ManualResetEventSlim stop;

        public CvCameraCaptureSource()
        {
            stop = new ManualResetEventSlim();
        }

        public int Index { get; set; }

        void CaptureNewFrame()
        {
            while (running)
            {
                var image = capture.QueryFrame();
                if (image == null) break;
                OnOutput(image);
            }

            stop.Set();
        }

        public override void Start()
        {
            running = true;
            captureThread.Start();
        }

        public override void Stop()
        {
            running = false;
            stop.Wait();
        }

        public override void Load(WorkflowContext context)
        {
            captureThread = new Thread(CaptureNewFrame);
            capture = CvCapture.CreateCameraCapture(Index);

            var width = (int)capture.GetProperty(CaptureProperty.FRAME_WIDTH);
            var height = (int)capture.GetProperty(CaptureProperty.FRAME_HEIGHT);
            context.AddService(typeof(CvSize), new CvSize(width, height));
            base.Load(context);
        }

        public override void Unload(WorkflowContext context)
        {
            capture.Close();
            captureThread = null;
            context.RemoveService(typeof(CvSize));
            base.Unload(context);
        }
    }
}
